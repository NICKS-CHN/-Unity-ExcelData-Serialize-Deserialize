using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using appDto;
using AssetPipeline;
using Excel;
using UnityEditor;
using UnityEngine;

public class ExcelDataGeneration
{
    #region Field

    private const string EXCEL_CLASS_CREATOR_PREFIX = "ExcelDataCreator.";
    private const string EXCEL_CLASS_CREATOR_SUFFIX = "_Creator";
    private const string GETDATA_METHOD = "GetData";

    private string STATIC_DATA_PATH; //静态数据路径

    public string EXCEL_DATA_PATH; //Excel文件路径
    public string GET_EXCEL_DATA_PATH {get { return EXCEL_DATA_PATH; } }

    public string EXCEL_DATA_NAME; //Excel文件名

    public string DTO_NAME = ""; //类名

    private List<string> methodList;
    private int column;
    private int row;
    private DataSet dataset;
    #endregion

    private static ExcelDataGeneration _instance;

    public static ExcelDataGeneration GetInstance
    {
        get
        {
            if (_instance == null)
                _instance = new ExcelDataGeneration();
            return _instance;
        }
    }
    public ExcelDataGeneration()
    {
        InitData();
    }

    #region InitData

    private void InitData()
    {
        EXCEL_DATA_PATH = Application.dataPath + "/Editor/ExcelData/";
        STATIC_DATA_PATH = Application.dataPath + "/GameResources/StaticData/";
    }

    #endregion


     public void DoSingleOperatoin()
     {
        InitDataList();
        UpdateMethodName();
        OperationData();
        AssetDatabase.Refresh();
    }

    #region Operation

    /// <summary>
    /// 载入Excel数据
    /// </summary>
    private void InitDataList()
    {
        dataset = ReadExcel(EXCEL_DATA_PATH + EXCEL_DATA_NAME);
        column = dataset.Tables[0].Columns.Count; //竖
        row = dataset.Tables[0].Rows.Count; //横
    }
    public static DataSet ReadExcel(string path)
    {
        FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        DataSet result = excelReader.AsDataSet();
        excelReader.Close();
        return result;
    }

    /// <summary>
    /// 获取方法名
    /// </summary>
    private void UpdateMethodName()
    {
        methodList = new List<string>();
        for (int i = 0; i < column; i++)
        {
            methodList.Add("set_" + dataset.Tables[0].Rows[1][i].ToString());//存入第二行字段名
        }
    }

    /// <summary>
    /// 传入数据
    /// </summary>
    private void OperationData()
    {
        DataList data = new DataList();

        Type type = Type.GetType(GetOperationClassName());
        for (int i = 3; i < row; i++)
        {
            object typeInst = Activator.CreateInstance(type);
            for (int j = 0; j < column; j++)
            {
                MethodInfo method = typeInst.GetType().GetMethod(methodList[j]);
                var lineData = dataset.Tables[0].Rows[i][j].ToString();   //源类型：excel内容 通常为string

                //var type2 = method.GetParameters()[0].ParameterType;      //目标类型 就是参数类型，优化都传入源类型，各自模块自行转换
                //var newObj = Convert.ChangeType(lineData, type2);

                //执行set方法
                string str = (string)method.Invoke(typeInst, new object[] { lineData });
            }

            MethodInfo GetDataMethod = typeInst.GetType().GetMethod(GETDATA_METHOD); 
            var dataItem = GetDataMethod.Invoke(typeInst, new object[] { });//得到目标类型
            if (dataItem != null)
                data.items.Add(dataItem);
        }
        WriteByte(data);
    }

    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="obj"></param>
    private void WriteByte(object obj)
    {
        string fileName = STATIC_DATA_PATH + DTO_NAME + ".bytes";
        FileHelper.DeleteFile(fileName);

        var buff = Object2Bytes(obj);
        FileHelper.WriteAllBytes(fileName, buff);
    }

    /// <summary>
    /// 得到二进制
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public ByteArray Object2Bytes(object obj)
    {
        ByteArray buff;
        using (MemoryStream ms = new MemoryStream())
        {
            IFormatter iFormatter = new BinaryFormatter();
            iFormatter.Serialize(ms, obj);
            buff = new ByteArray(ms.GetBuffer());
            buff.Compress();
        }
        return buff;
    }



    #region Event

    public bool IsExcelFileExist()
    {
        string filePath = EXCEL_DATA_PATH + GetExcelName();
        return FileHelper.IsExist(filePath);
    }

    public bool IsTargetClassExist()
    {
        Type type = Type.GetType(GetOperationClassName());
        return type != null;
    }

    public string GetOperationClassName()
    {
        return EXCEL_CLASS_CREATOR_PREFIX + DTO_NAME + EXCEL_CLASS_CREATOR_SUFFIX;
    }

    public string GetExcelName()
    {
        return string.Format("con_excel_{0}.xlsx", DTO_NAME);
    }

    #endregion

    #endregion
}
