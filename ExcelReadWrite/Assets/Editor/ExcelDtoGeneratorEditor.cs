using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using appDto;
using AssetPipeline;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 协议生成工具
/// </summary>
public class ExcelDtoGeneratorEditor : EditorWindow
{
    #region Field

    private const string APPDTO_GENERATED_PATH = "/Scripts/GameScripts/AppDto/"; //协议路径
    private const string APPDTO_GENERATED_CREATE_PATH = "/Editor/ExcelDataCreate/"; //协议操作路径
    private const string FIELD_PROPERTY_PREFIX = "public ";

    private string _dtoName;//类名
    private string _dataPath;//Excel文件路径

    //Excel Data
    private DataSet _dataSet;
    private int column;
    private int row;

    //理论下两个count是一致的，不然就是bug
    private List<string> _fieldList;//字段名列表
    private List<string> _propList; //类型列表
    private List<string> _notesList; //注释列表


    #endregion


    private static ExcelDtoGeneratorEditor _instance;
    [UnityEditor.MenuItem("Window/协议生成工具")]
    private static void Open()
    {
        if (_instance == null)
        {
            _instance = GetWindow<ExcelDtoGeneratorEditor>();
            _instance.minSize = new Vector2(700, 300);
            _instance.Show();
        }
        else
        {
            _instance.Close();
            _instance = null;
        }
    }



