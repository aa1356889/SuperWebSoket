using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammer.Cache
{
    /// <summary>
    /// Redis消息队列
    /// </summary>
    public static class WBRedisMq
    {
        private static WBRedisCache cache = new WBRedisCache(3);

        /// <summary>
        /// 消息入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool Push(string key, object value)
        {


            return cache.ListPush(key, value, TimeSpan.FromDays(1200));
        }

        /// <summary>
        /// 消息出队
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Pop<T>(string key) where T : class
        {
            return cache.ListPop<T>(key, TimeSpan.FromDays(1200));
        }


        public static bool RemoveList(string key)
        {
            return cache.RemoveList(key);
        }

        public static List<T> Getlist<T>(string key)
        {
            return cache.GetList<T>(key);
        }

        public static bool HashSet(string hashKey, string key, object value)
        {
            return cache.HashSet(hashKey, key, value);
        }


        public static T HashGet<T>(string hashKey, string key)where T:class
        {
            return cache.HashGet<T>(hashKey, key);
        }

        public static List<T> GetHashList<T>(string hashKey) where T:class
        {
            return cache.HashValues<T>(hashKey);
        }

        public static bool DleteHash(string hashKey, string key)
        {
            return cache.HashDelete(hashKey, key);
        }

    }
}
