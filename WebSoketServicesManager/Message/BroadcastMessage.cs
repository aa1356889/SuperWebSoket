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
    /// 广播消息
    /// </summary>
    public class BroadcastMessage : MessageTransmitter
    {
        public BroadcastMessage(IPush push) : base(push) { }


        public override void Send(params Iwotp[] list)
        {
            if (list == null || list.Length == 0)
            {
                return;
            }
            //非群发
            if (!string.IsNullOrEmpty(list[0].ReceiverId))
            {
                int maxValue = 5;
                System.Text.StringBuilder messagesb = new System.Text.StringBuilder(40);
                messagesb.Append("<div style='width:100%;text-align:center'>更新公告</div><br/>");
                if (list.Length > maxValue)
                {
                    messagesb.Append(string.Format("<div style='width:100%;text-align:center'>您当前拥有消息{0},仅显示{1}条更多请前往消息中心查看<a href=http://localhost:8088/Admin/MessageCenter/List>点击跳转</a></div>", list.Length, maxValue));
                }
                int length = maxValue > list.Length ? list.Length : maxValue;
                for (int i = 0; i < length; i++)
                {
                    messagesb.Append("<br/>").Append(list[i].Content).Append("<br/><br/><br/>");
                } var messageCenters = list.Select<Iwotp, MessageCenter>(c => new MessageCenter() { Content = c.Content, ReceiveDateTime =c.CreateDate, ReceiveUserID = list[0].ReceiverId, SendUserID = c.SenderId }).ToList<MessageCenter>();
                MessageCenterBusiness.BachAddMessageCenter(messageCenters);
                messagesb.Append("</div>");
                int nocount = MessageCenterBusiness.GetNoReadCountByUserGuid(list[0].ReceiverId);
                string jsonStr = "{\"State\":0, \"Message\":\"" + messagesb.ToString() + "\",\"Data\":\"" + nocount + "\"}";
                Pust.Send(list[0].ReceiverId, jsonStr);

            }//群发
            else
            {

                var users = Energy.Business.UsersBusiness.GetStaff(string.Empty, string.Empty, string.Empty);
                foreach (var user in users)
                {

                    if (Pust.Contains(user.RecordID))
                    {
                        var messageCenters = list.Select<Iwotp, MessageCenter>(c => new MessageCenter() { Content = c.Content, ReceiveDateTime = DateTime.Now, ReceiveUserID = user.RecordID, SendUserID = c.SenderId }).ToList<MessageCenter>();
                        int maxValue = 5;
                        System.Text.StringBuilder messagesb = new System.Text.StringBuilder(40);
                        messagesb.Append("<div style='width:100%;text-align:center'>更新公告</div><br/>");
                        if (list.Length > maxValue)
                        {
                            messagesb.Append(string.Format("<div style='width:100%;text-align:center'>您当前拥有消息{0},仅显示{1}条更多请前往消息中心查看<a href=http://localhost:8088/Admin/MessageCenter/List>点击跳转</a></div>", list.Length, maxValue));
                        }
                        int length = maxValue > list.Length ? list.Length : maxValue;

                        MessageCenterBusiness.BachAddMessageCenter(messageCenters);
                        for (int i = 0; i < length; i++)
                        {
                            messagesb.Append("<br/>").Append(list[i].Content).Append("<br/><br/><br/>");
                        }
                        messagesb.Append("</div>");
                        int nocount = MessageCenterBusiness.GetNoReadCountByUserGuid(user.RecordID);
                        string jsonStr = "{\"State\":0, \"Message\":\"" + messagesb.ToString() + "\",\"Data\":\"" + nocount + "\"}";
                        Pust.Send(user.RecordID, jsonStr);
                    }
                    else
                    {

                        foreach (var item in list)
                        {
                            item.ReceiverId = user.RecordID;//指定该广播消息的接收人
                            WBRedisMq.Push(RedisKeyManager.IwoUserOffLineMessage(user.RecordID), item);
                        }

                    }
                }
            }


        }
    }
}
