using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LorakonSync
{
    public enum FileEventType { Created, Renamed, Updated };

    public class FileEvent
    {
        public FileEvent(FileEventType t, string fullPath, string oldFullPath)
        {
            EventType = t;
            FullPath = fullPath;
            OldFullPath = oldFullPath;
        }

        public FileEventType EventType { get; set; }
        public string FullPath { get; set; }
        public string OldFullPath { get; set; }
    }
}
