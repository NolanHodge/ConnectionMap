using System;
using Gtk;

namespace ConnectionMap
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			MainControl win = new MainControl();
			win.DeleteEvent += delegate { Application.Quit(); };
			win.Title = "Connection Map";
			Application.Run ();
		}
	}
}
