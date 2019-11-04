public class LoginCreds
{
    public string UserName { get; set; }
    public string Password { get; set; }
}

public class CXAuthRequest
{
    public string SSOKey { get; set; } // From SAML attributes (Worker Attributes)
    public string UserName { get; set; } // From Flex Agent Dashboard Redux Store.
}
public class CXAuthCheckRequest
{
    public string UserName { get; set; } // From SAML attributes (Worker Attributes)
    public string Token { get; set; } // Current Bearer Token
}
public class CXAuthTokenCheckRequest
{
    public string Token { get; set; } // Current Bearer Token
}

public class CX_JWT_Payload
{
    public long sub { get; set; }
    public long exp { get; set; }
}