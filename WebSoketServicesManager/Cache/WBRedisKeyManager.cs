using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammer.Cache
{
   public class WBRedisKeyManager
    {
       /// <summary>
       /// 总消息队里管理key
       /// </summary>
      public const string Message = "message";

       /// <summary>
       /// 用于记录每个用户的离线信息的key
       /// </summary>
       /// <param name="uid"></param>
       /// <returns></returns>
      public static string IwoUserOffLineMessage(string uid)
      {
          return "Iwo" + uid;
      }
    }
}
