using System;

namespace ConnectionMap
{
	public partial class CustomMapPopup : Gtk.Window
	{
		public String countryColor = "";
		public String waterColor = "";
		public String countryLabelColor = "";
		public String waterLabelColor = "";
		public bool save = false;

		public CustomMapPopup () :
			base (Gtk.WindowType.Toplevel)
		{
			this.Build ();
		}

		protected void cancel_click (object sender, EventArgs e)
		{
			Destroy ();
		}

		protected void done_click (object sender, EventArgs e)
		{
			countryColor = "" + colorselection24.CurrentColor.Red.ToString("X2")[0] + colorselection24.CurrentColor.Red.ToString("X2")[1] +
				colorselection24.CurrentColor.Green.ToString("X2")[0] + colorselection24.CurrentColor.Green.ToString("X2")[1] +
				colorselection24.CurrentColor.Blue.ToString("X2")[0] + colorselection24.CurrentColor.Blue.ToString("X2")[1];

			waterColor = "" + colorselection25.CurrentColor.Red.ToString("X2")[0] + colorselection25.CurrentColor.Red.ToString("X2")[1] +
				colorselection25.CurrentColor.Green.ToString("X2")[0] + colorselection25.CurrentColor.Green.ToString("X2")[1] +
				colorselection25.CurrentColor.Blue.ToString("X2")[0] + colorselection25.CurrentColor.Blue.ToString("X2")[1];

			countryLabelColor = "" + colorselection26.CurrentColor.Red.ToString("X2")[0] + colorselection26.CurrentColor.Red.ToString("X2")[1] +
				colorselection26.CurrentColor.Green.ToString("X2")[0] + colorselection26.CurrentColor.Green.ToString("X2")[1] +
				colorselection26.CurrentColor.Blue.ToString("X2")[0] + colorselection26.CurrentColor.Blue.ToString("X2")[1];
			
			waterLabelColor = "" + colorselection27.CurrentColor.Red.ToString("X2")[0] + colorselection27.CurrentColor.Red.ToString("X2")[1] +
				colorselection27.CurrentColor.Green.ToString("X2")[0] + colorselection27.CurrentColor.Green.ToString("X2")[1] +
				colorselection27.CurrentColor.Blue.ToString("X2")[0] + colorselection27.CurrentColor.Blue.ToString("X2")[1];
			
			Hide ();
		}

		protected void save_toggled (object sender, EventArgs e)
		{
			save = save_box.Active;
		}
	}
}

