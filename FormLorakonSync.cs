/*	
	Lorakon Sync - Synchronizing unique files from one folder to another
    Copyright (C) 2016  Norwegian Radiation Protection Authority

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
// Authors: Dag Robole,

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using CTimer = System.Windows.Forms.Timer;

namespace LorakonSync
{
    public partial class FormLorakonSync : Form
    {
        private ContextMenu trayMenu = null;
        private Settings settings = null;
        private Monitor monitor = null;
        private ConcurrentQueue<FileEvent> events = null;
        private CTimer timer = null;

        public FormLorakonSync(NotifyIcon trayIcon)
        {
            InitializeComponent();

            trayMenu = new ContextMenu();
            trayMenu.MenuItems.Add("Avslutt", OnExit);
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Logg", OnLog);
            trayMenu.MenuItems.Add("Innstillinger", OnSettings);
            trayMenu.MenuItems.Add("-");
            trayMenu.MenuItems.Add("Informasjon", OnAbout);            
            
            trayIcon.Text = "Lorakon Sync";
            trayIcon.Icon = Properties.Resources.LorakonIcon;

            trayIcon.ContextMenu = trayMenu;
            trayIcon.Visible = true;
            
            settings = new Settings();

            events = new ConcurrentQueue<FileEvent>();
            monitor = new Monitor(settings, events);

            timer = new CTimer();            
        }        

        private void FormLorakonSync_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            if (!Directory.Exists(LorakonEnvironment.SettingsPath))
                Directory.CreateDirectory(LorakonEnvironment.SettingsPath);

            Database.AquireDatabase();

            Rectangle rect = Screen.FromControl(this).Bounds;
            Width = (rect.Right - rect.Left) / 2;
            Height = (rect.Bottom - rect.Top) / 2;
            Left = rect.Left + Width / 2;
            Top = rect.Top + Height / 2;

            LoadSettings();
            monitor.Start();

            timer.Interval = 500;
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {            
            while (!events.IsEmpty)
            {
                FileEvent evt;
                if (events.TryDequeue(out evt))
                {
                    if (!File.Exists(evt.FullPath))
                        continue;

                    string sum = FileOps.GetChecksum(evt.FullPath);
                    if (Database.HasChecksum(sum))
                    {
                        lbLog.Items.Add("Checksum [" + sum + "] already synced");
                        continue;
                    }

                    lbLog.Items.Add("Syncing " + evt.FullPath + " [" + sum + "]");
                    SyncFile(evt.FullPath, sum);
                }
            }
        }

        public void SyncFile(string fileName, string checksum)
        {
            Database.InsertChecksum(checksum);
            File.Copy(fileName, settings.DestinationDirectory + Path.DirectorySeparatorChar + checksum + ".cnf", true);
        }

        public void LoadSettings()
        {
            if (!File.Exists(LorakonEnvironment.SettingsFile))
                return;

            // Deserialize settings from file
            using (StreamReader sr = new StreamReader(LorakonEnvironment.SettingsFile))
            {
                XmlSerializer x = new XmlSerializer(settings.GetType());
                settings = x.Deserialize(sr) as Settings;
            }
        }

        private void SaveSettings()
        {
            // Serialize settings to file
            using (StreamWriter sw = new StreamWriter(LorakonEnvironment.SettingsFile))
            {
                XmlSerializer x = new XmlSerializer(settings.GetType());
                x.Serialize(sw, settings);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            if (MessageBox.Show("Er du sikker på at du vil stoppe synkronisering av Lorakon filer?", "Informasjon", MessageBoxButtons.YesNo) == DialogResult.No)
                return;

            monitor.Stop();
            timer.Stop();
            Database.ReleaseDatabase();
            Application.Exit();
        }

        private void OnAbout(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            Visible = true;
            tabs.SelectedTab = pageAbout;
        }

        private void OnSettings(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            Visible = true;
            tabs.SelectedTab = pageSettings;
        }

        private void OnLog(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;
            Visible = true;
            tabs.SelectedTab = pageLog;
        }

        private void FormLorakonSync_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown || e.CloseReason == CloseReason.ApplicationExitCall || e.CloseReason == CloseReason.TaskManagerClosing)
                return;
            
            e.Cancel = true;            
            Hide();                
        }
    }
}
