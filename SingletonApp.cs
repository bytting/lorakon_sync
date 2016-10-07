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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Threading;

namespace LorakonSync
{
    public static class SingletonApp
    {
        private static Semaphore sem;

        public static bool IsRunning()
        {
            string guid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
            var semName = "Global\\" + guid;
            try 
            {
                sem = Semaphore.OpenExisting(semName, SemaphoreRights.Synchronize);
                Close();
                return true;
            }
            catch (Exception ex) 
            {
                sem = new Semaphore(0, 1, semName);
                return false;
            }
        }

        public static void Close()
        {
            if (sem != null)
            {
                sem.Close();
                sem = null;
            }
        }        
    }
}
