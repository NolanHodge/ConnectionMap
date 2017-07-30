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
	public partial class MainControl : Gtk.Window
	{
		private String    		consoleText     = "";
		private IPLookup  		localIPLookup   = new IPLookup();
		private IPLookup  		globalIPLookup  = new IPLookup();
		private NetInfo   		netInfo         = new NetInfo ();
		private InternalWebView internalWebView = new InternalWebView();
		BackgroundWorker bgw;
		private bool auto_update = false;
		private ConnectionMap.MainControl.THEMES currentTheme = THEMES.DEFAULT;
		TaskCompletionSource<bool> firstMapLoadCompletion = new TaskCompletionSource<bool>();
		private String currentCustomTheme = "";

		public MainControl () :
			base (Gtk.WindowType.Toplevel)
		{
			internalWebView.LoadFinished += OnMapLoaded;
			SetDefaultSize (700, 700);
			Show ();
			Build ();
			sendToConsole ("Welcome!");
			localIPLookup.simpleIpLookup();
			vbox3.Add(internalWebView);
			applyCustomMaps ();
			startUp ();		
		}
		public void startUp()
		{
			printLocalIPInformation ();
			checkMap ();
		}
		private async void checkMap()
		{
			internalWebView.LoadFinished += (s, e) =>
			{
				firstMapLoadCompletion?.TrySetResult(true);
			};
			await firstMapLoadCompletion.Task;
			internalWebView.setUserLatLon (localIPLookup.getLatitude, localIPLookup.getLongitude);
			internalWebView.addMarker(localIPLookup.getLatitude,localIPLookup.getLongitude, InternalWebView.HOME_COMP_ICON, "Your Machine");
			internalWebView.centerOnUser();
			getActiveConnections();
			RunAutoMapUpdater ();
		}
		public void getActiveConnections()
		{
			netInfo.getActiveConnections ();
			sendToConsole ("Collecting Active Connections..");
			String[,] ForeignAddresses = netInfo.getForeignAddresses;
			for (int i = 0; i < netInfo.getAddressCount; i++) 
			{
				String ip = ForeignAddresses [i, 0];
				String port = ForeignAddresses [i, 1];
				if (!ip.Equals (IPLookup.LOCALHOST))
					printConnection (ip, port);
			}
		}
		public String getConsoleText(String strToAdd="")
		{
			if (strToAdd.Length == 0) 
			{
				return consoleText;
			}
			consoleText += "> ";
			consoleText += strToAdd;
			consoleText += "\n";
			return consoleText;
		}
		public void printLocalIPInformation()
		{
			sendToConsole ("\n>-------- Your Public Information ----------");
			for (int i=0; i<IPLookup.TOTAL_GEO; i++)
			{
				String key = localIPLookup.getGeoInfoAt (i,0);
				String val = localIPLookup.getGeoInfoAt (i,1);
				if (val.Length != 0) 
				{
					sendToConsole (key + ": " + val);
				}
			}
			sendToConsole ("-------------------------------------------------\n");
		}
		public void printConnection(String ipaddr, String port)
		{
			globalIPLookup.simpleIpLookup (ipaddr);
			sendToConsole ("\n>-------- Active Connection ----------");
			sendToConsole ("" + ipaddr + ":" + port);
			for (int i = 0; i < IPLookup.TOTAL_GEO; i++) 
			{
				String key = globalIPLookup.getGeoInfo [i, 0];
				String val = globalIPLookup.getGeoInfo [i, 1];
				if (val.Length != 0) 
				{
					if (key.Equals ("Latitude")) 
					{
						String ipAndCountry = globalIPLookup.getIP + " : " + globalIPLookup.getCity + " : " + globalIPLookup.getCountryName;
						internalWebView.addMarker (val, globalIPLookup.getGeoInfo [i + 1, 1], InternalWebView.SERVER_ICON, ipAndCountry);
					}
					sendToConsole (key + ": " + val);
				}
			}
			sendToConsole ("-------------------------------------------------\n");
		}
		public void sendToConsole(String text="", String color="green")
		{
			Gdk.Color colo = new Gdk.Color ();
			Gdk.Color.Parse (color, ref colo);
			console_tv.ModifyText (StateType.Normal, colo);
			getConsoleText (text);
			console_tv.Buffer.Text = getConsoleText();
			console_tv.ScrollToIter (console_tv.Buffer.EndIter, 0, false, 0, 0);
		}
		protected void onRefresh (object sender, EventArgs e)
		{
			restart ();
		}
		public void RunAutoMapUpdater()
		{
			if (!auto_update) 
			{
				return;
			} 
			bgw = new BackgroundWorker ();
			bgw.ProgressChanged += new ProgressChangedEventHandler(backgroundWorker_ProgressChanged);
			bgw.WorkerReportsProgress  = true;
			this.Dispose ();
			bgw.DoWork += (o,e) => {
				int i=0;
				while(true)
				{
					Thread.Sleep(1000);
					Console.WriteLine("sleeping for " + (++i) + " s" );
					if (i%5 == 0)
					{
						bgw.ReportProgress(0, "");
						bgw.CancelAsync();
					}
				}
			};
			bgw.RunWorkerAsync();
		}
		// This event handler updates the UI
		private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			// Update the UI here
			RunAutoMapUpdater();
			restart ();
		}
		protected void SetAutoUpdateConnections (object sender, EventArgs e)
		{
			hbox2.Remove (hbox2.Children[1]);
			auto_update = true;
			restart ();
		}
		protected void SetManualConnectionUpdate (object sender, EventArgs e)
		{
			hbox2.Add (refresh_button);
			auto_update = false;
			restart ();
		}
		private void restart()
		{
			internalWebView.removeMarkersAndLines ();
			consoleText = "";
			startUp ();
		}
		protected void create_custom_theme()
		{
			try {
				String countryKeyword = "{elementType: 'geometry'";
				String waterKeyword = "featureType: 'water'";

				StreamWriter sw = new StreamWriter(internalWebView.getMapFromFilename("custom_theme.html"));
				CustomMapPopup cmw = new CustomMapPopup();
				cmw.Hidden += (s,e) => {
					using (var sr = new StreamReader(internalWebView.getMapFromFilename("custom_theme_template.html"))) {
						while (!sr.EndOfStream) {
							var line = sr.ReadLine();
							if (String.IsNullOrEmpty(line)) continue;
							if (line.IndexOf(countryKeyword, StringComparison.CurrentCultureIgnoreCase) >= 0) {
								sw.WriteLine("{elementType: 'geometry', stylers: [{color: '#" + cmw.countryColor + "'}]},");
							} else if (line.IndexOf(waterKeyword, StringComparison.CurrentCultureIgnoreCase) >= 0) {
								sw.WriteLine("featureType: 'water',\n");
								line = sr.ReadLine();
								if (line.Contains("elementType: 'geometry'")) {
									sw.WriteLine("elementType: 'geometry',\n stylers: [{color: '#" + cmw.waterColor + "'}]");
									line = sr.ReadLine();
								} else if (line.Contains("elementType: 'labels.text.fill'")) {
									sw.WriteLine("elementType: 'labels.text.fill',\n stylers: [{color: '#" + cmw.waterLabelColor + "'}]");
									line = sr.ReadLine();
								} else{
									sw.WriteLine(line);
									line = sr.ReadLine();
									sw.WriteLine(line);
								}
							} else if (line.Contains("{elementType: 'labels.text.fill'")) {
								sw.WriteLine("{elementType: 'labels.text.fill', stylers: [{color: '#" + cmw.countryLabelColor + "'}]},");
							} else {
								sw.WriteLine(line);
							}
						}
					}
					sw.Close();
					if (cmw.save) {
						SaveMapMenu sm = new SaveMapMenu();
						sm.Hidden += (se,ev) => {
							if (sm.filename != "" && sm.filename.Length < 20) {
								String menuItemName = sm.filename;
								sm.filename = "user_custom_" + sm.filename;
								sm.filename = sm.filename.Replace("_", "")  + ".html";

								File.Delete(internalWebView.getMapThemeDirectory + sm.filename + ".html");
								File.Copy(internalWebView.getMapFromFilename("custom_theme.html"), internalWebView.getMapThemeDirectory + sm.filename);
								MenuItem Themes = (MenuItem)menubar2.Children[0];
								Menu ThemesMenu = (Menu)Themes.Submenu; 
								foreach (MenuItem x in ThemesMenu) {
									if (x.Submenu != null)  {
										Menu ThemesSubMenu = (Menu)x.Submenu; 
										MenuItem Custom = new MenuItem(menuItemName);
										Custom.Activated += (sen, eve) => {set_theme_from_customs(sen,eve,sm.filename);};
										ThemesSubMenu.Add(Custom);
									}
								}
								menubar2.ShowAll();
							}
							sm.Destroy();
						};
						sm.Show();
					}
					cmw.Destroy();
					apply_custom_theme(s,e);
				};
			} catch (Exception e) {
				sendToConsole (e.ToString());
			}
		}
		private void applyCustomMaps() {
			MenuItem Themes = (MenuItem)menubar2.Children[0];
			Menu ThemesMenu = (Menu)Themes.Submenu; 
			foreach (String file in Directory.GetFiles(internalWebView.getMapThemeDirectory)) {
				if (file.Contains ("usercustom")) {
					int pFrom = file.IndexOf("/MapThemes/")+"/MapThemes/".Length;
					int pTo = file.LastIndexOf(".html");
					String mapname = file.Substring(pFrom, pTo - pFrom);

					foreach (MenuItem x in ThemesMenu) {
						if (x.Submenu != null)  {
							Menu ThemesSubMenu = (Menu)x.Submenu; 
							MenuItem Custom = new MenuItem(mapname.Replace("usercustom",""));
							Custom.Activated += (sen, eve) => {set_theme_from_customs(sen,eve,mapname);};
							ThemesSubMenu.Add(Custom);
						}
					}
				}
			}
			menubar2.ShowAll();
		}
		protected void set_custom_theme (object sender, EventArgs e)
		{
			create_custom_theme ();
		}
		protected void apply_custom_theme (object sender, EventArgs e)
		{
			currentTheme = THEMES.CUSTOM;
			internalWebView.newTheme("custom_theme.html");	
		}
		protected void set_theme_from_customs (object sender, EventArgs e, String filename) {
			if (currentTheme == THEMES.CUSTOM && currentCustomTheme == filename)
				return;
			currentCustomTheme = filename;
			internalWebView.newTheme(filename+".html");
		}
		protected void set_default_theme (object sender, EventArgs e)
		{
			if (currentTheme == THEMES.DEFAULT)
				return;
			currentTheme = THEMES.DEFAULT;
			internalWebView.newTheme("default_theme.html");
		}
		protected void set_desert_theme (object sender, EventArgs e)
		{
			if (currentTheme == THEMES.DESERT)
				return;
			currentTheme = THEMES.DESERT;
			internalWebView.newTheme("desert_theme.html");
		}
		protected void set_forest_theme (object sender, EventArgs e)
		{
			if (currentTheme == THEMES.FOREST)
				return;
			currentTheme = THEMES.FOREST;
			internalWebView.newTheme("forest_theme.html");
		}
		protected void set_dark_theme (object sender, EventArgs e)
		{
			if (currentTheme == THEMES.DARK)
				return;
			currentTheme = THEMES.DARK;
			internalWebView.newTheme("dark_theme.html");		
		}
		protected void set_midnight_theme (object sender, EventArgs e)
		{
			if (currentTheme == THEMES.MIDNIGHT)
				return;
			currentTheme = THEMES.MIDNIGHT;
			internalWebView.newTheme("midnight_theme.html");		
		}
		protected void set_lunar_theme (object sender, EventArgs e)
		{
			if (currentTheme == THEMES.LUNAR)
				return;
			currentTheme = THEMES.LUNAR;
			internalWebView.newTheme("lunar_theme.html");		
		} 
		protected void set_corduroy_theme (object sender, EventArgs e)
		{
			if (currentTheme == THEMES.CORDUROY)
				return;
			currentTheme = THEMES.CORDUROY;
			internalWebView.newTheme("corduroy_theme.html");		
		}
		private enum THEMES : int 
		{
			DEFAULT = 0,
			DESERT,
			FOREST,
			DARK,
			MIDNIGHT,
			LUNAR,
			CORDUROY,
			CUSTOM
		};
		protected void OnMapLoaded(object sender, EventArgs e)  
		{  
			internalWebView.initializeJavascriptArrays ();
			restart();
		} 
	}
}

