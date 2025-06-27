namespace Api.Utils;

using static BCrypt.Net.BCrypt;

public static class HashHelper
{
    /// <summary>
    /// يقوم بإنشاء تجزئة آمنة باستخدام خوارزمية BCrypt.
    /// </summary>
    public static string CreateHash(string plainPassword)
    {
        return HashPassword(plainPassword);  // هنا نستدعي دالة BCrypt.HashPassword بفضل using static
    }

    /// <summary>
    /// يتحقق مما إذا كانت كلمة المرور النصية تطابق التجزئة المخزنة.
    /// </summary>
    public static bool VerifyPassword(string plainPassword, string hashedPassword)
    {
        return Verify(plainPassword, hashedPassword);  // دالة BCrypt.Verify
    }
}
