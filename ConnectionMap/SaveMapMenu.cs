using System;

namespace ConnectionMap
{
	public partial class SaveMapMenu : Gtk.Window
	{
		public string filename = "";

		public SaveMapMenu () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

		protected void on_save (object sender, EventArgs e)
		{
			filename = textview2.Buffer.Text;
			Hide ();
		}
	}
}

