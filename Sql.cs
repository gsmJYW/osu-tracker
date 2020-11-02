using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;

namespace osu_tracker
{
    class Sql
    {
        public static MySqlConnection conn;

        public Sql()
        {
            Dictionary<string, string> sql = Config.sql;
            
            string connStr = string.Format
            (
                "Server={0};Port={1};Database={2};Uid={3};Pwd={4};CharSet={5}",
                sql["server"], sql["port"], sql["database"], sql["uid"], sql["pwd"], sql["charset"]
            );

            conn = new MySqlConnection(connStr);

            try
            {
                conn.Open();
                conn.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
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

                if (command.ExecuteNonQuery() != 1)
                {
                    throw new Exception("failed to execute: " + query);
                }
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
