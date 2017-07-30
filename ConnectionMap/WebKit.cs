using System.Collections;
using Gdk;
using GLib;
using Gtk;
using System;
using System.Runtime.InteropServices;

namespace WebKit
{
	class DOMDocument : GLib.Object
	{
		public DOMDocument (IntPtr raw) : base (raw) {}

		[DllImport ("webkit-1.0")]
		private static extern IntPtr webkit_dom_document_get_elements_by_tag_name (IntPtr raw, IntPtr str1ng);

		public DOMNodeList get_elements_by_tag_name (string tag)
		{
			IntPtr intPtr = Marshaller.StringToPtrGStrdup (tag);
			IntPtr result = webkit_dom_document_get_elements_by_tag_name(base.Handle, intPtr);
			Marshaller.Free (intPtr);
			return new DOMNodeList(result);
		}
	}

	// http://www.opensource.apple.com/source/WebKit/WebKit-7533.16/gtk/tests/testdomdocument.c
	class DOMNodeList: GLib.Object, IEnumerable
	{
		public DOMNodeList (IntPtr raw) : base (raw) {}

		[DllImport ("webkit-1.0")]
		private static extern IntPtr webkit_dom_node_list_item (IntPtr raw, int element);

		[DllImport ("webkit-1.0")]
		private static extern int webkit_dom_node_list_get_length (IntPtr raw);

		public IEnumerator GetEnumerator ()
		{
			for (int i = 0; i < webkit_dom_node_list_get_length(base.Handle); i++) {
				yield return new DOMNode(webkit_dom_node_list_item(base.Handle, i));
			}
		}
	}

	class DOMNode : GLib.Object
	{
		public DOMNode (IntPtr raw) : base (raw) {}


	}

	class DOMElement : GLib.Object
	{
		public DOMElement (IntPtr raw) : base (raw) {}
	}

	class DOMHTMLElement : GLib.Object
	{
		public DOMHTMLElement (IntPtr raw) : base (raw) {}

		[DllImport ("webkit-1.0")]
		private static extern IntPtr webkit_dom_html_element_get_inner_text(IntPtr raw);

		public string get_inner_text ()
		{
			IntPtr data = webkit_dom_html_element_get_inner_text(base.Handle);
			return Marshaller.PtrToStringGFree(data);
		}
	}

	class ExtendedWebSettings : WebKit.WebSettings {
		public void g_object_set(string name, GLib.Value value) {
			SetProperty(name, value);
		}

		public GLib.Value g_object_get(string name) {
			return GetProperty(name);
		}
	}

	class ExtendedWebView : WebKit.WebView {

		[DllImport ("webkit-1.0")]
		private static extern IntPtr webkit_web_view_get_dom_document (IntPtr raw);

		public DOMDocument get_dom_document()
		{
			return new DOMDocument(webkit_web_view_get_dom_document(base.Handle));
		}

		public event CreateWebViewHandler CreateWebView
		{
			add
			{
				Signal signal = Signal.Lookup (this, "create-web-view", typeof(CreateWebViewArgs));
				signal.AddDelegate (value);
			}
			remove
			{
				Signal signal = Signal.Lookup (this, "create-web-view", typeof(CreateWebViewArgs));
				signal.RemoveDelegate (value);
			}
		}

		public event WebViewReadyHandler WebViewReady
		{
			add
			{
				Signal signal = Signal.Lookup (this, "web-view-ready", typeof(WebViewReadyArgs));
				signal.AddDelegate (value);
			}
			remove
			{
				Signal signal = Signal.Lookup (this, "web-view-ready", typeof(WebViewReadyArgs));
				signal.RemoveDelegate (value);
			}
		}

		public event NewWindowPolicyDecisionRequestedHandler NewWindowPolicyDecisionRequested
		{
			add
			{
				Signal signal = Signal.Lookup (this, "new-window-policy-decision-requested", typeof(NewWindowPolicyDecisionRequestedArgs));
				signal.AddDelegate (value);
			}
			remove
			{
				Signal signal = Signal.Lookup (this, "new-window-policy-decision-requested", typeof(NewWindowPolicyDecisionRequestedArgs));
				signal.RemoveDelegate (value);
			}
		}

