using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace OpcUaExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar + "Pertinax";
            if (!Directory.Exists(appFolder)) Directory.CreateDirectory(appFolder);
#if DEBUG
            FileStream myTraceLog = new FileStream(appFolder + System.IO.Path.DirectorySeparatorChar + "OpcUaExplorer.deb", FileMode.Create);
            TextWriterTraceListener myListener = new TextWriterTraceListener(myTraceLog);
            Debug.Listeners.Add(myListener);
            Debug.AutoFlush = true;
            Debug.WriteLine("Start");
#endif
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
        }

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
            e.Handled = false;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
//            MainViewModel mvm = ServiceLocator.Current.GetInstance<MainViewModel>();
//            SettingsViewModel svm = ServiceLocator.Current.GetInstance<SettingsViewModel>();
//            Settings.Default.Startup = svm.StartupFileName;
//            Settings.Default.RepetitiveRate = svm.RepetitiveRate;
//            Settings.Default.IsTls = svm.IsTls;
            //            Settings.Default.TargetLeft = mvm.SelectedTargetLeft;
            //            Settings.Default.TargetRight = mvm.SelectedTargetRight;
//            Settings.Default.Save();
        }
    }
}
