using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSoketServicesManager.Message;

namespace WebSoketServicesManager.Factory
{
    /// <summary>
    /// 消息处理器工厂类
    /// </summary>
   public class MessageProcessFactory
    {
       public static MessageTransmitter Factory(string type,IPush push)
       {
           MessageTransmitter mti = null;
           switch (type)
           {
               case "Offline":
                   mti = new OfflineMessage(push);
                   break;
               case "Online":
                   mti = new OnlineMessage(push);
                   break;
               case "Broadcast":
                   mti = new BroadcastMessage(push);
                   break;
               default:
                   mti = new OnlineMessage(push);
                   break;

           }
           return mti;
       }
    }
}
