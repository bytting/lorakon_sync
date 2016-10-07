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
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

namespace LorakonSync
{
    public class Database
    {
        private static SQLiteConnection connection;
        private static SQLiteCommand command;        

        public static bool AquireDatabase()
        {
            connection = new SQLiteConnection("Data Source=" + LorakonEnvironment.DatabaseFile + ";Version=3;Compress=True;");
            command = new SQLiteCommand(connection);

            if (!File.Exists(LorakonEnvironment.DatabaseFile))
            {
                try
                {
                    SQLiteConnection.CreateFile(LorakonEnvironment.DatabaseFile);
                    connection.Open();
                    command.CommandText = "create table sync_objects (checksum char(64))";
                    command.ExecuteNonQuery();
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }

            return true;
        }

        public static bool ReleaseDatabase()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
            return true;
        }

        public static bool InsertChecksum(string cs)
        {
            try
            {
                command.CommandText = "insert into sync_objects (checksum) values('" + cs + "')";
                connection.Open();
                command.ExecuteNonQuery();
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return false;
        }

        public static bool HasChecksum(string cs)
        {
            try
            {
                connection.Open();                
                command.CommandText = "select count(*) from sync_objects where checksum like '" + cs + "'";
                object o = command.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                    return false;
                int nRows = Convert.ToInt32(o);
                return nRows > 0;
            }            
            finally
            {
                if(connection.State == ConnectionState.Open)
                    connection.Close(); 
            }                        
        }
    }
}
