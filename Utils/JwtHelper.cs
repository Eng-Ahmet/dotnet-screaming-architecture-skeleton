using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

public class JwtHelper
{
    private readonly string? _secret;
    private readonly string? _issuer;
    private readonly string? _audience;

    public JwtHelper(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"];
        _issuer = configuration["Jwt:Issuer"];
        _audience = configuration["Jwt:Audience"];
    }

    // إنشاء توكن JWT
    public string GenerateToken(int userId, string userName, string[] roles, int expireMinutes = 60)
    {
        if (string.IsNullOrEmpty(_secret))
            throw new InvalidOperationException("JWT secret is not configured.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, userName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        // إضافة الأدوار كـ claims متعددة
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expireMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // التحقق من صحة التوكن واستخراج ClaimsPrincipal
    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        if (string.IsNullOrEmpty(_secret))
            throw new InvalidOperationException("JWT secret is not configured.");

        var key = Encoding.UTF8.GetBytes(_secret);

        try
        {
            var parameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero // بدون سماح بفارق الوقت
            };

            var principal = tokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);

            // يمكن هنا التأكد أن التوكن هو من نوع JWT
            if (validatedToken is JwtSecurityToken jwtToken &&
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }
        }
        catch
        {
            // التوكن غير صالح أو منتهي الصلاحية
            return null;
        }

        return null;
    }

    // استخراج UserId من التوكن (كمثال)
    public string? GetUserIdFromToken(string token)
    {
        var principal = ValidateToken(token);
        return principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
    }
}
