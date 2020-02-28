using System;
using System.Collections.Generic;
using System.Text;

namespace FiskalyClient.Errors
{
    public class FiskalyError : Exception
    {
        public FiskalyError(string message) : base(message) { }
    }

    public class FiskalyClientError : FiskalyError
    {
        public int Code { get; private set; }

        public FiskalyClientError(int code, string message) : base(message)
        {
            this.Code = code;
        }

        public override string ToString()
        {
            return "{ Message: \"" + this.Message + "\", Code: \"" + this.Code + "\" }";
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
            this.Status = status;
            this.Error = error;
            this.Code = code;
            this.RequestId = requestId;
        }

        public override string ToString()
        {
            return "{ Status: \"" + this.Status + "\", Error: \""
                + this.Error + "\", Code: \"" + this.Code
                + "\", RequestId: \"" + this.RequestId + "\" }";
        }
    }
}
