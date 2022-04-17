namespace OpenRequest.Authentication.Configuration;

public class JwtConfig
{
    public string Secret { get; set; } = null!;
    public TimeSpan ExpiryTimeFrame { get; set; }
}