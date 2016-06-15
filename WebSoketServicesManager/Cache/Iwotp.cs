using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammer.Cache
{
    /// <summary>
    /// iwo MSMQ消息交换协议
    /// </summary>
    [Serializable]
    public class Iwotp
    {
        /// <summary>
        /// 消息发送者ID
        /// </summary>
        public string SenderId { get; set; }
        /// <summary>
        /// 消息接受者GUID
        /// </summary>
        public string ReceiverId { get; set; }

        /// <summary>
        /// 接受者id
        /// </summary>
        public string RID { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Content { get; set; }        
        /// <summary>
        /// 消息产生时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 消息延期处理时间，马上处理则为0，单位：秒
        /// </summary>
        public int DelayProcessSecond { get; set; }
        /// <summary>
        /// 超时时间，为0则消息一直存在
        /// </summary>
          [Newtonsoft.Json.JsonIgnoreAttribute]
        public TimeSpan TimeOut { get; set; }
        /// <summary>
        /// 消息是否需要持久化
        /// </summary>
        public bool HasPersistent { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public string MessageType { get; set; }


    }
}
