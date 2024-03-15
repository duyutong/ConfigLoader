﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using LitJson;
using System.IO;
using System.Text;
using Excel;
using System.Data;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Reflection;
using System.Text.RegularExpressions;

/// <summary>
/// 导表工具
/// </summary>
public class ExcelToCSharp : EditorWindow
{
    public static string excelPath;
    public static string jsonPath;
    public static string binaryPath;
    public static string csharpPath;
    public static string loaderPath;
    private static string libraryPath = Application.dataPath + "/ConfigData/PathLibrary.json";
    private static Dictionary<string, Dictionary<string, string>> proTypeDic = new Dictionary<string, Dictionary<string, string>>();
    private static Dictionary<string, Dictionary<string, string>> proDesDic = new Dictionary<string, Dictionary<string, string>>();
    private static PathLibrary pathLibrary = null;


    private static void SetProDic()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(excelPath);
        FileInfo[] files = directoryInfo.GetFiles();
        proTypeDic.Clear();
        proDesDic.Clear();

        foreach (FileInfo fileInfo in files)
        {
            if (!fileInfo.Name.EndsWith(".xlsx")) continue;
            FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet excelData = reader.AsDataSet();
            foreach (DataTable sheet in excelData.Tables)
            {
                if (sheet.Rows.Count <= 0) continue;

                //读取数据表行数和列数
                int rowCount = sheet.Rows.Count;
                int colCount = sheet.Columns.Count;
                //准备一个列表存储整个表的数据
                List<object> table = new List<object>();
                //读取数据
                string className = fileInfo.Name.Replace(".xlsx", "");
                proTypeDic.Add(className, new Dictionary<string, string>());
                proDesDic.Add(className, new Dictionary<string, string>());
                for (int _col = 0; _col < colCount; _col++)
                {
                    string field = sheet.Rows[0][_col].ToString();
                    if (string.IsNullOrEmpty(field)) continue;
                    string pro = sheet.Rows[1][_col].ToString();
                    string des = sheet.Rows[2][_col].ToString();
                    proTypeDic[className].Add(field, pro);
                    proDesDic[className].Add(field, des);
                }
            }
        }
    }
    private static void ToCSharp_Json()
    {
        if (File.Exists(csharpPath)) File.Delete(csharpPath);
        foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in proTypeDic)
        {
            string className = keyValuePair.Key;
            Dictionary<string, string> proKeyPair = keyValuePair.Value;
            Dictionary<string, string> proDicPair = proDesDic[className];
            string _context = "";
            string _initContent = "";
            string _classStr = CSTemplate.classStr_Json;
            //Debug.LogError("生成配置表类 " + className);
            foreach (KeyValuePair<string, string> keyValuePair1 in proKeyPair)
            {
                string pro = keyValuePair1.Value;
                string proName = keyValuePair1.Key;
                string proDes = proDicPair[proName];

                string tempStr = CSTemplate.proStr_Json;
                tempStr = tempStr.Replace("#ProType#", pro);
                tempStr = tempStr.Replace("#ProName#", proName);
                tempStr = tempStr.Replace("#ProDes#", proDes);
                _context += tempStr;

                string initTemp = CSTemplate.classInitStr;
                initTemp = initTemp.Replace("#ProName#", proName);
                initTemp = initTemp.Replace("#MethodName#", pro.GetMethodName());
                _initContent += initTemp;
            }
            _classStr = _classStr.Replace("#ClassName#", className);
            _classStr = _classStr.Replace("#ProContext#", _context);
            _classStr = _classStr.Replace("#InitContext#", _initContent);

            //Debug.LogError("生成配置表类 " + className + " _classStr = " + _classStr);
            //写入文件
            string csSavePath = csharpPath + "/" + className + ".cs";
            FileInfo saveInfo = new FileInfo(csSavePath);
            DirectoryInfo dir = saveInfo.Directory;
            if (!dir.Exists) dir.Create();
            byte[] decBytes = Encoding.UTF8.GetBytes(_classStr);

            FileStream fileStream = saveInfo.Create();
            fileStream.Write(decBytes, 0, decBytes.Length);
            fileStream.Flush();
            fileStream.Close();
            Debug.Log("配置表类生成完毕 " + className);
        }
    }
    [MenuItem("Tools/Excel/Binary/ExcelToCsharp")]
    private static void ExcelToCsharp_PB()
    {
        if (!File.Exists(libraryPath)) { ShowInitWindow(); return; }
        if (pathLibrary == null) InitPathLibrary();

        SetProDic();
        ToCSharp_PB();
    }
    private static void ToCSharp_PB()
    {
        if (File.Exists(csharpPath)) File.Delete(csharpPath);

        foreach (KeyValuePair<string, Dictionary<string, string>> keyValuePair in proTypeDic)
        {
            string className = keyValuePair.Key;
            Dictionary<string, string> proKeyPair = keyValuePair.Value;
            Dictionary<string, string> proDicPair = proDesDic[className];
            string _context = "";
            string _initContent = "";
            string _classStr = CSTemplate.classStr_PB;
            //Debug.LogError("生成配置表类 " + className);
            int pos = 0;
            foreach (KeyValuePair<string, string> keyValuePair1 in proKeyPair)
            {
                string pro = keyValuePair1.Value;
                if (!CheckListAndDictionaryCount(pro)) continue;

                string proName = keyValuePair1.Key;
                string proDes = proDicPair[proName];
                string tempStr = CSTemplate.proStr_PB;
                pos += 1;
                tempStr = tempStr.Replace("#Pos#", pos.ToString());
                tempStr = tempStr.Replace("#ProType#", pro);
                tempStr = tempStr.Replace("#ProName#", proName);
                tempStr = tempStr.Replace("#ProDes#", proDes);
                _context += tempStr;

                string initTemp = CSTemplate.classInitStr;
                initTemp = initTemp.Replace("#ProName#", proName);
                initTemp = initTemp.Replace("#MethodName#", pro.GetMethodName());
                _initContent += initTemp;
            }
            _classStr = _classStr.Replace("#ClassName#", className);
            _classStr = _classStr.Replace("#ProContext#", _context);
            _classStr = _classStr.Replace("#InitContext#", _initContent);

            //Debug.LogError("生成配置表类 " + className + " _classStr = " + _classStr);
            //写入文件
            string csSavePath = csharpPath + "/" + className + ".cs";
            FileInfo saveInfo = new FileInfo(csSavePath);
            DirectoryInfo dir = saveInfo.Directory;
            if (!dir.Exists) dir.Create();
            byte[] decBytes = Encoding.UTF8.GetBytes(_classStr);

            FileStream fileStream = saveInfo.Create();
            fileStream.Write(decBytes, 0, decBytes.Length);
            fileStream.Flush();
            fileStream.Close();
            Debug.Log("配置表类生成完毕 " + className);
        }
    }
    static bool CheckListAndDictionaryCount(string input)
    {
        // Count occurrences of "List<" and "Dictionary<"
        int listCount = Regex.Matches(input, @"\bList<").Count;
        int dictCount = Regex.Matches(input, @"\bDictionary<").Count;

        // Check if the total count exceeds 1
        return (listCount + dictCount <= 1);
    }
    [MenuItem("Tools/Excel/Binary/CsharpToBinary")]
    private static void CsharpToBinary()
    {
        if (!File.Exists(libraryPath)) { ShowInitWindow(); return; }
        if (pathLibrary == null) InitPathLibrary();

        MemoryStream tempStream = new MemoryStream(1024);
        DirectoryInfo directoryInfo = new DirectoryInfo(excelPath);
        FileInfo[] files = directoryInfo.GetFiles();

        foreach (FileInfo fileInfo in files)
        {
            if (!fileInfo.Name.EndsWith(".xlsx")) continue;
            FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet excelData = reader.AsDataSet();

            //读取数据
            string className = fileInfo.Name.Replace(".xlsx", "");
            Type classType = Type.GetType(className);
            if (classType == null)
            {
                Debug.Log("请先生成Csharp文件：" + className + ".cs");
                continue;
            }
            PropertyInfo[] properties = classType.GetProperties();

            //准备一个列表存储整个表的数据
            List<object> table = new List<object>();
            foreach (DataTable sheet in excelData.Tables)
            {
                if (sheet.Rows.Count <= 0) continue;

                //读取数据表行数和列数
                int rowCount = sheet.Rows.Count;
                int colCount = sheet.Columns.Count;

                //存储数据的键值对
                Dictionary<string, object> pairs = new Dictionary<string, object>();
                for (int _row = 3; _row < rowCount; _row++)
                {
                    for (int _col = 0; _col < colCount; _col++)
                    {
                        //读取第1行数据作为表头字段
                        string field = sheet.Rows[0][_col].ToString();
                        if (string.IsNullOrEmpty(field)) continue;
                        object value = sheet.Rows[_row][_col];
                        pairs[field] = value;
                    }

                    // 实例化对象
                    // 获取构造函数信息
                    var constructor = classType.GetConstructor(new[] { typeof(Dictionary<string, object>) });
                    if (constructor != null)
                    {
                        // 使用构造函数实例化对象
                        object instance = constructor.Invoke(new object[] { pairs });
                        if (instance != null) table.Add(instance);
                    }
                }
            }

            //生成二进制文件
            tempStream.Position = 0;
            tempStream.SetLength(0L);
            ProtoBuf.Serializer.Serialize(tempStream, table);

            //存储
            string binarySavePath = binaryPath + "/" + fileInfo.Name.Replace(".xlsx", ".bytes");
            // 将MemoryStream的内容写入到bin文件中
            FileInfo saveInfo = new FileInfo(binarySavePath);
            DirectoryInfo dir = saveInfo.Directory;
            if (!dir.Exists) dir.Create();

            using (FileStream fileStream = File.Create(binarySavePath))
            {
                tempStream.Seek(0, SeekOrigin.Begin); // 将MemoryStream的指针移动到开头
                tempStream.CopyTo(fileStream); // 将MemoryStream的内容复制到文件流中
            }
        }
    }
    [MenuItem("Tools/Excel/Binary/RefreshAll")]
    private static void RefreshAllByPB()
    {
        //生成二进制脚本
        CsharpToBinary();
        //生成loader工具类
        CreatLoader_PB();
    }
    private static void CreatLoader_PB()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(csharpPath);
        FileInfo[] files = directoryInfo.GetFiles();
        string _loaderClassStr = CSTemplate.loaderClassStr_PB;
        string _loaderMember = CSTemplate.loaderMember_PB;
        string _member = "";
        _loaderClassStr = _loaderClassStr.Replace("#libraryPath#", libraryPath);
        foreach (FileInfo fileInfo in files)
        {
            if (!fileInfo.Name.EndsWith(".cs")) continue;
            string fileName = fileInfo.Name.Replace(".cs", "");
            _member += _loaderMember.Replace("#ClassName#", fileName);
        }
        _loaderClassStr = _loaderClassStr.Replace("#LoaderMember#", _member);
        //写入文件
        string csSavePath = loaderPath;
        if (File.Exists(csSavePath)) File.Delete(csSavePath);
        FileInfo saveInfo = new FileInfo(csSavePath);
        DirectoryInfo dir = saveInfo.Directory;
        if (!dir.Exists) dir.Create();
        byte[] decBytes = Encoding.UTF8.GetBytes(_loaderClassStr);

        FileStream fileStream = saveInfo.Create();
        fileStream.Write(decBytes, 0, decBytes.Length);
        fileStream.Flush();
        fileStream.Close();
    }
    [MenuItem("Tools/Excel/Json/ExcelToCsharp")]
    private static void ExcelToCsharp_Json()
    {
        if (!File.Exists(libraryPath)) { ShowInitWindow(); return; }
        if (pathLibrary == null) InitPathLibrary();

        SetProDic();
        ToCSharp_Json();
    }
    [MenuItem("Tools/Excel/Json/ExcelToJson")]
    private static void ExcelToJson()
    {
        if (!File.Exists(libraryPath)) { ShowInitWindow(); return; }
        if (pathLibrary == null) InitPathLibrary();
        if (File.Exists(jsonPath)) File.Delete(jsonPath);
        DirectoryInfo directoryInfo = new DirectoryInfo(excelPath);
        FileInfo[] files = directoryInfo.GetFiles();
        proTypeDic.Clear();
        proDesDic.Clear();
        foreach (FileInfo fileInfo in files)
        {
            if (!fileInfo.Name.EndsWith(".xlsx")) continue;
            FileStream stream = File.Open(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet excelData = reader.AsDataSet();
            foreach (DataTable sheet in excelData.Tables)
            {
                if (sheet.Rows.Count <= 0) continue;

                //读取数据表行数和列数
                int rowCount = sheet.Rows.Count;
                int colCount = sheet.Columns.Count;
                //准备一个列表存储整个表的数据
                List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();
                //读取数据
                string className = fileInfo.Name.Replace(".xlsx", "");
                proTypeDic.Add(className, new Dictionary<string, string>());
                proDesDic.Add(className, new Dictionary<string, string>());
                for (int _col = 0; _col < colCount; _col++)
                {
                    string field = sheet.Rows[0][_col].ToString();
                    if (string.IsNullOrEmpty(field)) continue;
                    string pro = sheet.Rows[1][_col].ToString();
                    string des = sheet.Rows[2][_col].ToString();
                    proTypeDic[className].Add(field, pro);
                    proDesDic[className].Add(field, des);
                }
                for (int _row = 3; _row < rowCount; _row++)
                {
                    //准备一个字典存储每一行的数据
                    Dictionary<string, object> row = new Dictionary<string, object>();
                    for (int _col = 0; _col < colCount; _col++)
                    {
                        //读取第1行数据作为表头字段
                        string field = sheet.Rows[0][_col].ToString();
                        if (string.IsNullOrEmpty(field)) continue;
                        string proType = proTypeDic[className][field];
                        string value = sheet.Rows[_row][_col].ToString();
                        row[field] = value;
                    }
                    //添加到表数据中
                    table.Add(row);
                }
                //生成Json字符串
                string json = JsonConvert.SerializeObject(table, Formatting.Indented);
                //写入文件
                string jsonSavePath = jsonPath + "/" + fileInfo.Name.Replace(".xlsx", ".json");
                FileInfo saveInfo = new FileInfo(jsonSavePath);
                DirectoryInfo dir = saveInfo.Directory;
                if (!dir.Exists) dir.Create();
                byte[] decBytes = Encoding.UTF8.GetBytes(json);

                FileStream fileStream = saveInfo.Create();
                fileStream.Write(decBytes, 0, decBytes.Length);
                fileStream.Flush();
                fileStream.Close();
            }
        }
        //刷新本地资源
        AssetDatabase.Refresh();
    }

    private static void InitPathLibrary()
    {
        if (!File.Exists(libraryPath)) { ShowInitWindow(); return; }
        pathLibrary = JsonMapper.ToObject<PathLibrary>(File.ReadAllText(libraryPath));

        excelPath = pathLibrary.excelPath;
        jsonPath = pathLibrary.jsonPath;
        binaryPath = pathLibrary.binaryPath;
        csharpPath = pathLibrary.csharpPath;
        loaderPath = pathLibrary.loaderPath;
    }

    [MenuItem("Tools/Excel/Json/RefreshAll")]
    private static void RefreshAllByJson()
    {
        ExcelToJson();

        //生成json脚本
        ToCSharp_Json();
        //生成loader工具类
        CreatLoader_Json();
    }
    private static void CreatLoader_Json()
    {
        DirectoryInfo directoryInfo = new DirectoryInfo(csharpPath);
        FileInfo[] files = directoryInfo.GetFiles();
        string _loaderClassStr = CSTemplate.loaderClassStr_Json;
        string _loaderMember = CSTemplate.loaderMember_Json;
        string _member = "";
        _loaderClassStr = _loaderClassStr.Replace("#libraryPath#", libraryPath);
        foreach (FileInfo fileInfo in files)
        {
            if (!fileInfo.Name.EndsWith(".cs")) continue;
            string fileName = fileInfo.Name.Replace(".cs", "");
            _member += _loaderMember.Replace("#ClassName#", fileName);
        }
        _loaderClassStr = _loaderClassStr.Replace("#LoaderMember#", _member);
        //写入文件
        string csSavePath = loaderPath;
        if (File.Exists(csSavePath)) File.Delete(csSavePath);
        FileInfo saveInfo = new FileInfo(csSavePath);
        DirectoryInfo dir = saveInfo.Directory;
        if (!dir.Exists) dir.Create();
        byte[] decBytes = Encoding.UTF8.GetBytes(_loaderClassStr);

        FileStream fileStream = saveInfo.Create();
        fileStream.Write(decBytes, 0, decBytes.Length);
        fileStream.Flush();
        fileStream.Close();
    }
    [MenuItem("Tools/Excel/InitPath")]
    private static void ShowInitWindow()
    {
        if (pathLibrary == null && File.Exists(libraryPath)) InitPathLibrary();
        Rect wr = new Rect(0, 0, 600, 150);
        ExcelToCSharp window = (ExcelToCSharp)GetWindowWithRect(typeof(ExcelToCSharp), wr, true, "InitPath");
        window.Show();
    }
    private void OnGUI()
    {
        excelPath = EditorGUILayout.TextField("excelPath/配置表地址:", excelPath);
        jsonPath = EditorGUILayout.TextField("jsonPath/json文件地址:", jsonPath);
        binaryPath = EditorGUILayout.TextField("binaryPath/二进制文件地址:", binaryPath);
        csharpPath = EditorGUILayout.TextField("csharpPath/配置表类:", csharpPath);
        loaderPath = EditorGUILayout.TextField("loaderPath/加载工具:", loaderPath);
        if (GUILayout.Button("保存并关闭窗口", GUILayout.Width(200)))
        {
            Dictionary<string, string> table = new Dictionary<string, string>()
            {
                ["excelPath"] = excelPath,
                ["jsonPath"] = jsonPath,
                ["binaryPath"] = binaryPath,
                ["csharpPath"] = csharpPath,
                ["loaderPath"] = loaderPath,
            };
            string json = JsonConvert.SerializeObject(table, Formatting.Indented);
            string jsonSavePath = libraryPath;
            if (File.Exists(jsonSavePath)) File.Delete(jsonSavePath);
            FileInfo saveInfo = new FileInfo(jsonSavePath);
            DirectoryInfo dir = saveInfo.Directory;
            if (!dir.Exists) dir.Create();
            byte[] decBytes = Encoding.UTF8.GetBytes(json);

            FileStream fileStream = saveInfo.Create();
            fileStream.Write(decBytes, 0, decBytes.Length);
            fileStream.Flush();
            fileStream.Close();
            this.Close();
        }
    }
}
