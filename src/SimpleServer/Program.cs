using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SimpleServer
{
	class Program
	{
		static void Main(string[] args)
		{
			// Hide Window
			var handle = GetConsoleWindow();
			ShowWindow(handle, SW_HIDE);

			var portAddr = int.Parse(args.Length > 1? args[1]: "3010");

			var server = new HttpListener();

			var success = false;
			var adder = 0;
			for (var n = 0; !success; n++)
			{
				adder = Fib(n);
				server.Prefixes.Add($"http://127.0.0.1:{portAddr + adder}/");
				server.Prefixes.Add($"http://localhost:{portAddr + adder}/");
				try
				{
					server.Start();
					success = true;
				}
				catch (Exception)
				{
					// Server disposed on error
					server = new HttpListener();
				}
			}

			portAddr += adder;

			var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			Console.WriteLine($"Listening to {portAddr}");

			while (true)
			{
				var context = server.GetContext();
				var response = context.Response;

				string page = context.Request.Url.LocalPath;

				if (string.IsNullOrEmpty(page) || page == "/")
					page = "/index.html";

				page = folder + page;

				Console.WriteLine($@"Request page: {page}");

				var st = response.OutputStream;

				using (var reader = new FileStream(page, FileMode.Open, FileAccess.Read))
				{
					response.ContentLength64 = reader.Length;
					const int blockSize = 10024;
					var buffer = new byte[blockSize];
					for (var c = reader.Length; c > 0;)
					{
						var bytesRead = reader.Read(buffer, 0, blockSize);
						st.Write(buffer, 0, bytesRead);
						c -= bytesRead;
					}
				}

				context.Response.Close();
			}
		}

		private static int Fib(int n)
		{
			if (n == 0) return 0;
			if (n == 1) return 1;
			return Fib(n - 1) + Fib(n - 2);
		}

		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;
	}
}
