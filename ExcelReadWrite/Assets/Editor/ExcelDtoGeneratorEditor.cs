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
    private const string FIELD_PROPERTY_PREFIX = "public ";

    private string _dtoName;//类名
    private string _dataPath;//Excel文件路径

    //Excel Data
    private DataSet _dataSet;
    private int column;
    private int row;



    private List<string> _fieldList;
    private List<string> usingList = new List<string>()
    {
        "using System;\n",
        "using System.Collections.Generic;\n"
    };

    private string namespaceStr = "namespace appDto";
    private string SerializableStr = "[Serializable]";

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
            _dtoName = SelectFile();
        }
        if (GUILayout.Button("生成协议脚本", GUILayout.Height(50)))
        {
            InitDataList();
            UpdateFieldList();
            ProduceCSharpCode();
        }

    }
    private string SelectFile()
    {
        _dataPath = EditorUtility.OpenFilePanel("Select Excel file", Application.dataPath + "/Editor/ExcelData/", "xlsx");
        return ExcelDataHelper.GetAppDtoName(_dataPath);
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
        for (int i = 0; i < column; i++)
        {
            string fieldProp = _dataSet.Tables[0].Rows[1][i].ToString(); //属性字段不为空才存入
            if (!string.IsNullOrEmpty(fieldProp))
                _fieldList.Add(FIELD_PROPERTY_PREFIX + fieldProp + " " + _dataSet.Tables[0].Rows[2][i].ToString() + ";");//存入字段
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
            foreach (var item in _fieldList)
            {
                textBuilder.Append("        " + item);
            }
        }
        textBuilder.Append("    }" + "\n");
        textBuilder.Append("}" + "\n");

        return textBuilder.ToString();
    }

    private void ProduceCSharpCode()
    {
        try
        {
            string fileFullName = Application.dataPath + APPDTO_GENERATED_PATH + _dtoName + ".cs";
            if (!FileHelper.IsExist(fileFullName))
                File.WriteAllText(Application.dataPath + APPDTO_GENERATED_PATH, GetScriptText());
            else
            {
                Debug.LogError("协议已经存在");
            }

            this.ShowNotification(new GUIContent("Success 生成C#代码完毕"));
            AssetDatabase.Refresh();
        }
        catch (System.Exception e)
        {
            Debug.Log("An error occurred while saving file: " + e);
        }
    }

}
