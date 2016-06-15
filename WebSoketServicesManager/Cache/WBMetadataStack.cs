using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections.Concurrent;
namespace Hammer.Cache
{
    /// <summary>
    ///  存储实体对象的元数据
    /// </summary>
    public class WBMetadataStack
    {
        #region  获取单例
        private WBMetadataStack()
        { }

        private static WBMetadataStack _metadataStack = null;
        private static Object Lock = new object();
        public static WBMetadataStack GetMetadataStack
        {
            get
            {
                if (_metadataStack == null)
                {
                    lock (Lock)
                    {
                        if (_metadataStack == null)
                        {
                            _metadataStack = new WBMetadataStack();
                        }
                    }
                }
                return _metadataStack;
            }
        }
        #endregion

        private static string attributekeyformat = "{0}.{1}";
        private ConcurrentDictionary<string, List<PropertyInfo>> propertyList = new ConcurrentDictionary<string, List<PropertyInfo>>();
        private ConcurrentDictionary<string, List<Attribute>> attributeList = new ConcurrentDictionary<string, List<Attribute>>();
        /// <summary>
        /// 当前缓存的所有属性信息
        /// </summary>
        public ConcurrentDictionary<string, List<PropertyInfo>> PropertyList
        {
            get
            {
                return propertyList;
            }
        }

        /// <summary>
        ///  当前缓存的所有特性信息
        /// </summary>
        public ConcurrentDictionary<string, List<Attribute>> AttributeList
        {
            get
            {
                return attributeList;
            }
        }

        public List<PropertyInfo> this[string TypeFullName]
        {
            get
            {
                if (this.PropertyList.Keys.Contains(TypeFullName))
                {
                    return this.PropertyList[TypeFullName];
                }
                return null;
            }
        }


        /// <summary>
        ///  根据一个属性信息获取它的特性集合
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public List<Attribute> this[PropertyInfo p]
        {
            get
            {
                string key = string.Format(attributekeyformat, p.DeclaringType.FullName, p.Name);
                if (!attributeList.Keys.Contains(key))
                {
                    return null;
                }

                return attributeList[key];
            }
        }

        /// <summary>
        ///  获取一个对象的特性集合
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public List<Attribute> this[Type t]
        {
            get
            {

                if (!attributeList.Keys.Contains(t.FullName))
                {
                    return null;
                }
                return attributeList[string.Format(t.FullName)];
            }
        }
        
    }
}
