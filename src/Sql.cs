using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace osu_tracker
{
    internal static class Sql
    {
        private static string connStr;

        public static void Connect(string server, string port, string database, string uid, string pwd)
        {
            connStr = $"Server={server};Port={port};Database={database};Uid={uid};Pwd={pwd};CharSet=utf8;";
        }

        public static DataTable Get(string str, params object[] args)
        {
            var ds = new DataSet();
            using var conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                var query = string.Format(str, args);
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var adpt = new MySqlDataAdapter(query, conn);
                
                // INFO: Expression is always false
                // if (adpt == null)
                //    // ReSharper disable once HeuristicUnreachableCode
                //    return new DataTable();

                adpt.Fill(ds, "members");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return ds.Tables[0];
        }

        public static void Execute(string str, params object[] args)
        {
            using var conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                var query = string.Format(str, args);
                // ReSharper disable once HeapView.ObjectAllocation.Evident
                var command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
