using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MD_CardInfo
{

    internal class Func
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenProcess(
 uint processAccess,
 bool bInheritHandle,
 int processId
);
        [DllImport("kernel32.dll")]
        static extern bool ReadProcessMemory(
    IntPtr hProcess,
    IntPtr lpBaseAddress,
    [Out] byte[] lpBuffer,
    int dwSize,
    out IntPtr lpNumberOfBytesRead);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);
        static readonly uint PROCESS_ALL_ACCESS = 0x001f0fff;


        public static IntPtr GameAssembly;
        public static Process? gProcess=null;
        public static IntPtr pHandle;
        public static int[] addresses = new int[] { 0x1CB7A40, 0xb8, 0, 0x18, 0x20, 0x34 };
        public static void GetMDProcess()
        {
            foreach (var p in Process.GetProcessesByName("masterduel"))
                gProcess = p;
        }
        public static void GetGameAssembly()
        {
            if (gProcess == null) return;
            foreach (ProcessModule pm in gProcess.Modules)
            {
                if (pm.ModuleName == "GameAssembly.dll")
                {
                    GameAssembly = pm.BaseAddress;
                    break;
                }
                  
            }
        }
        public static void OpenMDProcess()
        {
            if (gProcess == null) return;
            pHandle = OpenProcess(PROCESS_ALL_ACCESS, false, gProcess.Id);
        }
        public static void CloseMDProcess()
        {
            if (pHandle == IntPtr.Zero) return;
            CloseHandle(pHandle);
        }
        public static int? GetCardID()
        {
            if (gProcess is null || gProcess.HasExited || pHandle == IntPtr.Zero || GameAssembly == IntPtr.Zero) return null;
            IntPtr addr = GameAssembly;
            byte[] buffer = new byte[8];
            for (int i = 0; i < addresses.Length; i++)
            {
                var ret = ReadProcessMemory(pHandle, addr + addresses[i], buffer, buffer.Length, out _);
                if (!ret) return null;
                if (i == addresses.Length - 1) 
                {
                    return BitConverter.ToInt32(buffer);
                }
                else
                {
                    addr = new(BitConverter.ToInt64(buffer));
                }
            }
            return -1;
        }
    }
}
