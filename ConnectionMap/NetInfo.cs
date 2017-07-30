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
	public class NetInfo
	{
		private ArrayList ForeignIPAddresses = new ArrayList();
		private ArrayList ForeignPorts       = new ArrayList();
		private String[,] ForeignAddresses;
		private int addressCount             = 0;

		public NetInfo ()
		{
		}

		public void getActiveConnections()
		{
			List<string> _netstatLines = new List<string>();
			ProcessStartInfo psi = new ProcessStartInfo("netstat", "-a -n -o");
			psi.CreateNoWindow = true;
			psi.ErrorDialog = false;
			psi.UseShellExecute = false;
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.Verb = "runas";
			psi.WindowStyle = ProcessWindowStyle.Hidden;
			Process process = new Process();
			process.EnableRaisingEvents = true;
			process.StartInfo = psi;
			process.ErrorDataReceived += (s, e) => { _netstatLines.Add(e.Data); };
			process.OutputDataReceived += (s, e) => { _netstatLines.Add(e.Data); };
			process.Start();
			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			process.WaitForExit();

			foreach (var s in _netstatLines) 
			{
				if (s != null)
				{
					string[] rows = Regex.Split(s, "\r\n");
					foreach (string row in rows) {
						string[] tokens = Regex.Split (row, "\\s+");
						foreach (string tok in tokens) {
							if (tok.Equals ("UDP") || tok.Equals ("TCP") || tok.Equals ("udp") || tok.Equals ("tcp")) {
								string foreignAddress = Regex.Replace (tokens [4], @"\[(.*?)\]", "1.1.1.1");
								char[] splitBy = { ':' };
								if (foreignAddress [0] == '0')
									continue;
								string foreignIP = foreignAddress.Split (splitBy) [0];
								string foreignPort = foreignAddress.Split (splitBy) [1];

								if (!foreignIP.Contains(IPLookup.LOCALIP) &&
									!foreignIP.Contains(IPLookup.LOCALHOST) && 
									!containsAddress (ForeignIPAddresses, ForeignPorts, foreignIP, foreignPort)) 
								{
									ForeignIPAddresses.Add (foreignIP);
									ForeignPorts.Add (foreignPort);
									addressCount++;
								}
							}
						}
					}
				}
			}
			setForeignAddresses ();
		}
		public bool containsAddress(ArrayList ipArray, ArrayList portArray, String ip, String port)
		{
			for (int i = 0; i < addressCount; i++)
			{
				if (ipArray[i].Equals(ip) && portArray[i].Equals(port))
				{
					return true;
				}
			}
			return false;
		}
		public ArrayList getForeignIPAddresses
		{
			get
			{
				return ForeignIPAddresses;
			}
		}
		public ArrayList getForeignPorts
		{
			get
			{
				return ForeignPorts;
			}
		}
		public void setForeignAddresses()
		{
			ForeignAddresses = new string[addressCount,2];
			int i = 0;
			foreach (String ip in ForeignIPAddresses) 
			{
				if (i == 99)
					break;
				ForeignAddresses [i++, 0] = ip;
			}
			i = 0;
			foreach (String port in ForeignPorts) 
			{
				if (i == 99)
					break;
				ForeignAddresses [i++, 1] = port;
			}
		}
		public String[,] getForeignAddresses
		{
			get
			{
				return ForeignAddresses;
			}
		}
		public void clearAddresses()
		{
			ForeignAddresses = new String[11,2];
		}
		public int getAddressCount
		{
			get 
			{
				return addressCount;
			}
		}
	}
}

