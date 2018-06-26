using System;
using System.Reflection;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Gecko;


namespace PaPortable.Windows
{
	static class Program
	{
		private static readonly List<String> SupportFile = new List<string>();
		static TrappingGecko _browser;

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Xpcom.Initialize("Firefox");
			var randomName = Path.GetTempFileName();
			if (File.Exists(randomName))
				File.Delete(randomName);
			var indexFullName = CreateResources(Path.GetFileNameWithoutExtension(randomName));
			var serverFullName = $@"{Path.GetDirectoryName(indexFullName)}\SimpleServer.exe";
			File.Copy("SimpleServer.exe", serverFullName);
			SupportFile.Add(serverFullName);
			var startInfo = new ProcessStartInfo
			{
				FileName = serverFullName,
				WindowStyle = ProcessWindowStyle.Hidden,
				RedirectStandardOutput = true,
				UseShellExecute = false
			};
			using (var p = new Process {StartInfo = startInfo})
			{
				p.Start();
				var f = new Form { Size = new Size(800, 800) };
				_browser = new TrappingGecko { SupportFile = SupportFile, Dock = DockStyle.Fill };
				f.Controls.Add(_browser);
				var portAddr = GetPortAddr(p);
				_browser.Navigate($"http://localhost:{portAddr}");
				Application.Run(f);
				p.Kill();
				p.WaitForExit();
			}
			foreach (var fullPath in SupportFile)
			{
				File.Delete(fullPath);
				var folder = Path.GetDirectoryName(fullPath);
				try
				{
					Directory.Delete(folder);
				}
				catch
				{
					// if not empty, ignore delete directory
				}
			}
		}

		private static string GetPortAddr(Process p)
		{
			var buffer = new char[17];
			var task = p.StandardOutput.ReadAsync(buffer, 0, 17);
			while (!task.IsCompleted)
				Thread.Sleep(50);
			var portAddr = Encoding.UTF8.GetBytes(buffer, 13, 4).ToString();
			return portAddr.Length == 4 ? portAddr : "3010";
		}

		private static string CreateResources(string randomName)
		{
			var folder = Path.Combine(Path.GetTempPath(), randomName);
			if (!Directory.Exists(folder))
				Directory.CreateDirectory(folder);
			var portableName = new DirectoryInfo(".").GetFiles("PaPortable.dll")[0].FullName;
			var assembly = Assembly.LoadFile(portableName);
			WriteResource(folder, assembly, "css", "app.css");
			WriteResource(folder, assembly, "css", "app.css.map");
			WriteResource(folder, assembly, "fonts", "opensans-regular-webfont.woff");
			WriteResource(folder, assembly, "images", "favicon.ico");
			WriteResource(folder, assembly, "js", "application.js");
			WriteResource(folder, assembly, "js", "vendor.bundle.js");
			return WriteResource(folder, assembly, "", "index.html");
		}

		private static string WriteResource(string folder, Assembly assembly, string projectLocation, string name)
		{
			const string resourceBase = "PaPortable.react.build.client.";
			var resourceLocation = resourceBase;
			if (!string.IsNullOrEmpty(projectLocation))
				resourceLocation += projectLocation + ".";
			string  fullPath;
			using (var str = new StreamReader(assembly.GetManifestResourceStream(resourceLocation + name)))
			{
				var myFolder = string.IsNullOrEmpty(projectLocation)? folder : Path.Combine(folder, projectLocation);
				if (!Directory.Exists(myFolder)) Directory.CreateDirectory(myFolder);
				fullPath = Path.Combine(myFolder, name);
				var buffer = new byte[1000];
				using (var os = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
				{
					int count;
					do
					{
						count = str.BaseStream.Read(buffer, 0, 1000);
						os.Write(buffer, 0, count);
					} while (count > 0);
				}
				SupportFile.Add(fullPath);
			}

			return fullPath;
		}

	}
}
