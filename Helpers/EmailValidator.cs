using System.Net.Mail;
namespace AvaTerminal3.Helpers;

public static class EmailValidator
{
    public static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    public static string? SetLowerCase(this string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;
        else
            return email.ToLower();
    }
}
