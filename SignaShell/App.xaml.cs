using System;
using System.Linq;
using System.Windows;
using ZetaIpc.Runtime.Server;

namespace SignaShell
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var s = new IpcServer();
            s.Start(12345); 

            MainWindow mainWindow = new MainWindow();            
            mainWindow.Show();

            s.ReceivedRequest += (initiator, args) =>
            {                  
                Dispatcher.Invoke(new Action(() => 
                {
                    var windowExists = Application.Current.Windows.OfType<Window>().Any(w => w.GetType().FullName == args.Request);

                    if(!windowExists)
                    {
                        var window = (Window)Activator.CreateInstance(Type.GetType(args.Request));
                        window.Show();
                    }

                    var windows = Application.Current.Windows.OfType<Window>();                    
                    foreach(var window in windows)
                    {
                        if(window.GetType().FullName != args.Request)
                        {
                            window.Close();
                        }
                    }
                }));
            };
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
        }
    }
}
