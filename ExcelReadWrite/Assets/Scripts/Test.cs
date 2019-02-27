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
        DeSerializableExcel();
    }

    static void DeSerializableExcel()
    {
        var buff = ReadAllBytes();
        buff.UnCompress();
        var target = Deserialize(buff.bytes);
        var data = target as DataList;
        var achievementList = data.items.ConvertAll<PetDto>((item) => item as PetDto);

        foreach (var achievementDto in achievementList)
        {
            Debug.Log(achievementDto.name);
        }
    }

    public static ByteArray ReadAllBytes()
    {
        string path = Application.dataPath + "/GameResources/StaticData/" + "PetDto" + ".bytes";
            var data = FileHelper.LoadByteArrayFromFile(path);
            return data;
        }

        public static object Deserialize(byte[] arr)
        {
            object obj = null;
            using (MemoryStream ms = new MemoryStream())
            {
                ms.Write(arr, 0, arr.Length);
                ms.Flush();
                ms.Position = 0;
                BinaryFormatter formatter = new BinaryFormatter();
                obj = formatter.Deserialize(ms);
            }
            return obj;
        }
    }


