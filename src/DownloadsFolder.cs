using Syroot.Windows.IO;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Auto_Download_Mover
{
    // https://assist-software.net/snippets/how-get-path-windows-special-folders-windows-service-net
    internal static class DownloadsFolder
    {
        internal static string Path()
        {
            var windows_identity = WindowsIdentityFromProcess(Process.GetProcessesByName("explorer").FirstOrDefault());
            return windows_identity != null ? new KnownFolder(KnownFolderType.Downloads, windows_identity).Path : KnownFolders.PublicDownloads.Path;
        }

        private static WindowsIdentity WindowsIdentityFromProcess(Process process)
        {
            var process_handle = IntPtr.Zero;
            try
            {
                OpenProcessToken(process.Handle, 8, out process_handle);
                return new WindowsIdentity(process_handle);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (process_handle != IntPtr.Zero)
                {
                    CloseHandle(process_handle);
                }
            }
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr process_handle, uint desired_access, out IntPtr token_handle);
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr h_object);
    }
}
