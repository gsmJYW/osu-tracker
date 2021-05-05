using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace osu_tracker
{
    class Sql
    {
        private static string connStr;

        public static void Connect(string server, string port, string database, string uid, string pwd)
        {
            connStr = string.Format
            (
                "Server={0};Port={1};Database={2};Uid={3};Pwd={4};CharSet=utf8;",
                server, port, database, uid, pwd
            );
        }

        public static DataTable Get(string str, params object[] args)
        {
            string query;
            MySqlDataAdapter adpt;
            DataSet ds = new DataSet();

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                query = string.Format(str, args);
                adpt = new MySqlDataAdapter(query, conn);

                if (adpt == null)
                    return new DataTable();

                adpt.Fill(ds, "members");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }

            return ds.Tables[0];
        }

        public static void Execute(string str, params object[] args)
        {
            string query;

            MySqlConnection conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();

                query = string.Format(str, args);
                MySqlCommand command = new MySqlCommand(query, conn);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
