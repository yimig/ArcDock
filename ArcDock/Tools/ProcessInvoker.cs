using ArcDock.Data.Json;
using Microsoft.Scripting.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static ArcDock.App;
using static IronPython.Modules._ast;

namespace ArcDock.Tools
{
    public class ProcessInvoker
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        public const int WM_COPYDATA = 0x004A;

        public static ProcessData Data { get; set; } = new ProcessData() { IsHandled = true };

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        public static void LoadStartupArgs(string[] args)
        {
            Data = new ProcessData();
            Data.IsAssociate = args.Contains("--associate");
            Data.IsSilent = args.Contains("--silent");
            var temp_index = args.FindIndex(i => i == "--template");
            Data.TemplateName = temp_index == -1 ? "" : args[temp_index + 1];
            var args_index = args.FindIndex(i => i == "--args");
            Data.Arguments = JsonConvert.DeserializeObject<Dictionary<string, string>>(args[args_index + 1]);
            Data.IsHandled = false;
        }


        public static COPYDATASTRUCT GetDataStruct(string message)
        {
            // 向已有进程发送参数
            byte[] sarr = Encoding.UTF8.GetBytes(message);
            IntPtr pAgr = Marshal.AllocCoTaskMem(sarr.Length);
            Marshal.Copy(sarr,0, pAgr, sarr.Length);
            COPYDATASTRUCT cds;
            cds.dwData = IntPtr.Zero;
            cds.lpData = pAgr;
            cds.cbData = sarr.Length;
            return cds;
        }

        public static string GetDataString(COPYDATASTRUCT copyDataStruct)
        {
            var bytes = new byte[copyDataStruct.cbData];
            Marshal.Copy(copyDataStruct.lpData, bytes, 0, bytes.Length);
            //Marshal.FreeCoTaskMem(copyDataStruct.lpData);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
