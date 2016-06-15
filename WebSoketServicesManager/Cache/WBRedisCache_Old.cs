using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hammer.Cache
{
    /// <summary>
    ///  Redis访问类
    /// </summary>
    public class WBRedisCache_Old
    {
        private string Ip;
        private string Port;
        private int maxWritePoolSize;
        private int maxReadPoolSize;
        private int dbIndex; //数据库序号，Redis只支持以数字标识数据库，最多16个。可在Redis配置参数中设置
        private PooledRedisClientManager prcm = null;//CreateManager(new string[] { "10.0.4.210:6379" }, new string[] { "10.0.4.210:6379" });

        public WBRedisCache_Old(string ip, string port, int dbIndex = 0)
        {
            this.Ip = ip;
            this.Port = port;
            this.dbIndex = dbIndex;
            this.maxWritePoolSize = 40;
            this.maxReadPoolSize = 260;
            Start();
        }

        private void Start()
        {
            string[] host = new string[] { this.Ip + ":" + this.Port };
            prcm = new PooledRedisClientManager(host, host, new RedisClientManagerConfig
            {
                MaxReadPoolSize = maxReadPoolSize, //“读”链接池链接数
                MaxWritePoolSize = maxWritePoolSize, //写”链接池链接数
                AutoStart = true,
                DefaultDb = this.DbIndex  //默认连接数据库
            });
        }

        //模糊搜索一个key，并获取完整的Key
        public List<string> SearchKey(string key)
        {
            List<string> result = null;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.SearchKeys(key);
            }
            catch (Exception ex)
            {
                result = null;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }
            if (result != null)
            {
                if (result.Count == 0)
                {
                    result = null;
                }
            }

            return result;
        }

        public bool ContainsKey(string key)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.ContainsKey(key);
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public bool SetKeys(Dictionary<string, string> map)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                redis.SetAll(map);
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public bool Set<T>(string key, T value)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                redis.Set<T>(key, value);
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public bool Set<T>(string key, T value, DateTime dt)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                redis.Set<T>(key, value, dt);
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public bool Set<T>(string key, T value, TimeSpan ts)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                redis.Set<T>(key, value, ts);
            }
            catch (Exception ex)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public object Get(string key)
        {
            object result = null;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.Get<object>(key);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }
            return result;

        }

        public T Get<T>(string key)
        {
            T result = default(T);
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.Get<T>(key);
            }
            catch (Exception e)
            {

            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }
            return result;
        }

        public IDictionary<string, object> GetAll(IEnumerable<string> keys)
        {
            IDictionary<string, object> result = null;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.GetAll<object>(keys);
            }
            catch (Exception e) { }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public IDictionary<string, T> GetAll<T>(IEnumerable<string> keys)
        {
            IDictionary<string, T> result = null;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.GetAll<T>(keys);
            }
            catch (Exception e) { }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }
            return result;
        }

        public bool Delete(string key)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.Remove(key);
            }
            catch (Exception e)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public bool Delete(IEnumerable<string> keys)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                redis.RemoveAll(keys);
            }
            catch (Exception e)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public bool SetTime(string key, DateTime dt)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.ExpireEntryAt(key, dt);
            }
            catch (Exception e)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public bool SetTime(string key, TimeSpan ts)
        {
            bool result = true;
            IRedisClient redis = null;
            try
            {
                redis = prcm.GetClient();
                result = redis.ExpireEntryIn(key, ts);
            }
            catch (Exception e)
            {
                result = false;
            }
            finally
            {
                if (redis != null)
                {
                    redis.Dispose();
                }
            }

            return result;
        }

        public int MaxWritePoolSize
        {
            get
            {
                return maxWritePoolSize;
            }
            set
            {
                maxWritePoolSize = value;
            }
        }

        public int MaxReadPoolSize
        {
            get
            {
                return maxReadPoolSize;
            }
            set
            {
                maxReadPoolSize = value;
            }
        }

        public int DbIndex
        {
            get
            {
                return dbIndex;
            }
            set
            {
                dbIndex = value;
            }
        }

    }
}
