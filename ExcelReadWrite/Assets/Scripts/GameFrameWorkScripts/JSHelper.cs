using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;

public class JSHelper
{

    public static object ParseProToObj(ByteArray byteArr, bool isNeedUnCompress)
    {
        if (isNeedUnCompress)
            byteArr.UnCompress();

        return Deserialize(byteArr.bytes);
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

    public static int GetDataObjectKey(object obj)
    {
        FieldInfo field = obj.GetType().GetField("id", BindingFlags.Public | BindingFlags.Instance);
        if (field == null)
            return 0;

        int objId = (int)field.GetValue(obj);
        return objId;
    }


}
