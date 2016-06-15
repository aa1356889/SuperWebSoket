using Hammer.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSoketServicesManager.Factory;
using WebSoketServicesManager.Log4;
using WebSoketServicesManager.Message;

namespace WebSoketServicesManager
{
    /// <summary>
    /// 消息监听器
    /// </summary>
   public class MessageMonitor
    {
       IPush webservices = null;
       public MessageMonitor(IPush services)
       {
           this.webservices = services;
       }

       public bool Star()
       {
               for (int i = 0; i < 10; i++)
               {
                   Task.Run(() =>
                   {
                       while (true)
                       {
                           try
                           {

                               System.Threading.Thread.Sleep(5000);
                               var iwotp = WBRedisMq.Pop<Iwotp>(RedisKeyManager.Message);
                              
                               if (iwotp == null)
                               {

                                   continue;
                               }
                               Log4Helper.WriteLog(typeof(MessageMonitor),string.Format("接受到一条消息{0}{1}",iwotp.ReceiverId,iwotp.Content));
                               var trasmiter = MessageProcessFactory.Factory(iwotp.MessageType, webservices);
                               trasmiter.ProcessSend(iwotp);
                           }
                           catch (Exception e)
                           {
                               Log4Helper.WriteLog(typeof(MessageMonitor), e);
                           } 
                          
                       }
                   });
               }
               return true;

       }
    }
}
