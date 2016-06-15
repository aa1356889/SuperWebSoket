using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSoketServicesManager
{
    /// <summary>
    /// 在线用户信息
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string Uid { get; set; }

        /// <summary>
        /// 用户名字
        /// </summary>
        public string UName { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 上线时间
        /// </summary>
        public DateTime Date { get; set; }
    }
}
