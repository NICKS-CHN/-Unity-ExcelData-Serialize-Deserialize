using System;
using System.Collections;
using System.Collections.Generic;
using appDto;
using AssetPipeline;
using UnityEngine;

public class DataManager
{

    #region Singleton

    private static DataManager _instance;

    public static DataManager instance
    {
        get
        {
            if(_instance == null)
                _instance = new DataManager();
            return _instance;
        }
    }
    private DataManager() { }

    #endregion

    private Dictionary<Type, ByteArray> _staticDataByteArrayDic;


    public void SetUp()
    {
        LoadStaticData();
        if(_staticDataByteArrayDic!=null)
            DataCache.SetUp(_staticDataByteArrayDic);
        _staticDataByteArrayDic = null;
    }


    /// <summary>
    /// 加载静态数据
    /// </summary>
    public void LoadStaticData()
    {
        var typeList = DataCacheMap.serviceList();
        _staticDataByteArrayDic = new Dictionary<Type, ByteArray>(typeList.Count);
        if (_staticDataByteArrayDic != null)
        {
            for (var i = 0; i < typeList.Count; i++)
            {
                var type = typeList[i];
                var byteArray = FileHelper.LoadByteArrayFromFile(GetStaticDataPath(type.FullName));
                _staticDataByteArrayDic.Add(type, byteArray);
            }
        }
    }

    public static string GetStaticDataPath(string dataType)
    {
        string fileName = GetStaticFileName(dataType);
        return "Assets/GameResources/StaticData/" + fileName;
    }
    private static string GetStaticFileName(string type)
    {
        return type + ".bytes";
    }

}

namespace appDto
{
    public class DataCacheMap
    {
        public static List<System.Type> serviceList()
        {
            List<System.Type> serviceArr = new List<Type>();
            serviceArr.Add(typeof (AchievementDto));
            serviceArr.Add(typeof (PetDto));

            return serviceArr;
        }
    }
}