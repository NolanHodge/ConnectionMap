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
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ConnectionMap
{
	public class IPLookup
	{
		public static int    TOTAL_GEO    = 11;
		public static String LOCALHOST = "127.0";
		public static String LOCALIP   = "192.168";

		private String[]  IP           = new String[2];
		private String[]  CountryName  = new String[2];
		private String[]  CountryCode  = new String[2];
		private String[]  RegionCode   = new String[2];
		private String[]  RegionName   = new String[2];
		private String[]  City         = new String[2];
		private String[]  ZipCode      = new String[2];
		private String[]  TimeZone     = new String[2];
		private String[]  Latitude     = new String[2];
		private String[]  Longitude    = new String[2];
		private String[]  MetroCode    = new String[2];
		private String[,] GeoInfo      = new string[TOTAL_GEO,2];

		public IPLookup ()
		{
		}

		public void simpleIpLookup(String ipaddr="")
		{
			var client = new System.Net.WebClient ();
			var response = client.DownloadString ("http://freegeoip.net/xml/" + ipaddr);
			stripResponseTags (response);
		}
		public void stripResponseTags(string response)
		{
			GeoInfo [0,0] = "IP";
			GeoInfo [1,0] = "CountryCode";
			GeoInfo [2,0] = "CountryName";
			GeoInfo [3,0] = "RegionCode";
			GeoInfo [4,0] = "RegionName";
			GeoInfo [5,0] = "City";
			GeoInfo [6,0] = "ZipCode";
			GeoInfo [7,0] = "TimeZone";
			GeoInfo [8,0] = "Latitude";
			GeoInfo [9,0] = "Longitude";
			GeoInfo [10,0] = "MetroCode";

			Regex regex  = new Regex("<IP>(.*)</IP>");
			var v = regex.Match(response);
			string strip = v.Groups[1].ToString();
			IP [1] = strip;
			GeoInfo [0,1] = IP[1];

			regex = new Regex("<CountryCode>(.*)</CountryCode>");
			v  = regex.Match(response);
			strip = v.Groups[1].ToString();
			CountryCode[1] = strip;
			GeoInfo [1,1] = CountryCode[1];

			regex = new Regex("<CountryName>(.*)</CountryName>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			CountryName[1] = strip;
			GeoInfo [2,1] = CountryName[1];

			regex = new Regex("<RegionCode>(.*)</RegionCode>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			RegionCode[1] = strip;
			GeoInfo [3,1] = RegionCode[1];

			regex = new Regex("<RegionName>(.*)</RegionName>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			RegionName[1] = strip;
			GeoInfo [4,1] = RegionName[1];

			regex = new Regex("<City>(.*)</City>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			City[1] = strip;
			GeoInfo [5,1] = City[1];

			regex = new Regex("<ZipCode>(.*)</ZipCode>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			ZipCode[1] = strip;
			GeoInfo [6,1] = ZipCode[1];

			regex = new Regex("<TimeZone>(.*)</TimeZone>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			TimeZone[1] = strip;
			GeoInfo [7,1] = TimeZone[1];

			regex = new Regex("<Latitude>(.*)</Latitude>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			Latitude[1] = strip;
			GeoInfo [8,1] = Latitude[1];

			regex = new Regex("<Longitude>(.*)</Longitude>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			Longitude[1] = strip;
			GeoInfo [9,1] = Longitude[1];

			regex = new Regex("<MetroCode>(.*)</MetroCode>");
			v = regex.Match(response);
			strip = v.Groups[1].ToString();
			MetroCode[1] = strip;
			GeoInfo [10,1] = MetroCode[1];
		}
		// Private Accessors 
		public String    getIP          { get { return IP[1];          } }
		public String    getCountryCode { get { return CountryCode[1]; } }
		public String    getCountryName { get { return CountryName[1]; } }
		public String    getRegionName  { get { return RegionName[1];  } }
		public String    getCity        { get { return City[1];        } }
		public String    getTimeZone    { get { return TimeZone[1];    } }
		public String    getLatitude    { get { return Latitude[1];    } }
		public String    getLongitude   { get { return Longitude[1];   } }
		public String    getMetroCode   { get { return MetroCode[1];   } }

		public String[,] getGeoInfo     { get { return GeoInfo;        } }

		public String getGeoInfoAt(int x, int y)     
		{   
			if (x < TOTAL_GEO && y < 2) 
			{
				return GeoInfo [x,y];
			}
			return "";
		}

	}
}