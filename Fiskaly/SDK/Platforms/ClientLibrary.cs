using System;
using System.Runtime.InteropServices;

namespace Fiskaly.Client
{
    public enum DebugLevel
    {
        NO_OUTPUT = -1,
        ERRORS_ONLY = 0,
        ERRORS_AND_WARNINGS = 1,
        EVERYTHING = 2
    }

    public class ClientConfiguration
    {
        public DebugLevel DebugLevel { get; set; }
        public string DebugFile { get; set; }
        public int ClientTimeout { get; set; }
        public int SmaersTimeout { get; set; }
        public string HttpProxy { get; set; }
    }

    public abstract class AbstractClient
    {
        protected static bool Is64BitSystem = IntPtr.Size == 8;

        protected const string SYMBOL_INVOKE = "_fiskaly_client_invoke";
        protected const string SYMBOL_FREE = "_fiskaly_client_free";

        protected const string LIB_PREFIX = "com.fiskaly.client";
        protected const string CLIENT_VERSION = Constants.CLIENT_VERSION;

        abstract protected IntPtr PerformInvokeForArchitecure(byte[] request);
        abstract protected void PerformFreeForArchitecture(IntPtr allocatedMemory);

        public string Invoke([In] byte[] request)
        {
            IntPtr resultPtr = PerformInvokeForArchitecure(request);
            string result = Marshal.PtrToStringAnsi(resultPtr);

            PerformFreeForArchitecture(resultPtr);

            return result;
        }
    }
}
