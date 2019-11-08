using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RedOnion.KSP.API
{
	[Description("Safe API for KSP Application Launcher (toolbar/buttons).")]
	public static class App
	{
		[Description("Default Red Onion Icon")]
		public static Texture2D DefaultIcon
			=> _defaultIcon ?? (_defaultIcon
			= UI.Element.LoadIcon(38, 38, "RedOnionLauncherIcon.png"));
		private static Texture2D _defaultIcon;

		//public static void Add()
	}

	/*
	[KSPAddon(KSPAddon.Startup.AllGameScenes, false)]
	public class AppButtonManager : MonoBehaviour
	{
		//TODO: Instance and Add/Remove Queue
	}
	*/
}