		[DefaultSignalHandler (Type = typeof(WebView), ConnectionMethod = "OverrideCreateWebView")]
		protected virtual WebView OnCreateWebView (WebFrame frame)
		{
			ExtendedWebView webView = new ExtendedWebView();
			Value empty = Value.Empty;
			ValueArray valueArray = new ValueArray (2u);
			Value[] array = new Value[2];
			array [0] = new Value (this);
			valueArray.Append (array [0]);
			array [1] = new Value (frame);
			valueArray.Append (array [1]);
			GLib.Object.g_signal_chain_from_overridden (valueArray.ArrayPtr, ref empty);
			Value[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Value value = array2 [i];
				value.Dispose ();
			}
			return webView;
		}

		[DefaultSignalHandler (Type = typeof(WebView), ConnectionMethod = "OverrideNewWindowPolicyDecisionRequested")]
		protected virtual int OnNewWindowPolicyDecisionRequested (WebFrame frame, NetworkRequest request, WebNavigationAction navigation_action, WebPolicyDecision policy_decision)
		{
			Value val = new Value (GType.Int);
			ValueArray valueArray = new ValueArray (3u);
			Value[] array = new Value[5];
			array [0] = new Value (this);
			valueArray.Append (array [0]);
			array [1] = new Value (frame);
			valueArray.Append (array [1]);
			array [2] = new Value (request);
			valueArray.Append (array [2]);
			array [3] = new Value (navigation_action);
			valueArray.Append (array [3]);
			array [4] = new Value (policy_decision);
			valueArray.Append (array [4]);
			GLib.Object.g_signal_chain_from_overridden (valueArray.ArrayPtr, ref val);
			Value[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Value value = array2 [i];
				value.Dispose ();
			}
			int result = (int)val;
			val.Dispose ();
			return result;
		}

		[DefaultSignalHandler (Type = typeof(WebView), ConnectionMethod = "OverrideWebViewReady")]
		protected virtual bool OnWebViewReady (WebFrame frame)
		{
			Value empty = Value.Empty;
			ValueArray valueArray = new ValueArray (2u);
			Value[] array = new Value[2];
			array [0] = new Value (this);
			valueArray.Append (array [0]);
			array [1] = new Value (frame);
			valueArray.Append (array [1]);
			GLib.Object.g_signal_chain_from_overridden (valueArray.ArrayPtr, ref empty);
			Value[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Value value = array2 [i];
				value.Dispose ();
			}
			return true;
		}
	}

	public delegate void WebViewReadyHandler (object o, WebViewReadyArgs args);

	public class WebViewReadyArgs : SignalArgs
	{
		//
		// Properties
		//

		public WebFrame Frame
		{
			get
			{
				return (WebFrame)base.Args [0];
			}
		}
	}

	public delegate void CreateWebViewHandler (object o, CreateWebViewArgs args);

	public class CreateWebViewArgs : SignalArgs
	{
		//
		// Properties
		//

		public WebFrame Frame
		{
			get
			{
				return (WebFrame)base.Args [0];
			}
		}
	}

	public delegate void NewWindowPolicyDecisionRequestedHandler (object o, NewWindowPolicyDecisionRequestedArgs args);

	public class NewWindowPolicyDecisionRequestedArgs : SignalArgs
	{
		//
		// Properties
		//

		public WebFrame Frame
		{
			get
			{
				return (WebFrame)base.Args [0];
			}
		}

		public NetworkRequest Request
		{
			get
			{
				return (NetworkRequest)base.Args [1];
			}
		}

		public WebNavigationAction NavigationAction
		{
			get
			{
				return (WebNavigationAction)base.Args [2];
			}
		}

		public WebPolicyDecision PolicyDecision
		{
			get
			{
				return (WebPolicyDecision)base.Args [3];
			}
		}
	}

}


