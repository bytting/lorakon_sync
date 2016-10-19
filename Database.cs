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
    public static class Database
    {        
        public static SQLiteConnection CreateConnection()
        {            
            SQLiteConnection conn = new SQLiteConnection("Data Source=" + LorakonEnvironment.DatabaseFile + ";Version=3;Compress=True;");            

            if (!File.Exists(LorakonEnvironment.DatabaseFile))
            {                
                SQLiteConnection.CreateFile(LorakonEnvironment.DatabaseFile);                
                SQLiteCommand cmd = new SQLiteCommand("create table sync_objects (checksum char(64))", conn);
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            return conn;
        }

        public static void OpenConnection(SQLiteConnection conn)
        {
            if (conn != null && conn.State != ConnectionState.Open)
                conn.Open();
        }

        public static void CloseConnection(ref SQLiteConnection conn)
        {
            if (conn != null && conn.State == ConnectionState.Open)
                conn.Close();            
        }

        public static bool InsertChecksum(SQLiteConnection conn, string cs)
        {                            
            SQLiteCommand cmd = new SQLiteCommand("insert into sync_objects (checksum) values('" + cs + "')", conn);                
            cmd.ExecuteNonQuery();
            return false;
        }

        public static bool HasChecksum(SQLiteConnection conn, string cs)
        {            
            SQLiteCommand cmd = new SQLiteCommand("select count(*) from sync_objects where checksum like '" + cs + "'", conn);                                
            object o = cmd.ExecuteScalar();
            if (o == null || o == DBNull.Value)
                return false;
            int nRows = Convert.ToInt32(o);
            return nRows > 0;
        }
    }
}
