using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

[assembly: InternalsVisibleTo("FiskalyClientDemo")]
namespace Fiskaly.Client
{
    internal static class ClientLibrary
    {
        private static bool Is64BitSystem = IntPtr.Size == 8;

        private const string SYMBOL_INVOKE = "_fiskaly_client_invoke";
        private const string SYMBOL_FREE = "_fiskaly_client_free";

        private const string LIB_64 = "com.fiskaly.kassensichv.client-windows-amd64.dll";
        private const string LIB_32 = "com.fiskaly.kassensichv.client-windows-386.dll";

        [DllImport(LIB_32, EntryPoint = SYMBOL_INVOKE)]
        internal static extern IntPtr Invoke_32([In] byte[] request);

        [DllImport(LIB_32, EntryPoint = SYMBOL_FREE)]
        internal static extern void Free_32(IntPtr response);

        [DllImport(LIB_64, EntryPoint = SYMBOL_INVOKE)]
        internal static extern IntPtr Invoke_64([In] byte[] request);

        [DllImport(LIB_64, EntryPoint = SYMBOL_FREE)]
        internal static extern void Free_64(IntPtr response);

        internal static void Free(IntPtr response)
        {
            if (Is64BitSystem)
            {
                Free_64(response);
            }
            else
            {
                Free_32(response);
            }
        }

        internal static string Invoke([In] byte[] request)
        {
            IntPtr resultPtr = Is64BitSystem ? Invoke_64(request) : Invoke_32(request);
            string result = Marshal.PtrToStringAnsi(resultPtr);

            Free(resultPtr);

            return result;
        }

    }
}
