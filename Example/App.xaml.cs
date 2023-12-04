using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Example
{
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message + "\n" + e.Exception.StackTrace, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            App.Current.Shutdown(1);
        }
    }
}
