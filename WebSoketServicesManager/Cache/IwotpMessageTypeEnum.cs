using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSoketServicesManager
{
  public class IwotpMessageTypeEnum
    {
      /// <summary>
      /// 离线消息
      /// </summary>
      public static string Offline { get { return "Offline"; } }


      /// <summary>
      /// 在线消息
      /// </summary>
      public static string Online { get { return "Online"; } }


      /// <summary>
      /// 广播消息
      /// </summary>
      public static string Broadcast { get { return "Broadcast"; } }
    }
}
