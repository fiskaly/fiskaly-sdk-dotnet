using System;
using System.Runtime.InteropServices;

namespace Fiskaly.Client
{
    public abstract class AbstractClient
    {
        protected static bool Is64BitSystem = IntPtr.Size == 8;
        protected const string SYMBOL_INVOKE = "_fiskaly_client_invoke";
        protected const string SYMBOL_FREE = "_fiskaly_client_free";

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
