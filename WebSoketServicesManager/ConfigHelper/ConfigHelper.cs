using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSoketServicesManager.ConfigHelper
{
   public class ConfigHelper
    {
       public static string Get(string key)
       {
           return ConfigurationManager.AppSettings[key];
       }
       public static string GetPassWord { get { return Get("passWord"); } }

       public static string GetServiceIp { get { return Get("ServiceIp"); } }
       public static int GetServicePort { get { return Convert.ToInt32(Get("ServicesPort")); } }

       public static string GetRedisIp { get { return Get("redisIp"); } }
       public static int GetRedisPort { get { return Convert.ToInt32(Get("redisPort")); } }

    }
}
