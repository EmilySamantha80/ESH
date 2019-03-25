// Title:  Windows user impersonation
// Author: Emily Heiner
//
// MIT License
// Copyright(c) 2019 Emily Heiner (emilysamantha80@gmail.com)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Principal;
using Microsoft.Win32.SafeHandles;
using System.Security;
using System.Security.Permissions;

namespace ESH.Utility
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public sealed class Impersonation : IDisposable
    {
        public enum LogonType
        {
            Interactive = 2,
            Network = 3,
            Batch = 4,
            Service = 5,
            Unlock = 7,
            NetworkCleartext = 8,
            NewCredentials = 9,
        }

        private readonly SafeTokenHandle _handle;
        private readonly WindowsImpersonationContext _context;

        public static Impersonation LogonUser(string domain, string username, string password, LogonType logonType)
        {
            return new Impersonation(domain, username, password, logonType);
        }

        public static Impersonation LogonUser(string domain, string username, SecureString password, LogonType logonType)
        {
            return new Impersonation(domain, username, password, logonType);
        }

        private Impersonation(string domain, string username, SecureString password, LogonType logonType)
        {
            IntPtr token;
            IntPtr passPtr = Marshal.SecureStringToGlobalAllocUnicode(password);

            bool success;
            try
            {
                success = NativeMethods.LogonUser(username, domain, passPtr, (int)logonType, 0, out token);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(passPtr);
            }

            CompleteImpersonation(success, token, out _handle, out _context);
        }

        public Impersonation(string domain, string username, string password, LogonType logonType)
        {
            IntPtr token;
            bool success = NativeMethods.LogonUser(username, domain, password, (int)logonType, 0, out token);
            CompleteImpersonation(success, token, out _handle, out _context);
        }

        private void CompleteImpersonation(bool success, IntPtr token, out SafeTokenHandle handle, out WindowsImpersonationContext context)
        {
            if (!success)
            {
                var errorCode = Marshal.GetLastWin32Error();

                if (token != IntPtr.Zero)
                {
                    NativeMethods.CloseHandle(token);
                }

                throw new ApplicationException(string.Format("Could not impersonate the elevated user.  LogonUser returned error code {0}.", errorCode));
            }

            handle = new SafeTokenHandle(token);
            context = WindowsIdentity.Impersonate(_handle.DangerousGetHandle());
        }

        /// <summary>
        /// Disposes the <see cref="Impersonation"/> object, ending impersonation and restoring the original user.
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            _handle.Dispose();
        }
    }

    internal class NativeMethods
    {
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern bool LogonUser(String lpszUsername, String lpszDomain, IntPtr phPassword, int dwLogonType, int dwLogonProvider, out IntPtr phToken);

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CloseHandle(IntPtr handle);
    }

    internal sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal SafeTokenHandle(IntPtr handle)
            : base(true)
        {
            this.handle = handle;
        }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.CloseHandle(handle);
        }
    }


}
