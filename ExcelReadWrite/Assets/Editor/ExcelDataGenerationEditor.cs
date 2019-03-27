using System;

using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using AssetPipeline;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ProtoBuf;
using UnityEditor;
using UnityEngine;


class ExcelDataGenerationEditor : EditorWindow
{
    #region Field

    private static ExcelDataGeneration _ExcelDataGeneration;
    private string _excelDataName;
    private string _dtoName;


    //继承
    private bool isInherit = false;
    private string _inheritDtoName;//类名
    #endregion

    #region Editor

    private static ExcelDataGenerationEditor _instance;
    [UnityEditor.MenuItem("Window/读表工具")]
    private static void Open()
    {
        if (_instance == null)
        {
            _instance = GetWindow<ExcelDataGenerationEditor>();
            _instance.minSize = new Vector2(700,300);
            _instance.Show();
            _ExcelDataGeneration = ExcelDataGeneration.GetInstance;
        }
        else
        {
            _instance.Close();
            _instance = null;
        }
    }



    void OnGUI()
    {
        if (_ExcelDataGeneration == null)
            return;
        EditorGUILayout.BeginVertical();
        {
            TargetExcelPanel();
            SingleOperationPanel();
            EditorGUILayout.Space();
            AllOperationPanel();
        }
    }

    private void TargetExcelPanel()
    {
        _dtoName = EditorGUILayout.TextField("静态数据类名", _dtoName, GUILayout.Width(700));
        if (GUILayout.Button("选择文件", GUILayout.Height(50)))
        {
            string tName = SelectFile();
            if (!string.IsNullOrEmpty(tName))
                _dtoName = tName;
        }
        isInherit = GUILayout.Toggle(isInherit, new GUIContent("是否继承"));
        if (isInherit)
        {
            EditorGUI.BeginDisabledGroup(true);
            _inheritDtoName = EditorGUILayout.TextField("继承类", _inheritDtoName, GUILayout.Width(700));
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("选择继承文件", GUILayout.Height(50)))
            {
                string tName = SelectFile();
                if (!string.IsNullOrEmpty(tName))
                {
                    _inheritDtoName = tName;
                }
            }
        }
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("Excel文件路径", _ExcelDataGeneration.GET_EXCEL_DATA_PATH, GUILayout.Width(700));
        _excelDataName = EditorGUILayout.TextField("Excel文件名", string.Format("/con_excel_{0}.xlsx", _dtoName), GUILayout.Width(700));
        EditorGUI.EndDisabledGroup();
    }

    private void SingleOperationPanel()
    {
        if (GUILayout.Button("更新指定数据", GUILayout.Height(50)))
        {
            _ExcelDataGeneration.DTO_NAME = _dtoName;
            _ExcelDataGeneration.EXCEL_DATA_NAME = _excelDataName;

            if (!_ExcelDataGeneration.IsExcelFileExist())
            {
                this.ShowNotification(
                    new GUIContent(string.Format("Excel文件：{0}不存在", _ExcelDataGeneration.EXCEL_DATA_NAME)));
                Debug.LogError(string.Format("Excel文件：{0}不存在，Path:{1}",
                    _ExcelDataGeneration.GET_EXCEL_DATA_PATH + _ExcelDataGeneration.EXCEL_DATA_NAME));
                return;
            }
            if (!_ExcelDataGeneration.IsTargetClassExist())
            {
                this.ShowNotification(
                    new GUIContent(string.Format("操作类：{0}不存在", _ExcelDataGeneration.GetOperationClassName())));
                return;
            }
            _ExcelDataGeneration.DoSingleOperatoin();
            Debug.LogError(string.Format("已更新{0}静态数据", _dtoName));
        }
    }
    private string SelectFile()
    {
        string path = EditorUtility.OpenFilePanel("Select Excel file", _ExcelDataGeneration.EXCEL_DATA_PATH,"xlsx");
        return ExcelDataHelper.GetAppDtoName(path);
    }

    private void AllOperationPanel()
    {
        if (GUILayout.Button("更新全部数据", GUILayout.Height(50)))
        {
            UpdateAllExcelData();
        }
    }

    private void UpdateAllExcelData()
    {
        var files = Directory.GetFiles(_ExcelDataGeneration.EXCEL_DATA_PATH, "*.xlsx").ToList();
        var data = files.ConvertAll<string>(item =>
        {
            string[] FileName = Path.GetFileNameWithoutExtension(item).Split('_');
            return FileName[FileName.Length - 1];
        });

        int successCount = 0;
        int failCount = 0;

        for (var i = 0; i < data.Count; i++)
        {
            _ExcelDataGeneration.DTO_NAME = data[i];
            _ExcelDataGeneration.EXCEL_DATA_NAME = _ExcelDataGeneration.GetExcelName();
            if (!_ExcelDataGeneration.IsExcelFileExist())
            {
                this.ShowNotification(
                    new GUIContent(string.Format("Excel文件：{0}不存在", _ExcelDataGeneration.EXCEL_DATA_NAME)));
                Debug.LogError(string.Format("Excel文件：{0}不存在，Path:{1}", _ExcelDataGeneration.GET_EXCEL_DATA_PATH + _ExcelDataGeneration.EXCEL_DATA_NAME));
                failCount++;
                break;
            }
            if (!_ExcelDataGeneration.IsTargetClassExist())
            {
                Debug.LogError(string.Format("操作类：{0}不存在", _ExcelDataGeneration.GetOperationClassName()));
                failCount++;
                break;
            }
            successCount++;
            _ExcelDataGeneration.DoSingleOperatoin();
        }
        Debug.LogError(string.Format("已更新全部静态数据，成功：{0} 失败{1}", successCount, failCount));

    }

    #endregion

}
