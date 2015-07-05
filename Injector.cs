/*   MSEA-WZSearcher-HackTool - A handy tool for MapleStory packet editing
    Copyright (C) 2012~2015 eaxvac/lastBattle https://github.com/eaxvac

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MSEAHackUtility
{
    public static class Injector
    {
        private static class WINAPI
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr OpenProcess(
            UInt32 dwDesiredAccess,
            Int32 bInheritHandle,
            UInt32 dwProcessId);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern Int32 CloseHandle(
            IntPtr hObject);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetProcAddress(
            IntPtr hModule,
            string lpProcName);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr GetModuleHandle(
            string lpModuleName);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr VirtualAllocEx(
            IntPtr hProcess,
            IntPtr lpAddress,
            IntPtr dwSize,
            uint flAllocationType,
            uint flProtect);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern Int32 WriteProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            byte[] buffer,
            uint size,
            out IntPtr lpNumberOfBytesWritten);
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr CreateRemoteThread(
            IntPtr hProcess,
            IntPtr lpThreadAttribute,
            IntPtr dwStackSize,
            IntPtr lpStartAddress,
            IntPtr lpParameter,
            uint dwCreationFlags,
            IntPtr lpThreadId);
            public static class VAE_Enums
            {
                public enum AllocationType
                {
                    MEM_COMMIT = 0x1000,
                    MEM_RESERVE = 0x2000,
                    MEM_RESET = 0x80000,
                }
                public enum ProtectionConstants
                {
                    PAGE_EXECUTE = 0X10,
                    PAGE_EXECUTE_READ = 0X20,
                    PAGE_EXECUTE_READWRITE = 0X40,
                    PAGE_EXECUTE_WRITECOPY = 0X80,
                    PAGE_NOACCESS = 0X01
                }
            }
        }
        public static bool DoInject(
            Process pToBeInjected,
            string sDllPath,
            string sDLLPath2,
            out string sError)
        {
            IntPtr hwnd = IntPtr.Zero;
            if (!CRT(pToBeInjected, sDllPath, out sError, out hwnd)) //CreateRemoteThread 
            {
                //close the handle, since the method wasn't able to get to that 
                if (hwnd != (IntPtr)0)
                    WINAPI.CloseHandle(hwnd);
                return false;
            }
            int wee = Marshal.GetLastWin32Error();

            if (sDLLPath2 != null && sDLLPath2 != String.Empty)
            {
                if (!CRT(pToBeInjected, sDLLPath2, out sError, out hwnd)) //CreateRemoteThread 
                {
                    //close the handle, since the method wasn't able to get to that 
                    if (hwnd != (IntPtr)0)
                        WINAPI.CloseHandle(hwnd);
                    return false;
                }
                int wee2 = Marshal.GetLastWin32Error();
            }
            return true;
        }
        private static bool CRT(
            Process pToBeInjected,
            string sDllPath,
            out string sError,
            out IntPtr hwnd)
        {
            sError = String.Empty; //in case we encounter no errors 
            IntPtr hndProc = WINAPI.OpenProcess(
                (0x2 | 0x8 | 0x10 | 0x20 | 0x400), //create thread, query info, operation, write, and read 
                1,
                (uint)pToBeInjected.Id);
            hwnd = hndProc;
            if (hndProc == (IntPtr)0)
            {
                sError = "Unable to attatch to process.n";
                sError += "Error code: " + Marshal.GetLastWin32Error();
                return false;
            }
            IntPtr lpLLAddress = WINAPI.GetProcAddress(
                WINAPI.GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (lpLLAddress == (IntPtr)0)
            {
                sError = "Unable to find address of LoadLibraryA";
                sError += "Error code: " + Marshal.GetLastWin32Error();
                return false;
            }
            IntPtr lpAddress = WINAPI.VirtualAllocEx(
                hndProc,
                (IntPtr)null,
                (IntPtr)sDllPath.Length, //520 bytes should be enough 
                (uint)WINAPI.VAE_Enums.AllocationType.MEM_COMMIT |
                (uint)WINAPI.VAE_Enums.AllocationType.MEM_RESERVE,
                (uint)WINAPI.VAE_Enums.ProtectionConstants.PAGE_EXECUTE_READWRITE);
            if (lpAddress == (IntPtr)0)
            {
                if (lpAddress == (IntPtr)0)
                {
                    sError = "Unable to allocate memory to target process.n";
                    sError += "Error code: " + Marshal.GetLastWin32Error();
                    return false;
                }
            }
            byte[] bytes = CalcBytes(sDllPath);
            IntPtr ipTmp = IntPtr.Zero;
            WINAPI.WriteProcessMemory(
                hndProc,
                lpAddress,
                bytes,
                (uint)bytes.Length,
                out ipTmp);
            if (Marshal.GetLastWin32Error() != 0)
            {
                sError = "Unable to write memory to process.";
                sError += "Error code: " + Marshal.GetLastWin32Error();
                return false;
            }
            IntPtr ipThread = WINAPI.CreateRemoteThread(
                hndProc,
                (IntPtr)null,
                (IntPtr)0,
                lpLLAddress,
                lpAddress,
                0,
                (IntPtr)null);
            if (ipThread == (IntPtr)0)
            {
                sError = "Unable to load dll into memory.";
                sError += "Error code: " + Marshal.GetLastWin32Error();
                return false;
            }
            return true;
        }

        private static byte[] CalcBytes(string sToConvert)
        {
            byte[] bRet = System.Text.Encoding.ASCII.GetBytes(sToConvert);
            return bRet;
        }
    }
}

