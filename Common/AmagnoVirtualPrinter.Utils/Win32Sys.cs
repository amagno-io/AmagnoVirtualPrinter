using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

using JetBrains.Annotations;

using Microsoft.Win32.SafeHandles;

namespace AmagnoVirtualPrinter.Utils
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal static class Win32Sys
    {
        /// <summary>
        /// Creates process with given command line via Win32 API.
        /// Use this method to start a process as a service.
        /// </summary>
        /// <param name="sessId"></param>
        /// <param name="user"></param>
        /// <param name="commandLine"></param>
        internal static void CreateProcessAsUser(int sessId, string user, string commandLine)
        {
            WTSQueryUserToken((uint) sessId, out var userToken);
            if (userToken.IsInvalid)
            {
                throw new InvalidOperationException($"Could not query user token for session {sessId}! (" + GetLastError() + ")",
                    Windows.LastError);
            }

            CreateProcessAsUser(userToken, user, commandLine);
        }

        private static void CreateProcessAsUser([NotNull]SafeTokenHandle token, string user, string commandLine)
        {
            var processInformation = new PROCESS_INFORMATION();
            try
            {
                var securityAttributes = new SECURITY_ATTRIBUTES();
                securityAttributes.Length = Marshal.SizeOf(securityAttributes);

                var startupInfo = new STARTUPINFO();
                startupInfo.cb = Marshal.SizeOf(startupInfo);
                startupInfo.lpDesktop = "winsta0\\default";

                var info = new ProfileInfo();
                info.dwSize = Marshal.SizeOf(info);
                info.lpUserName = user;
                info.dwFlags = 1;

                var result = LoadUserProfile(token, ref info);
                if (!result)
                {
                    throw new UserProfileException($"Can not load user profile for User: {user} and command line: {commandLine}.", Windows.LastError);
                }

                result = CreateEnvironmentBlock(out var lpEnvironment, token, false);
                if (!result)
                {
                    throw new UserProfileException($"Can not create environment block for User: {user} and command line: {commandLine}.", Windows.LastError);
                }

                result = CreateProcessAsUser
                (
                    token,
                    null,
                    commandLine,
                    ref securityAttributes,
                    ref securityAttributes,
                    false,
                    0x00000400,
                    lpEnvironment,
                    null,
                    ref startupInfo,
                    ref processInformation
                );

                if (!result)
                {
                    throw new UserProfileException($"Can not create process as user for User: {user} and command line: {commandLine}.", Windows.LastError);
                }
            }
            finally
            {
                if (processInformation.hProcess != IntPtr.Zero)
                {
                    CloseMyHandle(processInformation.hProcess);
                }

                if (processInformation.hThread != IntPtr.Zero)
                {
                    CloseMyHandle(processInformation.hThread);
                }

                if (!token.IsInvalid)
                {
                    token.Dispose();
                }
            }
        }

        #region Classes

        public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            private SafeTokenHandle() : base(true)
            {
            }

            [DllImport("kernel32.dll")]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            [SuppressUnmanagedCodeSecurity]
            [return: MarshalAs(UnmanagedType.Bool)]
            private static extern bool CloseHandle(IntPtr handle);

            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }

            public override string ToString()
            {
                return $"{handle}";
            }
        }

        #endregion

        #region Structures

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessID;
            public int dwThreadID;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ProfileInfo
        {
            public int dwSize;
            public int dwFlags;
            public string lpUserName;
            public string lpProfilePath;
            public string lpDefaultPath;
            public string lpServerName;
            public string lpPolicyPath;
            public IntPtr hProfile;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int Length;
            public IntPtr lpSecurityDescriptor;
            public bool bInheritHandle;
        }

        #endregion

        #region Windows interop

        [DllImport("userenv.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool LoadUserProfile(SafeTokenHandle hToken, ref ProfileInfo lpProfileInfo);

        [DllImport("userenv.dll", SetLastError = true)]
        private static extern bool CreateEnvironmentBlock(out IntPtr lpEnvironment,
            SafeTokenHandle hToken, bool bInherit);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true,
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool CloseMyHandle(IntPtr handle);

        [DllImport("advapi32.dll", EntryPoint = "CreateProcessAsUser", SetLastError = true,
            CharSet = CharSet.Ansi, CallingConvention = CallingConvention.StdCall)]
        private static extern bool CreateProcessAsUser(SafeTokenHandle hToken,
            string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle,
            int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory,
            ref STARTUPINFO lpStartupInfo, ref PROCESS_INFORMATION lpProcessInformation);

        [DllImport("wtsapi32.dll", SetLastError = true)]
        private static extern bool WTSQueryUserToken(uint sessionId, out SafeTokenHandle Token);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        [DllImport("kernel32")]
        public static extern int GetPrivateProfileString
        (
            string section,
            string key,
            string def,
            StringBuilder retVal,
            int size,
            string filePath
        );

        [DllImport("kernel32")]
        public static extern long WritePrivateProfileString
        (
            string section,
            string key,
            string val,
            string filePath
        );

        #endregion
    }
}