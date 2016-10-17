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
using System.Data;
using System.Data.SQLite;
using CTimer = System.Windows.Forms.Timer;

namespace LorakonSync
{
    public partial class FormLorakonSync : Form
    {
        private ContextMenu trayMenu = null;
        private Settings settings = null;
        private Monitor monitor = null;
        private ConcurrentQueue<FileEvent> events = null;
        SQLiteConnection connection = null;
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
        }        

        private void FormLorakonSync_Load(object sender, EventArgs e)
        {
            Visible = false;
            ShowInTaskbar = false;

            // Set default window layout
            Rectangle rect = Screen.FromControl(this).Bounds;
            Width = (rect.Right - rect.Left) / 2;
            Height = (rect.Bottom - rect.Top) / 2;
            Left = rect.Left + Width / 2;
            Top = rect.Top + Height / 2;

            // Create environment and load settings
            if (!Directory.Exists(LorakonEnvironment.SettingsPath))
                Directory.CreateDirectory(LorakonEnvironment.SettingsPath);            
            settings = new Settings();
            LoadSettings();

            events = new ConcurrentQueue<FileEvent>();            
            connection = Database.CreateConnection();
            
            // Sync files that has been created after last shutdown and has not been synced before
            Database.OpenConnection(connection);
            foreach (string fname in Directory.EnumerateFiles(settings.SourceDirectory, "*.cnf", SearchOption.AllDirectories))
            {
                DateTime dt = File.GetCreationTime(fname);
                if (dt.CompareTo(settings.LastShutdownTime) < 0)
                {
                    lbLog.Items.Add("Skipping " + fname + ", older than last shutdown");
                    continue;
                }

                string sum = FileOps.GetChecksum(fname);
                if (!Database.HasChecksum(connection, sum))
                {
                    lbLog.Items.Add("Syncing " + fname + " [" + sum + "]");
                    SyncFile(connection, fname, sum);
                }
            }
            Database.CloseConnection(ref connection);

            // Start timer for processing file events
            timer = new CTimer();
            timer.Interval = 500;
            timer.Tick += timer_Tick;
            timer.Start();

            // Start monitoring file events
            monitor = new Monitor(settings, events);
            monitor.Start();
        }

        void timer_Tick(object sender, EventArgs e)
        {            
            while (!events.IsEmpty)
            {
                FileEvent evt;
                if (events.TryDequeue(out evt))
                {
                    if (!File.Exists(evt.FullPath)) // This happens when the same event are reported more than once
                        continue;
                    
                    string sum = FileOps.GetChecksum(evt.FullPath);

                    Database.OpenConnection(connection);
                    if (!Database.HasChecksum(connection, sum))                    
                    {
                        lbLog.Items.Add("Syncing " + evt.FullPath + " [" + sum + "]");
                        SyncFile(connection, evt.FullPath, sum);
                    }
                    else
                    {
                        lbLog.Items.Add("File " + evt.FullPath + " is already synced");
                    }
                    Database.CloseConnection(ref connection);
                }
            }
        } 
       
        private void SyncFile(SQLiteConnection conn, string filename, string checksum)
        {
            File.Copy(filename, settings.DestinationDirectory + Path.DirectorySeparatorChar + checksum + ".cnf", true);
            Database.InsertChecksum(connection, checksum);
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

            settings.LastShutdownTime = DateTime.Now;
            SaveSettings();

            monitor.Stop();
            timer.Stop();            
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
