
using Hammer.Cache;
using SuperSocket.SocketBase.Config;
using SuperWebSocket;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebSoketServicesManager.Factory;
using WebSoketServicesManager.Log4;
using WebSoketServicesManager.Message;
using WebSoketServicesManager.Tool;
namespace WebSoketServicesManager
{
    public class WebSoketServices : IPush
    {
        /// <summary>
        /// 授权密码
        /// </summary>
        public static string password = ConfigurationManager.AppSettings["passWord"];
        public UsersManager users { get; set; }
        public WebSocketServer services { get; set; }

        private MessageMonitor monitor { get; set; }

        public event Action<User> CloseCallbCack;
        public event Action<User> LoginCallBack;
        public WebSoketServices(string ip, int port)
        {
            users = new UsersManager();
            services = new WebSocketServer();
            ServerConfig config = new ServerConfig()
            {
                Ip = ip,
                Port = port,
                MaxConnectionNumber = 10000
            };
            services.Setup(config);
            services.NewSessionConnected += services_NewSessionConnected;
            services.NewMessageReceived += services_NewMessageReceived;
            services.SessionClosed += services_SessionClosed;
            monitor = new MessageMonitor(this);//消息监听器
        }

        void services_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            NameValueCollection n = Path.QueryString(session.Path);
            if (!Gave(n)) return;//没有权限退出
            User user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(n["userInfo"]);
            if (users.Remove(user.Uid, session.SessionID) && !users.Contains(user.Uid))
            {
                CloseCallbCack(user);
            }
        }

        void services_NewMessageReceived(WebSocketSession session, string value)
        {
        }

        void services_NewSessionConnected(WebSocketSession session)
        {


            NameValueCollection n = Path.QueryString(session.Path);
            if (!Gave(n)) return;//没有权限退出
            User user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(n["userInfo"]);
            if (!users.Contains(user.Uid))
            {
                LoginCallBack(user);
            }
            string key = WBRedisKeyManager.IwoUserOffLineMessage(user.Uid);
            user.Address = session.SessionID;
            users.Add(user);
            try
            {

                var list = WBRedisMq.Getlist<Iwotp>(key);






                if (list != null && list.Count > 0)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        IEnumerable<IGrouping<string, Iwotp>> group = list.GroupBy<Iwotp, string>(i => i.MessageType);

                        foreach (IGrouping<string, Iwotp> i in group)
                        {
                            var trasmiter = MessageProcessFactory.Factory(i.Key, this); ;
                            trasmiter.ProcessSend(i.ToList<Iwotp>().ToArray());
                        }


                    });
                }
            }

            catch (Exception e)
            {
                Log4Helper.WriteLog(typeof(WebSoketServices), e);
            }
            finally
            {
                WBRedisMq.RemoveList(key);
            }



        }
        /// <summary>
        /// 是否授权
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool Gave(NameValueCollection values)
        {

            if (values == null | values.Count < 2)
            {
                return false;
            }
            bool index = false;
            foreach (var item in values.AllKeys)
            {
                if (item == "password")
                {
                    index = true;
                    break;
                }
            }
            if (index == false) return false;
            List<string> strs = new List<string>();
            for (int i = 0; i < values.Count; i++)
            {
                if (values.AllKeys[i] != "password")
                {
                    strs.Add(values[i]);
                }
            }
            strs.Add(password);
            string md5passWord = MD5.Encode(strs.ToArray());
            return md5passWord == values["password"];
        }
        public void Send(Iwotp iwotp)
        {
            if (users.Contains(iwotp.ReceiverId))
            {
                foreach (var item in users.usersAddress[iwotp.ReceiverId])
                {

                    var session = services.GetSessions(c => c.SessionID == item).FirstOrDefault();
                    if (session != null)
                    {
                        var trasmiter = new OnlineMessage(this);
                        trasmiter.ProcessSend(iwotp);
                    }
                }
            }
            else
            {
                WBRedisMq.Push(RedisKeyManager.IwoUserOffLineMessage(iwotp.ReceiverId), iwotp);
            }
        }
        public void Send(string uid, string message)
        {
            foreach (var item in users.usersAddress[uid])
            {

                var session = services.GetSessions(c => c.SessionID == item).FirstOrDefault();
                if (session != null)
                {
                    session.Send(message);
                }
            }
        }
        public void SendAll(string message)
        {
            var sessinos = services.GetAllSessions();
            foreach (var item in sessinos)
            {
                item.Send("{\"State\":0, \"Message\":\"" + message + "\",\"Data\":\"\"}");
            }
        }

        public bool Contains(string uid)
        {
            return users.Contains(uid);
        }
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {

            return services.Start() && monitor.Star();
        }


    }
}
