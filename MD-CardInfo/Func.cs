using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(
     IntPtr hProcess,
     IntPtr lpBaseAddress,
     byte[] lpBuffer,
     Int32 nSize,
     out IntPtr lpNumberOfBytesWritten);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);
        static readonly uint PROCESS_ALL_ACCESS = 0x001f0fff;

        public static IntPtr GameAssembly;
        public static Process? gProcess=null;
        public static IntPtr pHandle;
        public static int[] CheckCardIndex = new int[] { 0x01E8A1F0, 0xb8, 0, 0x40, 0x10, 0x20, 0x110, 0x34 }; //检查卡片的序号
        public static int[] CheckCardArray = new int[] { 0x01E8A1F0, 0xb8, 0, 0x40, 0x10, 0x20, 0x20, 0x18, 0x48, 0x10, 0x20 }; //检查卡片的数组

        public static int[] DuelCID = new int[] { 0x01E7C600, 0xb8, 0, 0x4c }; //决斗界面
        public static int[] EditCID = new int[] { 0x01E99C18, 0xb8, 0, 0xF8,0x1E0,0x2c }; //编辑界面
        public static int[] MainDeckCount = new int[] { 0x01E99C18, 0xb8, 0, 0xF8, 0x1C8, 0x148, 0x18 };
        public static int[] MainDeck = new int[] { 0x01E99C18, 0xb8, 0, 0xF8, 0x1C8, 0x148, 0x10, 0x20 };
        public static int[] ExDeckCount = new int[] { 0x01E99C18, 0xb8, 0, 0xF8, 0x1C8, 0x150, 0x18 };
        public static int[] ExDeck = new int[] { 0x01E99C18, 0xb8, 0, 0xF8, 0x1C8, 0x150, 0x10, 0x20 };
        public static int CardSize = 0x18;  //一张卡在内存中的大小;
        public static void GetMDProcess()
        {
            gProcess = null;
            foreach (var p in Process.GetProcessesByName("masterduel"))
            {
                gProcess = p;
                break;
            }
        }
        public static void GetGameAssembly()
        {
            if (gProcess == null) return;
            gProcess.Refresh();
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
            pHandle = IntPtr.Zero;
            gProcess = null;
        }
        public static int? ReadInt(int[] addrs)
        {
            if (gProcess is null || gProcess.HasExited || pHandle == IntPtr.Zero || GameAssembly == IntPtr.Zero) return null;
            IntPtr addr = GameAssembly;
            byte[] buffer = new byte[8];
            for (int i = 0; i < addrs.Length; i++)
            {
                var ret = ReadProcessMemory(pHandle, addr + addrs[i], buffer, buffer.Length, out _);
                if (!ret) return null;
                if (i == addrs.Length - 1)
                    return BitConverter.ToInt32(buffer);
                else
                    addr = new(BitConverter.ToInt64(buffer));
            }
            return -1;
        }
        public static int? WriteInt(int[] addrs, int value)
        {
            if (gProcess is null || gProcess.HasExited || pHandle == IntPtr.Zero || GameAssembly == IntPtr.Zero) return null;
            IntPtr addr = GameAssembly;
            byte[] buffer = new byte[8];
            for (int i = 0; i < addrs.Length; i++)
            {
                if (i == addrs.Length - 1)
                {
                    buffer = BitConverter.GetBytes(value);
                    WriteProcessMemory(pHandle, addr + addrs[i], buffer, buffer.Length, out _);
                }
                else
                {
                    var ret = ReadProcessMemory(pHandle, addr + addrs[i], buffer, buffer.Length, out _);
                    if (!ret) return null;
                    addr = new(BitConverter.ToInt64(buffer));
                }
               
            }
            return -1;
        }

        public static int? GetCardCID()
        {
            var id = ReadInt(DuelCID);
            if (id != null) return id;

            var index = ReadInt(CheckCardIndex);
            if (index != null)
            {
                var addrs = (int[])CheckCardArray.Clone();
                addrs[addrs.Length - 1] = (int)(CheckCardArray[CheckCardArray.Length - 1] + index * 4);
                id = ReadInt(addrs);
                return id;
            }

            id = ReadInt(EditCID);
            if (id != null) return id;
   

            return null;
        }
        public static List<int>? GetMainDeck()
        {
            List<int> deck = new();
            var count = ReadInt(MainDeckCount);
            if (count is null) return null;
            int[] addrs = (int[])MainDeck.Clone();
            for (int i = 0; i < count; i++)
            {
                addrs[addrs.Length - 1] = MainDeck[MainDeck.Length - 1] + i * CardSize;
                var cardId = ReadInt(addrs);
                if (cardId != null && cardId > 4000 && cardId < 50000) 
                {
                    deck.Add(cardId.Value);
                }
            }
            return deck;
        }
        public static List<int>? GetExDeck()
        {
            List<int> deck = new();
            var count = ReadInt(ExDeckCount);
            if (count is null) return null;
            int[] addrs = (int[])ExDeck.Clone();
            for (int i = 0; i < count; i++)
            {
                var cardId = ReadInt(addrs);
                addrs[addrs.Length - 1] += CardSize;
                if (cardId != null && cardId > 4000 && cardId < 50000)
                    deck.Add(cardId.Value);
            }
            return deck;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deck"></param>
        /// <returns>-2:write error -1:count error; 0: success</returns>
        public static int WriteDeck(List<int> mainDeck, List<int> exDeck)
        {
             var count = ReadInt(MainDeckCount);
            if (count == null) return -2;
            if (mainDeck.Count != count) return -1;
            count = ReadInt(ExDeckCount);
            if (exDeck.Count != count) return -1;
                                     
            int[] addrs = (int[])MainDeck.Clone();
            for (int i = 0; i < mainDeck.Count; i++)
            {
                WriteInt(addrs,mainDeck[i]);
                addrs[addrs.Length - 1] += CardSize;
            }
            //额外
            addrs = (int[])ExDeck.Clone();
            for (int i = 0; i < exDeck.Count; i++)
            {
                WriteInt(addrs, exDeck[i]);
                addrs[addrs.Length - 1] += CardSize;
            }
            return 0;
        }
    }
}
