using Hammer.Cache;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSoketServicesManager.Log4;
using WebSoketServicesManager.Tool;
using WebSoketServicesManager.ConfigHelper;
namespace WebSoketServicesManager
{

  
    public partial class FormManager : Form
    {
        public WebSoketServices services = null;
        public FormManager()
        {
            InitializeComponent();
            services = new WebSoketServices(ConfigHelper.ConfigHelper.GetServiceIp, ConfigHelper.ConfigHelper.GetServicePort);
         
            services.CloseCallbCack += services_CloseCallbCack;
            services.LoginCallBack += services_LoginCallBack;
            if (services.Start())
            {
                AppedMessage("服务器已经启动" + DateTime.Now.ToString());
            }
            else
            {



                AppedMessage("服务器启动失败" + DateTime.Now.ToString());
            }
        }

        void services_LoginCallBack(User obj)
        {
            AppedMessage(obj.UName + "已经上线," + DateTime.Now.ToString());
            AddItems(obj);
        }

        void services_CloseCallbCack(User obj)
        {
            AppedMessage(obj.UName + "已经离线，" + DateTime.Now.ToString());
            RemoveItems(obj);
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
        
        }
        public void AppedMessage(string text)
        {

            lock (lockobj)
            {
                if (this.txt_Message.Text.Length >= 1000)
                {

                    this.txt_Message.Text = string.Empty;
                }
                this.txt_Message.Text += text + "\r\n";
            }
        }

        public void AddItems(User user) {

            ListViewItem item = new ListViewItem(new string[] { user.Uid, user.UName, DateTime.Now.ToString() });
;            this.lv_Users.Items.Add(item);
        }
        object lockobj = new object();
        public void RemoveItems(User user)
        {
            lock (lockobj)
            {
                for (int i = 0; i < this.lv_Users.Items.Count; i++)
                {
                    if (this.lv_Users.Items[i].Text == user.Uid)
                    {
                        this.lv_Users.Items.RemoveAt(i);
                    }
                }
            }
        }

        private void btn_send_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(this.txt_SendMessage.Text.Trim()))
            {
                 Iwotp tp = new Iwotp() { Content =this.txt_SendMessage.Text, CreateDate = DateTime.Now, HasPersistent =true, MessageType = IwotpMessageTypeEnum.Broadcast};
                 WBRedisMq.Push(WBRedisKeyManager.Message, tp); 
            }
            else
            {
                MessageBox.Show("广播内容不能为空");
            }
        }
       

    }
}
