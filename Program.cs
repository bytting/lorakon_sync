using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace LorakonSync
{
    static class Program
    {
        [STAThread]
        static void Main()
        {            
            if (SingletonApp.IsRunning())
            {
                MessageBox.Show("Lorakon Sync kjører allerede");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (NotifyIcon trayIcon = new NotifyIcon())
            {
                FormLorakonSync app = new FormLorakonSync(trayIcon);                
                Application.Run(app);
            }

            SingletonApp.Close();
        }
    }
}
