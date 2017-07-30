using System;

namespace ConnectionMap
{
	public partial class CustomMapWindow : Gtk.Window
	{
		public CustomMapWindow () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}
	}
}

