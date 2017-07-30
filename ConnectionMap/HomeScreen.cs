using System;
using Gtk;
using Mono.Posix;
using Mono.Unix;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Collections.Specialized;
using System.Xml;
using System.Web;
using WebKit;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ConnectionMap
{
	public partial class HomeScreen : Gtk.Window
	{
		private String    		consoleText     = "";
		private IPLookup  		localIPLookup   = new IPLookup();
		private IPLookup  		globalIPLookup  = new IPLookup();
		private NetInfo   		netInfo         = new NetInfo ();
		private InternalWebView internalWebView = new InternalWebView();
		BackgroundWorker bgw;
		private bool auto_update = false;

		TaskCompletionSource<bool> firstMapLoadCompletion = new TaskCompletionSource<bool>();

		public HomeScreen () :
			base (Gtk.WindowType.Toplevel)
		{
			
		}
	
	}
}

