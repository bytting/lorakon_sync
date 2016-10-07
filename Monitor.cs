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
using System.Linq;
using System.Text;

namespace LorakonSync
{
    class Monitor
    {
        FileSystemWatcher monitor = null;
        DateTime lastChange = DateTime.MinValue;
        Settings settings = null;
        ConcurrentQueue<FileEvent> events = null;

        public Monitor(Settings s, ConcurrentQueue<FileEvent> evts)
        {
            settings = s;
            events = evts;

            monitor = new FileSystemWatcher(settings.SourceDirectory, settings.FileFilter);
            monitor.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;            
            monitor.Created += monitor_Created;
            monitor.Changed += monitor_Changed;
            monitor.Renamed += monitor_Renamed;
            monitor.IncludeSubdirectories = true;            
        }

        private void monitor_Created(object sender, FileSystemEventArgs e)
        {            
            FileEvent evt = new FileEvent(FileEventType.Created, e.FullPath, String.Empty);
            events.Enqueue(evt);
        }

        private void monitor_Changed(object sender, FileSystemEventArgs e)
        {
            DateTime lastChangeTime = File.GetLastWriteTime(e.FullPath);
            if (lastChangeTime != lastChange)
            {
                FileEvent evt = new FileEvent(FileEventType.Updated, e.FullPath, String.Empty);
                events.Enqueue(evt);
                lastChange = lastChangeTime;
            }
        }

        private void monitor_Renamed(object sender, RenamedEventArgs e)
        {
            FileEvent evt = new FileEvent(FileEventType.Renamed, e.FullPath, e.OldFullPath);
            events.Enqueue(evt);
        }

        public void Start()
        {
            monitor.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            monitor.EnableRaisingEvents = false;
        }
    }
}
