using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;

namespace osu_tracker
{
    class Config
    {
        public static string bot_token, api_key;
        public static Dictionary<string, string> sql = new Dictionary<string, string>();

        public Config()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();

            bot_token = config["bot_token"];
            api_key = config["api_key"];

            sql.Add("server", config["sql:server"]);
            sql.Add("port", config["sql:port"]);
            sql.Add("database", config["sql:database"]);
            sql.Add("uid", config["sql:uid"]);
            sql.Add("pwd", config["sql:pwd"]);
            sql.Add("charset", config["sql:charset"]);
        }
    }
}
