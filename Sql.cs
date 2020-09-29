using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace osu_tracker
{
    class Sql
    {
        public static string server = ""; // 서버 주소
        public static string port = ""; // 포트 번호
        public static string database = "osutracker"; // db 이름
        public static string uid = ""; // 유저 id
        public static string pwd = ""; // 패스워드
        public static string charset = "utf8";

        public static string connStr = "Server=" + server + ";Port=" + port + ";Database=" + database + ";Uid=" + uid + ";Pwd=" + pwd + ";CharSet=" + charset;
        public static MySqlConnection conn = new MySqlConnection(connStr);

        public static void Connect()
        {
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
                    throw new Exception(query + "\n실행 실패");
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
