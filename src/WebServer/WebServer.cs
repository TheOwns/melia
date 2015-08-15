﻿using Melia.Shared;
using Melia.Shared.Util;
using Melia.Shared.Util.Commands;
using Melia.Shared.Util.Configuration;
using Melia.Web.Controllers;
using SharpExpress;
using SharpExpress.Engines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Melia.Web
{
	public class WebServer : Server
	{
		public static readonly WebServer Instance = new WebServer();

		/// <summary>
		/// Web application
		/// </summary>
		public WebApplication App { get; private set; }

		/// <summary>
		/// Web's console commands
		/// </summary>
		public ConsoleCommands ConsoleCommands { get; private set; }

		/// <summary>
		/// Starts the server.
		/// </summary>
		public override void Run()
		{
			base.Run();

			Cmd.WriteHeader("Web Server", ConsoleColor.DarkRed);
			Cmd.LoadingTitle();

			// Conf
			this.LoadConf();

			// Web server
			this.StartWebServer();

			Cmd.RunningTitle();

			// Commands
			this.ConsoleCommands = new ConsoleCommands();
			this.ConsoleCommands.Wait();
		}

		/// <summary>
		/// Sets up default controllers and starts web server
		/// </summary>
		public void StartWebServer()
		{
			Log.Info("Starting web server...");

			this.App = new WebApplication();

			this.App.Engine("htm", new HandlebarsEngine());

			this.App.Get("/toscdn/patch/serverlist.xml", new ServerListController());
			this.App.Get("/toscdn/patch/static__Config.txt", new StaticConfigController());

			try
			{
				this.App.Listen(8080);

				Log.Info("ServerListURL: *:{0}/{1}", 8080, "toscdn/patch/serverlist.xml");
				Log.Info("StaticConfigURL: *:{0}/{1}", 8080, "toscdn/patch/");
				Log.Status("Server ready, listening on 0.0.0.0:{0}.", 8080);
			}
			catch (HttpListenerException)
			{
				Log.Error("Failed to start web server.");
				Log.Info("The port might already be in use, make sure no other application, like other web servers or Skype, are using it, or set a different port in web.conf.");
				Cmd.Exit(1);
			}
		}
	}
}
