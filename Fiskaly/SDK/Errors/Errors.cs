using System;

namespace Fiskaly.Errors
{
    public static class ClientError
    {
        public const int HTTP_ERROR = -20_000;
        public const int HTTP_TIMEOUT_ERROR = -21_000;
    }

    public class FiskalyError : Exception
    {
        public FiskalyError(string message) : base(message ) { }
    }

    public class FiskalyClientError : FiskalyError
    {
        public int Code { get; private set; }
        public string Error { get; private set; }

        public FiskalyClientError(int code, string message, string error) : base(message)
        {
            Code = code;
            Error = error;
        }

        public override string ToString()
        {
            return "{\n\tMessage: \"" + Message + "\n\t\",\n\tCode: \"" + Code + "\"\n\t Error: " + Error + "\n}";
        }
    }

    public class FiskalyHttpError : FiskalyError
    {
        public int Status { get; private set; }
        public string Error { get; private set; }
        public string Code { get; private set; }
        public string RequestId { get; private set; }

        public FiskalyHttpError(int status, string error, string message, string code, string requestId) : base(message)
        {
            Status = status;
            Error = error;
            Code = code;
            RequestId = requestId;
        }

        public override string ToString()
        {
            return "{ Status: \"" + Status + "\", Error: \""
                + Error + "\", Code: \"" + Code
                + "\", RequestId: \"" + RequestId + "\" }";
        }
    }

    public class FiskalyHttpTimeoutError : FiskalyError
    {
        public FiskalyHttpTimeoutError(string message) : base(message) { }
    }
}
