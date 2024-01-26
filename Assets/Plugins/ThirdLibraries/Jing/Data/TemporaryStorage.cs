using System.Collections;
using System.Collections.Generic;

namespace Jing
{
    /// <summary>
    /// 临时数据存储器，存入的数据被Get一次后就会清除。
    /// </summary>
    public class TemporaryStorage : TemporaryStorage<object>
    {

    }

    /// <summary>
    /// 临时数据存储器，存入的数据被Get一次后就会清除。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TemporaryStorage<T>
    {
        Dictionary<string, T> _dic = new Dictionary<string, T>();

        /// <summary>
        /// 数据的数量
        /// </summary>
        public virtual int Count => _dic.Count;

        /// <summary>
        /// 设置数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public virtual void Set(string key, T value)
        {
            _dic[key] = value;
        }

        /// <summary>
        /// 获取数据，可以获得Key对应的Value，数据会在返回后，从存储器中清除。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T Get(string key)
        {
            if (_dic.ContainsKey(key))
            {
                var value = _dic[key];
                _dic.Remove(key);
                return value;
            }

            return default(T);
        }

        /// <summary>
        /// 检查数据，可以获得Key对应的Value，但是数据并不会清除。
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual T Check(string key)
        {
            if (_dic.ContainsKey(key))
            {
                return _dic[key];
            }
            return default(T);
        }

        /// <summary>
        /// 获取所有临时存储数据的KEY
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetKeys()
        {
            string[] keys = new string[_dic.Keys.Count];
            _dic.Keys.CopyTo(keys, 0);
            return keys;
        }

        /// <summary>
        /// 清理所有数据
        /// </summary>
        public virtual void Clear()
        {
            _dic.Clear();
        }
    }


    #region 线程安全版本
    /// <summary>
    /// 线程安全的临时数据存储器，存入的数据被Get一次后就会清除。
    /// </summary>
    public sealed class TemporaryStorageThreadSafe : TemporaryStorage<object>
    {
        object _lockObject = new object();

        public override int Count
        {
            get
            {
                lock (_lockObject)
                {
                    return base.Count;
                }
            }
        }

        public override object Check(string key)
        {
            lock (_lockObject)
            {
                return base.Check(key);
            }
        }

        public override void Clear()
        {
            lock (_lockObject)
            {
                base.Clear();
            }
        }

        public override object Get(string key)
        {
            lock (_lockObject)
            {
                return base.Get(key);
            }
        }

        public override string[] GetKeys()
        {
            lock (_lockObject)
            {
                return base.GetKeys();
            }
        }

        public override void Set(string key, object value)
        {
            lock (_lockObject)
            {
                base.Set(key, value);
            }
        }
    }

    #endregion

}
