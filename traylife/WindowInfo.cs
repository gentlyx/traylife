/*
 * 由SharpDevelop创建。
 * 用户： GentlyXu
 * 日期: 2018/6/29
 * 时间: 10:35
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;

namespace traylife
{
	/// <summary>
	/// Description of WindowInfo.
	/// </summary>
	public class WindowInfo
	{
		public WindowInfo(string title, IntPtr handle)
		{
			this._title = title;
            this._handle = handle;
		}
		
         private string _title;
         private IntPtr _handle;

		 public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public IntPtr Handle
        {
            get { return _handle; }
            set { _handle = value; }
        }
        public override string ToString()
        {
            return _handle.ToString() + ":" + Title;
        }
	}
}
