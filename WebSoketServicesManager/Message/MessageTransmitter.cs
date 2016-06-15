using Energy.Business;
using Energy.Entity;
using Hammer.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSoketServicesManager.Message
{
    /// <summary>
    /// 抽象消息处理器
    /// </summary>
   public abstract class MessageTransmitter
    {
       /// <summary>
       /// 发送器
       /// </summary>
        public IPush Pust { get; set; }

        public MessageTransmitter(IPush push)
        {
            this.Pust=push;
        }
        public  void ProcessSend(params Iwotp[] list){
            if (list == null || list.Length == 0)
            {
                return;
            }
          IEnumerable<IGrouping<string,Iwotp>>group = list.GroupBy<Iwotp, string>(i => i.ReceiverId);

           foreach (IGrouping<string, Iwotp> i in group)
           {
               Send(i.ToList<Iwotp>().ToArray());
           }
        }

        public abstract void Send(params Iwotp[] list);

    }
}
