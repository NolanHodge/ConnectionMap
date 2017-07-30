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
using Mono.WebBrowser;
using System.Web;
using WebKit;

namespace ConnectionMap
{
	public class InternalWebView : WebView
	{
		public int loadCheck                = 0;
		public static String SERVER_ICON    = "server";
		public static String HOME_COMP_ICON = "home";
		private String userLon = "";
		private String userLat = "";
		delegate void Func<T>(T t);
		public InternalWebView()
		{
			Open (getMapFromFilename("default_theme.html"));
			SetSizeRequest (0, 100);
			initializeJavascriptArrays ();
			ShowAll ();
		}

		public void newTheme(String theme) {
			Open (getMapFromFilename(theme));
			initializeJavascriptArrays ();
			ShowAll ();
		}

		public void addMarker(String lat, String lon, String icon, String title)
		{
			if (lat.Equals ("0") || lon.Equals ("0"))
				return;
			String script = " var marker = new google.maps.Marker({ position: {lat: "+lat+", lng: "+lon+"}, map: map, icon: "+icon+", title: '"+title+"' }); markers.push(marker);";
			ExecuteScript (script);  
			String script2 = "var line = new google.maps.Polyline({ path: [new google.maps.LatLng("+userLat+", "+userLon+"), new google.maps.LatLng("+lat+", "+lon+")], strokeColor: \"#66FF00\", strokeOpacity: 1.0, strokeWeight: 1, geodesic: true, map: map}); line.setMap(map); polylines.push(line);";
			ExecuteScript (script2);  
		}
		public void centerOnUser()
		{
			String script = "map.setCenter(new google.maps.LatLng("+userLat+", "+userLon+"));";
			ExecuteScript (script);  
		}
		public void initializeJavascriptArrays()
		{
			String script = "var markers = []; var polylines = []";
			ExecuteScript (script);
		}
		public void removeMarkersAndLines()
		{
			String script = "function removeMarkers(){ for(i=0; i<markers.length; i++){ markers[i].setMap(null); }} removeMarkers();";
			ExecuteScript (script);  
			script = "function removeLines(){ for(i=0; i<polylines.length; i++){ polylines[i].setMap(null); }} removeLines();";
			ExecuteScript (script);  
			refreshMap ();
		}
		public void refreshMap()
		{
			String script = "google.maps.event.trigger(map, 'resize');";
			ExecuteScript (script);  
		}
		public void setUserLatLon(String newLat, String newLon)
		{
			userLon = newLon;
			userLat = newLat;
		}
		public string getMapFromFilename(String filename) {
			return Directory.GetCurrentDirectory () + "/MapThemes/" + filename;
		}
		public string getMapThemeDirectory {
			get {
				return Directory.GetCurrentDirectory () + "/MapThemes/";
			}
		}
	}
}

