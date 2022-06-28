using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace ZatCad
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AppFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Pertinax";
            if (!Directory.Exists(AppFolder)) Directory.CreateDirectory(AppFolder);
#if DEBUG
            FileStream myTraceLog = new FileStream(AppFolder + System.IO.Path.DirectorySeparatorChar + "Zatcad.deb", FileMode.Create, FileAccess.Write, FileShare.Write);
            TextWriterTraceListener myListener = new TextWriterTraceListener(myTraceLog);
            Debug.Listeners.Add(myListener);
            Debug.AutoFlush = true;
            Debug.WriteLine("Start");
#endif
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;

            AppDomain = AppDomain.CreateDomain("ChildDomain");
            Assembly = AppDomain.Load("ConfigOpcUa");
            configOpcUa = (ConfigOpcUa.ConfigOpcUa) Assembly.CreateInstance("ConfigOpcUa.ConfigOpcUa");
        }

        public static string AppFolder { get; set; }
        public static AppDomain AppDomain { get; set; }
        public static Assembly Assembly { get; set; }
        public static ConfigOpcUa.ConfigOpcUa configOpcUa { get; set; }
        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            string msg = $"Exception: {e.Exception.Message}";
            MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Debug.WriteLine($"Exception: {e.Exception.Message}");
            if (e.Exception.InnerException != null)
            {
                Debug.Print($"Inner exception: {e.Exception.InnerException.Message}");
            }
            StackTrace stackTrace = new StackTrace(e.Exception, true);
            for (int i = 0; i < stackTrace.FrameCount; ++i)
            {
                Debug.WriteLine($"  {stackTrace.GetFrame(i).GetFileName()}, {stackTrace.GetFrame(i).GetFileLineNumber()} : {stackTrace.GetFrame(i).GetMethod().Name}");
            }
            e.Handled = true;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Debug.Print("Exit");
            AppDomain.Unload(AppDomain);
        }
    }
}
