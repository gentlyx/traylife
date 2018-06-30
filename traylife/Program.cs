/*
 * 由SharpDevelop创建。
 * 用户： GentlyXu
 * 日期: 2018/6/29
 * 时间: 10:32
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Collections.Generic;

namespace traylife
{
	class Program
	{
		//获取托盘指针 
        private static IntPtr TrayToolbarWindow32()
        {
            IntPtr h = IntPtr.Zero;
            IntPtr hTemp = IntPtr.Zero;

            h = W32API.FindWindow("Shell_TrayWnd", null); //托盘容器 
            h = W32API.FindWindowEx(h, IntPtr.Zero, "TrayNotifyWnd", null);//找到托盘 
            h = W32API.FindWindowEx(h, IntPtr.Zero, "SysPager", null);

            hTemp = W32API.FindWindowEx(h, IntPtr.Zero, "ToolbarWindow32", null);

            return hTemp;
        }
		
		//获取托盘图标列表 
        public static List<WindowInfo> GetIconList()
        {
            var iconList = new List<WindowInfo>();

            IntPtr pid = IntPtr.Zero;
            IntPtr ipHandle = IntPtr.Zero; //图标句柄 
            IntPtr lTextAdr = IntPtr.Zero; //文本内存地址 

            IntPtr ipTray = TrayToolbarWindow32();

            W32API.GetWindowThreadProcessId(ipTray, ref pid);
            if (pid.Equals(0)) return iconList;

            IntPtr hProcess = W32API.OpenProcess(W32API.PROCESS_ALL_ACCESS | W32API.PROCESS_VM_OPERATION | W32API.PROCESS_VM_READ | W32API.PROCESS_VM_WRITE, IntPtr.Zero, pid);
            IntPtr lAddress = W32API.VirtualAllocEx(hProcess, 0, 4096, W32API.MEM_COMMIT, W32API.PAGE_READWRITE);

            //得到图标个数 
            int lButton = W32API.SendMessage(ipTray, W32API.TB_BUTTONCOUNT, 0, 0);
            
            Console.WriteLine("图标个数：{0}",lButton);
            
            for (int i = 0; i < lButton; i++)
            {
                int bb = W32API.SendMessage(ipTray, W32API.TB_GETBUTTON, i, lAddress);

                //W32API.SendMessage(ipTray, W32API.TB_GETBUTTONINFO, i, lAddress);

                //读文本地址 
                W32API.ReadProcessMemory(hProcess, (IntPtr)(lAddress.ToInt32() + 24), ref lTextAdr, 4, 0);

                if (!lTextAdr.Equals(-1))
                {
                    var buff = new byte[1024];

                    W32API.ReadProcessMemory(hProcess, lTextAdr, buff, 1024, 0);//读文本 
                    string title = System.Text.Encoding.Unicode.GetString(buff);

                    // 从字符0处截断 
                    int nullindex = title.IndexOf("\0", StringComparison.Ordinal);
                    if (nullindex > 0)
                    {
                        title = title.Substring(0, nullindex);
                    }

                    IntPtr ipHandleAdr = IntPtr.Zero;

                    //读句柄地址 
//                    W32API.ReadProcessMemory(hProcess, (IntPtr)(lAddress.ToInt32() + 16), ref ipHandleAdr, 4, 0);
//                    W32API.ReadProcessMemory(hProcess, ipHandleAdr, ref ipHandle, 4, 0);

                    if (title.Replace("\0", "") == "") continue;//不加载空项
                    iconList.Add(new WindowInfo(title, ipHandleAdr));
                }
            }
            W32API.VirtualFreeEx(hProcess, lAddress, 4096, W32API.MEM_RELEASE);
            W32API.CloseHandle(hProcess);

            return iconList;
        }
		
		public static void Main(string[] args)
		{
			
			
			// TODO: Implement Functionality Here
			
			Console.WriteLine("找到托盘句柄：{0:D}",TrayToolbarWindow32());
			
			var iconList = GetIconList();
			foreach(WindowInfo info in iconList){
				Console.WriteLine("h:{0},t:{1}",info.Handle, info.Title);
			}
			
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
		}
	}
}