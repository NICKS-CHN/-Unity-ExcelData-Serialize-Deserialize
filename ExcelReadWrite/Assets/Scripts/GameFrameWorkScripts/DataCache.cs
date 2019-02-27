using System;
using System.Collections;
using System.Collections.Generic;
using appDto;
using UnityEngine;

public class DataCache
{

    public class DataCollection<T>
    {
        public List<T> list;
        public Dictionary<int, T> map;

        public DataCollection(int count)
        {
            list = new List<T>(count);
            map = new Dictionary<int, T>(count);
        }

        public void AddMap(int id, T obj)
        {
            if (map.ContainsKey(id))
                Debug.LogError(string.Format("the same key{0} to add <{1}>", id, obj.GetType().Name));
            map[id] = obj;
        }

        public void AddList(T obj)
        {
            list.Add(obj);
        }
    }

    private static Dictionary<Type, ByteArray> _rawBytesDic; //保存全部静态二进制据，需要时从这里读取，然后移除
    private static Dictionary<Type, object> _staticDataMap; //存储使用过的数据，方便读取而无需再次反射获得，使用DataCollection结构便于对数据进行操作

    //载入静态数据
    public static void SetUp(Dictionary<Type, ByteArray> staticDataBytes)
    {
        Debug.Log("Start To DataCache Setup");

        _rawBytesDic = staticDataBytes;

        if(_staticDataMap == null)
            _staticDataMap = new Dictionary<Type, object>();
    }

    //解析数据
    private static TResult CacheToMap<T, TResult>(Type type)
    {
        ByteArray byteArr;
        if (_rawBytesDic.TryGetValue(type, out byteArr)) ;
        {
            if (byteArr != null && byteArr.Length > 0)
            {
                var dataList = JSHelper.ParseProToObj(byteArr, true) as DataList;
                if (dataList != null)
                {
                    var collection = new DataCollection<T>(dataList.items.Count);
                    for (int i = 0; i < dataList.items.Count; i++)
                    {
                        T obj = (T) dataList.items[i];
                        int key = JSHelper.GetDataObjectKey(obj);  //反射获取id
                        collection.AddList(obj);
                        collection.AddMap(key, obj);
                    }
                    _staticDataMap[type] = collection;
                }
            }
            _rawBytesDic.Remove(type);
        }

        return _staticDataMap.ContainsKey(type) ? (TResult)_staticDataMap[type] : default(TResult);
    }

    #region Method

    /// <summary>
    /// 得到表字典数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Dictionary<int, T> GetDictByCls<T>()
    {
        if (_staticDataMap == null)
            return null;

        var type = typeof(T);
        var result = CacheToMap<T, DataCollection<T>>(type);
        return result != null ? result.map : null;
    }

    /// <summary>
    /// 得到表字典列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> GetListByCls<T>()
    {
        if (_staticDataMap == null)
            return null;

        var type = typeof(T);
        var result = CacheToMap<T, DataCollection<T>>(type);
        return result != null ? result.list : null;
    }

    /// <summary>
    /// 得到表字典列表
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetDtoByCls<T>(int key)
    {
        if (_staticDataMap == null)
            return default(T);

        var type = typeof(T);
        var result = CacheToMap<T, DataCollection<T>>(type);
        if (result == null)
        {
            Debug.LogError(string.Format("[Excel Error]读表：{0}失败，可能是DataManager中没有进行加载，或者是CacheToMap中没有进行配置"));
            return default(T);
        }
        else if(result.map.ContainsKey(key))
        {
            return result.map[key];
        }
        else
            return default(T);
    }

    #endregion
}