    void OnGUI()
    {
        EditorGUI.BeginDisabledGroup(true);
        _dtoName = EditorGUILayout.TextField("静态数据类名", _dtoName, GUILayout.Width(700));
        EditorGUILayout.TextField("Excel文件名", string.Format("/con_excel_{0}.xlsx", _dtoName), GUILayout.Width(700));
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("选择文件", GUILayout.Height(50)))
        {
            string tName = SelectFile();
            if (!string.IsNullOrEmpty(tName))
            {
                _dtoName = tName;
                InitData();
            }
        }
        if (GUILayout.Button("生成协议脚本", GUILayout.Height(50)))
        {
            ProduceCSharpCode();
        }
        if (GUILayout.Button("生成协议操作脚本", GUILayout.Height(50)))
        {
            ProduceCSharpOperationCode();
        }

    }
    private string SelectFile()
    {
        _dataPath = EditorUtility.OpenFilePanel("Select Excel file", Application.dataPath + "/Editor/ExcelData/", "xlsx");
        return ExcelDataHelper.GetAppDtoName(_dataPath);
    }



    private void InitData()
    {
        EditorUtility.DisplayProgressBar("初始化数据","Loading Excel data...",0f);
        InitDataList();
        EditorUtility.DisplayProgressBar("初始化数据","Loading Excel data...",0.5f);
        UpdateFieldList();
        EditorUtility.DisplayProgressBar("初始化数据","Loading Excel data...",1f);
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 载入Excel数据
    /// </summary>
    private void InitDataList()
    {
        _dataSet = ExcelDataHelper.ReadExcel(_dataPath);
        column = _dataSet.Tables[0].Columns.Count; //竖
        row = _dataSet.Tables[0].Rows.Count; //横
    }

    /// <summary>
    /// 载入字段信息
    /// </summary>
    private void UpdateFieldList()
    {
        _fieldList = new List<string>();
        _propList = new List<string>();
        _notesList = new List<string>();
        for (int i = 0; i < column; i++)
        {
            string fieldProp = _dataSet.Tables[0].Rows[1][i].ToString(); //属性字段不为空才存入
            if (!string.IsNullOrEmpty(fieldProp))
            {
                _propList.Add(fieldProp);
                _fieldList.Add(_dataSet.Tables[0].Rows[2][i].ToString());
                string desc = _dataSet.Tables[0].Rows[3][i].ToString();
                _notesList.Add(string.Format("/** {0} */", string.IsNullOrEmpty(desc) ? "[Unknown]" : desc));
            }
        }
    }

    private string ChangeToField(string propStr, string fieldStr)
    {
        return FIELD_PROPERTY_PREFIX + propStr + " " + fieldStr + ";";
    }


    #region 协议生成
    private List<string> usingList = new List<string>()
    {
        "using System;\n",
        "using System.Collections.Generic;\n"
    };

    private string namespaceStr = "namespace appDto";
    private string SerializableStr = "[Serializable]";
    private void ProduceCSharpCode()
    {
        if (string.IsNullOrEmpty(_dtoName))
        {
            this.ShowNotification(new GUIContent("请选择文件"));
            return;
        }
        try
        {          
            string fileFullName = Application.dataPath + APPDTO_GENERATED_PATH + _dtoName + ".cs";
            Action writeText = () =>
            {
                EditorUtility.DisplayProgressBar("写入数据", "Writing...", 0.1f);
                File.WriteAllText(fileFullName, GetScriptText(),Encoding.UTF8);
                EditorUtility.DisplayProgressBar("写入数据", "Writing...", 0.75f);
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                this.ShowNotification(new GUIContent("Success 生成C#代码完毕"));

            };
            if (!FileHelper.IsExist(fileFullName))
                writeText();
            else
            {
                if (EditorUtility.DisplayDialog("提示", "协议已存在，是否进行替换？", "确定", "取消"))
                {
                    writeText();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("An error occurred while saving file: " + e);
        }
    }
    private string GetScriptText()
    {
        StringBuilder textBuilder = new StringBuilder();
        foreach (var s in usingList)
        {
            textBuilder.Append(s);
        }

        textBuilder.Append(namespaceStr + "\n");
        textBuilder.Append("{" + "\n");
        textBuilder.Append("    " + SerializableStr + "\n");
        textBuilder.Append(string.Format("    public class {0}", _dtoName) + "\n");
        textBuilder.Append("    {" + "\n");
        if (_fieldList != null && _fieldList.Count > 0)
        {
            for (var i = 0; i < _fieldList.Count && i < _propList.Count && i < _notesList.Count; i++)
            {
                string str = ChangeToField(_propList[i], _fieldList[i]);
                textBuilder.Append("        " + _notesList[i] + "\n");
                textBuilder.Append("        " + str + "\n");
                textBuilder.AppendLine();
            }
        }
        textBuilder.Append("    }" + "\n");
        textBuilder.Append("}" + "\n");

        return textBuilder.ToString();
    }

    #endregion

    #region 协议创建器生成

    private void ProduceCSharpOperationCode()
    {
        if (string.IsNullOrEmpty(_dtoName))
        {
            this.ShowNotification(new GUIContent("请选择文件"));
            return;
        }
        try
        {
            string fileFullName = Application.dataPath + APPDTO_GENERATED_CREATE_PATH + _dtoName + "_Creator.cs";
            Action writeText = () =>
            {
                EditorUtility.DisplayProgressBar("写入数据", "Writing...", 0.0f);
                File.WriteAllText(fileFullName, GetOperationScriptText(),Encoding.UTF8);
                EditorUtility.DisplayProgressBar("写入数据", "Writing...", 0.75f);
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                this.ShowNotification(new GUIContent("Success 生成C#代码完毕"));
            };
            if (!FileHelper.IsExist(fileFullName))
                writeText();
            else
            {
                if (EditorUtility.DisplayDialog("提示", "协议操作类已存在，是否进行替换？", "确定", "取消"))
                {
                    writeText();
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.Log("An error occurred while saving file: " + e);
        }
    }
    private string GetOperationScriptText()
    {
        StringBuilder textBuilder = new StringBuilder();
        textBuilder.Append("using appDto;\n");
        textBuilder.Append("\n");
        textBuilder.Append("namespace ExcelDataCreator\n");
        textBuilder.Append("{\n");
        textBuilder.Append(string.Format("    public class {0}_Creator : BaseExcel_Creator\n", _dtoName));
        textBuilder.Append("    {\n");
        textBuilder.Append(string.Format("        public {0} _{1} = new {0}();\n", _dtoName, ToLowerFirst(_dtoName)));
        textBuilder.Append("\n");
        string tStr = "        public override object GetData()\n        {" + string.Format("\n            return _{0};\n", ToLowerFirst(_dtoName)) + "        }\n";
        textBuilder.Append(tStr);
        textBuilder.Append("\n");
        string tStr2 = "        public void set_id(string pId)\n        {\n" + string.Format("            _{0}.id = int.Parse(pId);\n        ", ToLowerFirst(_dtoName)) + "}\n";
        textBuilder.Append(tStr2);
        textBuilder.Append("\n");
        for (var i = 0; i < _fieldList.Count; i++)
        {
            if (_fieldList[i] != "id") //ID字段已经设置过了
            {
                textBuilder.AppendLine(_notesList[i]);
                textBuilder.AppendLine(ChangeToFunction(_fieldList[i]));
            }
        }
        textBuilder.Append("    }\n");
        textBuilder.Append("}\n");


        return textBuilder.ToString();
    }

    //通过字段名转为空函数（业务逻辑自己实现）
    private string ChangeToFunction(string fieldStr)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Format("        public void set_{0}(string p{1})\n", fieldStr, ToUpperFirst(fieldStr)));
        sb.Append("        {\n");
        sb.Append("\n");
        sb.Append("        }\n");

        return sb.ToString();
    }
    public static string ToUpperFirst(string str)
    {
        char[] a = str.ToCharArray();
        a[0] = char.ToUpper(a[0]);
        return new string(a);
    }
    public static string ToLowerFirst(string str)
    {
        char[] a = str.ToCharArray();
        a[0] = char.ToLower(a[0]);
        return new string(a);
    }
    #endregion

}
