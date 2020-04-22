using System;
using System.Runtime.InteropServices;

namespace Fiskaly.Client
{
    public class AndroidClient : AbstractClient
    {
        private const string EXTENSION = ".so";
        private const string LIB = "lib" + LIB_PREFIX + "-" + CLIENT_VERSION + EXTENSION;

        [DllImport(LIB, EntryPoint = SYMBOL_FREE)]
        internal static extern void Free(IntPtr response);

        [DllImport(LIB, EntryPoint = SYMBOL_INVOKE)]
        internal static extern IntPtr InvokeLib([In] byte[] request);

        protected override void PerformFreeForArchitecture(IntPtr allocatedMemory)
        {
            Free(allocatedMemory);
        }

        protected override IntPtr PerformInvokeForArchitecure(byte[] request)
        {
            return InvokeLib(request);
        }
    }
}