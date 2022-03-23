using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using SisMaper.ViewModel;
using SisMaper.Views;

namespace SisMaper.Tools;

public static class HelpWebService
{
    public static FullCmd<string> OpenHelp => new(ShowHelp);

    private static string URL = "http://localhost:33067/";
    private static Thread ServerThread;
    private static CancellationTokenSource CancelSource;
    private static HttpListener Server;
    private static ViewHelpBrowser? Window; 

    public static void ShowHelp(string? path)
    {
        if (path != null && !path.Contains(".html"))
        {
            if (path.Contains('#'))
                path = path.Replace("#", ".html#");
            else
                path += ".html";
        }
        
        if (Window is null)
        {
            CancelSource = new CancellationTokenSource();
            ServerThread = new Thread(() => StartWebServer(CancelSource.Token));
            ServerThread.Start();
            Window = new ViewHelpBrowser();
            Window.Show();
            Window.Closed += CloseHelp;
        }
        else
        {
            Window.Focus();
        }

        Window.URL = URL + path;
    }

    private static void CloseHelp(object? sender, EventArgs eventArgs)
    {
        Window!.Closed -= CloseHelp;
        Window = null;
        CancelSource.Cancel();
        Server.Close();
    }

    private static void StartWebServer(CancellationToken cancelToken)
    {
        Server = new HttpListener();
        Server.Prefixes.Add(URL);
        Server.Start();
        HttpListenerContext context;
        while (true)
        {
            try
            {
                context = Server.GetContext();
            }
            catch (Exception)
            {
                break;
            }

            if (cancelToken.IsCancellationRequested)
                break;

            var response = context.Response;
            var path = context.Request.Url.LocalPath;
            if (path == "/" || string.IsNullOrEmpty(path))
                path = "SisMaper.html";
            path = ".\\Help\\HTML\\" + path;
            try
            {
                var st = response.OutputStream;
                var textRead = true;
                if (path.EndsWith(".html"))
                {
                    response.ContentType = "text/html";
                    textRead = true;
                }
                else if (path.EndsWith(".json"))
                {
                    response.ContentType = "application/json";
                    textRead = true;
                }
                else if (path.EndsWith(".jpg"))
                {
                    response.ContentType = "image/jpg";
                    textRead = false;
                }
                else if (path.EndsWith(".png"))
                {
                    response.ContentType = "image/png";
                    textRead = false;
                }
                else if (path.EndsWith(".gif"))
                {
                    response.ContentType = "image/gif";
                    textRead = false;
                }
                else if (path.EndsWith(".svg"))
                {
                    response.ContentType = "image/svg+xml";
                    textRead = false;
                }
                else if (path.EndsWith(".css"))
                {
                    response.ContentType = "text/css";
                    textRead = true;
                }
                else if (path.EndsWith(".js"))
                {
                    response.ContentType = "application/javascript";
                    textRead = true;
                }
                else if (path.EndsWith(".ttf"))
                {
                    response.ContentType = "application/x-font-ttf";
                    textRead = false;
                }
                else if (path.EndsWith(".otf"))
                {
                    response.ContentType = "application/x-font-opentype";
                    textRead = false;
                }

                else if (path.EndsWith(".woff"))
                {
                    response.ContentType = "application/font-woff";
                    textRead = false;
                }
                else if (path.EndsWith(".woff2"))
                {
                    response.ContentType = "application/font-woff2";
                    textRead = false;
                }
                else { Console.WriteLine(path); }

                if (textRead)
                {
                    TextReader tr = new StreamReader(path);
                    var msg = tr.ReadToEnd();
                    tr.Close();
                    var buffer = Encoding.UTF8.GetBytes(msg);
                    response.ContentLength64 = buffer.Length;
                    st.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    var output = File.ReadAllBytes(path);
                    response.ContentLength64 = output.Length;
                    response.ContentType = "image/png";
                    st.Write(output, 0, output.Length);
                }

                context.Response.Close();
            }
            catch (Exception ex)
            {
                response.StatusCode = (int) HttpStatusCode.NotFound;
                context.Response.Close();
            }
        }
    }
}