using ArcDock.Tools;
using log4net;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using static ArcDock.Tools.ProcessInvoker;

namespace ArcDock
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 当前页面的日志控制对象
        /// </summary>
        private static ILog log = LogManager.GetLogger("App");

        /// <summary>
        /// 程序初始化的事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // 获取当前进程的名称
            string processName = Process.GetCurrentProcess().ProcessName;

            // 查找是否已有同名进程在运行
            Process[] processes = Process.GetProcessesByName(processName);
            log.Debug("process name:" + processName + ";count:" + processes.Length);
            log.Debug("args:【" + String.Join("|", e.Args + "】"));
            if (processes.Length > 1)  // 已有同名进程在运行
            {
                // 将窗口激活到前台
                IntPtr hWnd = ProcessInvoker.FindWindow(null, processName);
                ProcessInvoker.SetForegroundWindow(hWnd);
                if (e.Args.Contains("--args")) ProcessInvoker.LoadStartupArgs(e.Args);
                COPYDATASTRUCT cds = GetDataStruct(JsonConvert.SerializeObject(ProcessInvoker.Data));
                ProcessInvoker.SendMessage(hWnd, ProcessInvoker.WM_COPYDATA, 0, ref cds);
                Environment.Exit(0);
            }
            else
            {
                if (e.Args.Contains("--args")) ProcessInvoker.LoadStartupArgs(e.Args);
            }

        }


    }
}
