using System;
using System.Runtime.InteropServices;

namespace Fiskaly.Client
{
    public class LinuxClient : AbstractClient
    {
        private const string LIB_64 = "com.fiskaly.kassensichv.client-linux-amd64.so";
        private const string LIB_32 = "com.fiskaly.kassensichv.client-linux-386.so";

        [DllImport(LIB_32, EntryPoint = SYMBOL_FREE)]
        internal static extern void Free32(IntPtr response);

        [DllImport(LIB_64, EntryPoint = SYMBOL_FREE)]
        internal static extern void Free64(IntPtr response);

        [DllImport(LIB_32, EntryPoint = SYMBOL_INVOKE)]
        internal static extern IntPtr Invoke32([In] byte[] request);

        [DllImport(LIB_64, EntryPoint = SYMBOL_INVOKE)]
        internal static extern IntPtr Invoke64([In] byte[] request);

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

        protected override IntPtr PerformInvokeForArchitecure(byte[] request)
        {
            if (Is64BitSystem)
            {
                return Invoke64(request);
            }

            return Invoke32(request);
        }
    }
}