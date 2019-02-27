using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using appDto;
using AssetPipeline;
using UnityEngine;
//using appDto;

public class Test : MonoBehaviour
{
    void Start()
    {
        DataManager.instance.SetUp();

        var petList = DataCache.GetListByCls<AchievementDto>();
    }
}


