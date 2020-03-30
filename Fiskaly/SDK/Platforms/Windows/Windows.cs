using System;
using System.Runtime.InteropServices;

namespace Fiskaly.Client
{
    class WindowsClient : AbstractClient
    {
        private const string PLATFORM = "windows";
        private const string EXTENSION = ".dll";

        private const string LIB_64 = LIB_PREFIX + "-" + PLATFORM + "-amd64-" + CLIENT_VERSION + EXTENSION;
        private const string LIB_32 = LIB_PREFIX + "-" + PLATFORM + "-386-" + CLIENT_VERSION + EXTENSION;

        [DllImport(LIB_32, EntryPoint = SYMBOL_FREE)]
        internal static extern void Free32(IntPtr response);

        [DllImport(LIB_64, EntryPoint = SYMBOL_FREE)]
        internal static extern void Free64(IntPtr response);

        [DllImport(LIB_32, EntryPoint = SYMBOL_INVOKE)]
        internal static extern IntPtr Invoke32([In] byte[] request);

        [DllImport(LIB_64, EntryPoint = SYMBOL_INVOKE)]
        internal static extern IntPtr Invoke64([In] byte[] request);

        protected override IntPtr PerformInvokeForArchitecure(byte[] request)
        {
            if (Is64BitSystem)
            {
                return Invoke64(request);
            }

            return Invoke32(request);
        }

        protected override void PerformFreeForArchitecture(IntPtr allocatedMemory)
        {
            if (Is64BitSystem)
            {
                Free64(allocatedMemory);
            } else
            {
                Free32(allocatedMemory);
            }
        }
    }
}