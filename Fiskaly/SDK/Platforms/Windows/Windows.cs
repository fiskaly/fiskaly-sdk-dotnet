using System;
using System.Runtime.InteropServices;

namespace Fiskaly.Client
{

    public class WindowsClient : AbstractClient
    {
        private const string LIB_64 = "com.fiskaly.kassensichv.client-windows-amd64.dll";
        private const string LIB_32 = "com.fiskaly.kassensichv.client-windows-386.dll";

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