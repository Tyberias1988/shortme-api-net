namespace shortme_api_net.Helpers;

public class RandomStringGenerator
{
    public static string RandomString(int n)
    {
        var guid = Guid.NewGuid().ToString().Replace("-", "");

        if (n > guid.Length)
        {
            throw new ArgumentException("The length is limited to " + guid.Length + " chars");
        }

        return guid.Substring(0, n);
    }
}