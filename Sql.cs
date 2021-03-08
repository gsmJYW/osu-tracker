using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace osu_tracker
{
    class Sql
    {
        public static MySqlConnection conn;

        public Sql(string server, string port, string database, string uid, string pwd, string charset)
        {
            string connStr = string.Format
            (
                "Server={0};Port={1};Database={2};Uid={3};Pwd={4};CharSet={5}",
                server, port, database, uid, pwd, charset
            );

            conn = new MySqlConnection(connStr);

            // SQL 접속 시도 (실패 시 예외)
            conn.Open();
            conn.Close();
        }

        public static DataTable Get(string str, params object[] args)
        {
            string query;
            MySqlDataAdapter adpt;
            DataSet ds = new DataSet();

            try
            {
                query = string.Format(str, args);
                conn.Open();

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

            try
            {
                query = string.Format(str, args);
                conn.Open();

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
