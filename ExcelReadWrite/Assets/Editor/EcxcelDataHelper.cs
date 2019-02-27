using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using UnityEngine;

public class ExcelDataHelper
{
    /// <summary>
    /// 从路径得到Excel文件名
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string GetAppDtoName(string path)
    {
        string[] FileName = Path.GetFileNameWithoutExtension(path).Split('_');
        return FileName[FileName.Length - 1];
    }

    /// <summary>
    /// 得到Excel数据 DataSet
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static DataSet ReadExcel(string path)
    {
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        excelReader.Close();
        return result;
    }
}
