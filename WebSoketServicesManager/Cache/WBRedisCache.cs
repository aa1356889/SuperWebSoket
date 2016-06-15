
using Hammer.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using WebSoketServicesManager.ConfigHelper;

namespace Hammer.Cache
{
    /// <summary>
    /// Redis访问类
    /// </summary>
    public class WBRedisCache
    {
        private static ConnectionMultiplexer connection;
        private static ConnectionMultiplexer Connection
        {
            get
            {

                try
                {
                    if (connection == null || !connection.IsConnected)
                    {
                        connection = ConnectionMultiplexer.Connect(string.Format("{0}:{1}", ConfigHelper.GetRedisIp, ConfigHelper.GetRedisPort));
                    }
                    return connection;
                }
                catch (RedisConnectionException ex)
                {
                    throw new Exception("redis链接失败");
                }
            }
        }

        private readonly IDatabase db;

        public WBRedisCache(int dbIndex = 0)
        {
            if (Connection != null)
            {
                db = Connection.GetDatabase(dbIndex);
            }
        }

        public bool Set(string key, object value, bool defaultExpiryStrategy = true, TimeSpan? expiry = null)
        {
            if (db != null && value != null)
            {
                if (defaultExpiryStrategy)
                {
                    return db.StringSet(key, Serialize(value), TimeSpan.FromSeconds(30));
                }
                return db.StringSet(key, Serialize(value), expiry);
            }
            return false;
        }
        public bool Set(Dictionary<string, string> values)
        {
            if (db != null)
            {
                Dictionary<RedisKey, RedisValue> dic = new Dictionary<RedisKey, RedisValue>();
                foreach (var item in values)
                {
                    dic.Add(item.Key, item.Value);
                }
                return db.StringSet(dic.ToArray());
            }
            return false;
        }
        public bool SetString(string key, string value, bool defaultExpiryStrategy = true, TimeSpan? expiry = null)
        {
            if (db != null)
            {
                if (defaultExpiryStrategy)
                {
                    return db.StringSet(key, value, TimeSpan.FromSeconds(30));
                }
                return db.StringSet(key, value, expiry);
            }
            return false;
        }
        public bool SetInt(string key, int value, bool defaultExpiryStrategy = true, TimeSpan? expiry = null)
        {
            if (db != null)
            {
                if (defaultExpiryStrategy)
                {
                    return db.StringSet(key, value, TimeSpan.FromSeconds(30));
                }
                return db.StringSet(key, value, expiry);
            }
            return false;
        }


        public T Get<T>(string key, bool defaultExpiryStrategy = true) where T : class
        {
            if (db != null)
            {
                RedisValue value = db.StringGet(key);
                if (defaultExpiryStrategy)
                {
                    db.KeyExpire(key, TimeSpan.FromSeconds(30));//设置key过期时间，实现相对过期时间的效果
                }
                return Deserialize<T>(value);
            }
            return null;
        }
        public Dictionary<string, T> Get<T>(IEnumerable<string> keys, bool defaultExpiryStrategy = true) where T : class
        {
            if (keys != null && keys.Count() > 0)
            {
                Dictionary<string, T> dic = new Dictionary<string, T>();
                foreach (var item in keys)
                {
                    if (!dic.Keys.Contains(item))
                    {
                        dic.Add(item, Get<T>(item, defaultExpiryStrategy));
                    }
                }
                return dic;
            }
            return null;
        }
        public string GetString(string key, bool defaultExpiryStrategy = true)
        {
            if (db != null)
            {
                RedisValue value = db.StringGet(key);
                if (defaultExpiryStrategy)
                {
                    db.KeyExpire(key, TimeSpan.FromSeconds(30));//设置key过期时间，实现相对过期时间的效果
                }
                return db.StringGet(key);
            }
            return string.Empty;
        }
        public int GetInt(string key, bool defaultExpiryStrategy = true)
        {
            if (db != null)
            {
                RedisValue value = db.StringGet(key);
                if (defaultExpiryStrategy)
                {
                    db.KeyExpire(key, TimeSpan.FromSeconds(30));//设置key过期时间，实现相对过期时间的效果
                }
                return (int)db.StringGet(key);
            }
            return 0;
        }


        public bool Contains(string key)
        {
            if (db != null)
            {
                return db.KeyExists(key);
            }
            return false;
        }
        /// <summary>
        /// 删除指定的key，如果key不存在则忽略
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            if (db != null)
            {
                return db.KeyDelete(key);
            }
            return false;
        }

        /// <summary>
        /// 从队列末尾入队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public bool ListPush(string key, object value, TimeSpan? ts = null)
        {
            bool result = false;
            if (db != null)
            {
                result = db.ListLength(key) < db.ListRightPush(key, SerializeJSON(value));//就算这样也未必就一定插入成功，这个需要研究研究
                if (ts != null)
                {
                    db.KeyExpire(key, ts);
                }
            }
            return result;
        }

        /// <summary>
        /// 获得指定队列里面的所有数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<T> GetList<T>(string key)
        {
            List<T> list = null;
            if (db != null)
            {
                list = new List<T>();
                RedisValue[] values = db.ListRange(key, 0, -1) ?? new RedisValue[0];
                foreach (var item in values)
                {
                    list.Add(DeserializeJSON<T>(item));
                }
            }
            return list;
        }

        public bool RemoveList(string key)
        {
            if (db != null)
            {
                RedisValue[] values = db.ListRange(key, 0, -1) ?? new RedisValue[0];
                foreach (var item in values)
                {
                    db.ListRemove(key, item);
                }
                return true;
            }
            return false;
        }
        /// <summary>
        /// 从队列起始出队
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ts"></param>
        /// <returns></returns>
        public T ListPop<T>(string key, TimeSpan? ts = null) where T : class
        {
            if (db != null)
            {
                if (ts != null)
                {
                    db.KeyExpire(key, ts);
                }
                return DeserializeJSON<T>(db.ListLeftPop(key));
            }
            return null;
        }


        public bool HashSet(string key, string hashField, object value, TimeSpan? ts = null)
        {
            bool result = false;
            if (db != null)
            {
                db.HashSet(key, hashField, Serialize(value));//神啊，反人类啊，这个方法新增一个field值返回true，更新存在的field则返回flase
                if (ts != null)
                {
                    db.KeyExpire(key, ts);
                }
                result = true;
            }
            return result;
        }

        public T HashGet<T>(string key, string hashField, TimeSpan? ts = null) where T : class
        {
            if (db != null)
            {
                if (ts != null)
                {
                    db.KeyExpire(key, ts);
                }
                return Deserialize<T>(db.HashGet(key, hashField));
            }
            return null;

        }
        public bool HashDelete(string key, string hashField)
        {

            if (db != null)
            {

                return db.HashDelete(key, hashField);
            }
            return false;
        }

        public List<T> HashValues<T>(string hashKey) where T : class
        {
            List<T> list = new List<T>();
            RedisValue[] valuse = db.HashValues(hashKey);
            foreach (var item in valuse)
            {
                list.Add(Deserialize<T>(item));
            }
            return list;

        }
        static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }

        static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }
        static string SerializeJSON(object o)
        {

            if (o == null)
            {
                return string.Empty;
            }

            /*
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }*/

            var obj = Newtonsoft.Json.JsonConvert.SerializeObject(o);
            return obj;
        }

        static T DeserializeJSON<T>(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }

            /*

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }*/

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
