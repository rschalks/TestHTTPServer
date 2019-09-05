namespace LWHTTP.HTTPCommon
{
    public enum HTTPConnection
    {
        KeepAlive = 0,
        TransferEncoding = 1,
        TEConnection = 2,
        Trailer = 3,
        Upgrade = 4,
        ProxyAuthorisation = 5,
        ProxyAuthenticate = 6,
        Close = 7,
    }

    public enum HTTPRequestMethod
    {
        Connect = 0,
        Delete = 1,
        Get = 2,
        Head = 3,
        Options = 4,
        Patch = 5,
        Post = 6,
        Put = 7,
        Trace = 8,
    }

    public enum HTTPStatusCodeCategory
    {
        Informational = 1,
        Succesful = 2,
        Redirection = 3,
        ClientError = 4,
        ServerError = 5,
    }

    public enum HTTPStatusCode
    {
        Continue = 100,
        SwitchingProtocols = 101,
        EarlyHints = 102,
        OK = 200,
        Created = 201,
        Accepted = 202,
        NonAuthorativeInformation = 203,
        NoContent = 204,
        ResetContent = 205,
        PartialContent = 206,
        MultipleChoices = 300,
        MovedPermanently = 301,
        Found = 302,
        SeeOther = 303,
        NotModified = 304,
        TemporaryRedirect = 307,
        PermanentRedirect = 308,
        BadRequest = 400,
        Unathorized = 401,
        PaymentRequired = 402,
        Forbidden = 403,
        NotFound = 404,
        MethodNotAllowed = 405,
        NotAcceptable = 406,
        ProxyAuthenticationRequired = 407,
        RequestTimeout = 408,
        Conflict = 409,
        Gone = 410,
        LengthRequired = 411,
        PreconditionFailed = 412,
        PayloadTooLarge = 413,
        URITooLong = 414,
        UnsupportedMediaType = 415,
        RangeNotSatisfiable = 416,
        ExpectationFailed = 417,
        ImATeapot = 418,
        UnprocessableEntity = 422,
        TooEarly = 425,
        UpgradeRequired = 426,
        PreconditionRequired = 428,
        TooManyRequests = 429,
        RequestHeaderFieldTooLarge = 431,
        UnavailableForLegalReasons = 451,
        InternalServerError = 500,
        NotImplemented = 501,
        BadGateway = 502,
        ServiceUnavailable = 503,
        GatewayTimeout = 504,
        HTTPVersionNotSupported = 505,
        NetworkAuthenticationRequired = 511,
    }

    public enum HTTPVersion
    {
        None = 0,
        Unsupported = 1,
        HTTP10 = 2,
        HTTP11 = 3
    }
}
