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
    /// 在线消息处理器
    /// </summary>
  public  class OnlineMessage:MessageTransmitter
    {
      public OnlineMessage(IPush push)
           : base(push)
       {

       }

      public override void Send(params Iwotp[] list)
      {
          if (list != null && list.Length > 0)
          {
              if (Pust.Contains(list[0].ReceiverId))
              {
                  var messageCenters = list.Select<Iwotp, MessageCenter>(c => new MessageCenter() { Content = c.Content, ReceiveDateTime =c.CreateDate, ReceiveUserID = c.ReceiverId, SendUserID = c.SenderId }).ToList<MessageCenter>();
                  if (!MessageCenterBusiness.BachAddMessageCenter(messageCenters)) return;//物理化失败则返回
                  int maxValue = 5;

                  System.Text.StringBuilder messagesb = new System.Text.StringBuilder(40);
                  messagesb.Append("<div style='width:100%;text-align:center'>在线消息</div>");
                  if (list.Length > maxValue)
                  {
                      messagesb.Append(string.Format("<div style='width:100%;text-align:center'>您当前拥有消息{0},仅显示{1}条更多请前往消息中心查看<a href=http://localhost:8088/Admin/MessageCenter/List>点击跳转</a></div>", list.Length, maxValue));
                  }
                  int length = maxValue > list.Length ? list.Length : maxValue;
                  for (int i = 0; i < length; i++)
                  {
                      messagesb.Append("<br/>").Append(list[i].Content).Append("<br/><br/><br/>");
                  }
                  messagesb.Append("</div>");
                  int nocount = MessageCenterBusiness.GetNoReadCountByUserGuid(list[0].ReceiverId);
                  string jsonStr = "{\"State\":0, \"Message\":\"" + messagesb.ToString() + "\",\"Data\":\"" + nocount + "\"}";
                  Pust.Send(list[0].ReceiverId, jsonStr);
              }
              else
              {
                  foreach (var item in list)
	{
        item.MessageType = IwotpMessageTypeEnum.Offline;
        WBRedisMq.Push(WBRedisKeyManager.IwoUserOffLineMessage(item.ReceiverId), item);
	}
               
              }
          }
      }
    }
}
