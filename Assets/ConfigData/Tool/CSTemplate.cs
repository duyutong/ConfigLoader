/// <summary>
/// 自动生成cs文件时使用的预制文字
/// </summary>
public class CSTemplate
{
    /// <summary>
    /// json生成的配置表类
    /// </summary>
    public const string classStr_PB =
@"
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// #ClassDes#
/// <summary>
[global::ProtoBuf.ProtoContract (Name = @""#ClassName#"")]
public class #ClassName#:BaseConfig
{
	#ProContext#

    public #ClassName#() { }
    public #ClassName#(Dictionary<string, object> _dataDic)
    {#InitContext#
        id = ID;
    }
} ";
    /// <summary>
    /// 配置表类的属性部分
    /// </summary>
    public const string proStr_PB =
    @"
    /// <summary>
    /// #ProDes#
    /// </summary>
    [global::ProtoBuf.ProtoMember(#Pos#)] public #ProType# #ProName#;";
    /// <summary>
    /// json生成的配置表类
    /// </summary>
    public const string classStr_Json =
@"
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// #ClassDes#
/// <summary>
public class #ClassName#:BaseConfig
{
	#ProContext#

    public #ClassName#() { }
    public #ClassName#(Dictionary<string, object> _dataDic)
    {#InitContext#
        id = ID;
    }
} ";
    /// <summary>
    /// 配置表类的属性部分
    /// </summary>
    public const string proStr_Json =
    @"
    /// <summary>
    /// #ProDes#
    /// </summary>
    public #ProType# #ProName# { get; protected set; }";

    /// <summary>
    /// 配置表类的Init函数内容
    /// </summary>
    public const string classInitStr =
        @"
        #ProName# = _dataDic[""#ProName#""].#MethodName#();";

    /// <summary>
    /// 配置表加载类
    /// </summary>
    public const string loaderClassStr_PB =
@"
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ConfigLoader 
{
    #region AutoContext
    public static string BinaryPath
    {
        get
        {
            if (_binaryPath == null)
            {
                string pathLibStr = File.ReadAllText(ExcelToCSharp.LibraryPath);
                PathLibrary pathLibrary = JsonMapper.ToObject<PathLibrary>(pathLibStr);
                _binaryPath = pathLibrary.binaryPath;
            }

            return _binaryPath;
        }
    }
    private static string _binaryPath;
    private static MemoryStream tempStream = new MemoryStream(1024 * 32);
    private static MemoryStream LoadFileAsMemoryStream(string filePath)
    {
        // 读取文件内容为字节数组
        byte[] fileBytes = File.ReadAllBytes(filePath);
        // 使用文件内容初始化一个MemoryStream
        MemoryStream memoryStream = new MemoryStream(fileBytes);
        // 将MemoryStream的读取位置设置为起始位置
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
    #endregion

    #LoaderMember#
}
";
    /// <summary>
    /// 加载对应配置类方法的预置文字
    /// </summary>
    public const string loaderMember_PB =
@"
    #region #ClassName#
    private static Dictionary<int, #ClassName#> config#ClassName#Table;
    public static #ClassName# Get#ClassName#Config(int _id)
    {
        if (config#ClassName#Table == null) config#ClassName#Table = Load#ClassName#Config();
        if (!config#ClassName#Table.ContainsKey(_id)) return null;
        return config#ClassName#Table[_id];
    }
    public static List<#ClassName#> Get#ClassName#Config(Func<#ClassName#, bool> select,int count = 1) 
    {
        if (config#ClassName#Table == null) config#ClassName#Table = Load#ClassName#Config();
        return config#ClassName#Table.Values.Where(v => select(v)).Take(count).ToList();
    }
    private static Dictionary<int, #ClassName#> Load#ClassName#Config() 
    {
        Dictionary<int, #ClassName#> result = new Dictionary<int, #ClassName#>();
        string filePath = Path.Combine(BinaryPath, ""#ClassName#.bytes"");
        tempStream = LoadFileAsMemoryStream(filePath);

        tempStream.Position = 0;
        List<#ClassName#> table = ProtoBuf.Serializer.Deserialize<List<#ClassName#>>(tempStream);

        foreach (#ClassName# item in table) 
        {
            int key = item.ID;
            result[key] = item;
        }

        return result;
    }
    #endregion
";
    /// <summary>
    /// 配置表加载类
    /// </summary>
    public const string loaderClassStr_Json =
@"
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
public class ConfigLoader
{
    #region AutoContext
    public static string JsonPath  
    {
        get 
        {
            if (string.IsNullOrEmpty(_jsonPath)) 
            {
                string pathLibStr = File.ReadAllText(ExcelToCSharp.LibraryPath);
                PathLibrary pathLibrary = JsonMapper.ToObject<PathLibrary>(pathLibStr);
                _jsonPath = pathLibrary.jsonPath;
            }

            return _jsonPath;
        }
    }
    private static string _jsonPath;
    #endregion

    #LoaderMember#
}";
    /// <summary>
    /// 加载对应配置类方法的预置文字
    /// </summary>
    public const string loaderMember_Json =
    @"
    #region #ClassName#
    private static Dictionary<int, #ClassName#> config#ClassName#Table = new Dictionary<int, #ClassName#>();
    public static #ClassName# Get#ClassName#Config(int _id)
    {
        if (config#ClassName#Table.Count == 0) config#ClassName#Table = Load#ClassName#Config();
        if (!config#ClassName#Table.ContainsKey(_id)) return null;
        return config#ClassName#Table[_id];
    }
    public static List<#ClassName#> Get#ClassName#Config(Func<#ClassName#, bool> select,int count = 1) 
    {
        if (config#ClassName#Table == null) config#ClassName#Table = Load#ClassName#Config();
        return config#ClassName#Table.Values.Where(v => select(v)).Take(count).ToList();
    }
    private static Dictionary<int, #ClassName#> Load#ClassName#Config()
    {
        Dictionary<int, #ClassName#> result = new Dictionary<int, #ClassName#>();
        JsonData _data = JsonMapper.ToObject(File.ReadAllText(JsonPath + ""/#ClassName#.json""));
        for (int i = 0; i<_data.Count; i++)
        {
            int index = i;
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            foreach (string key in _data[index].Keys) pairs.Add(key, _data[index][key]);
            #ClassName# confItem = new #ClassName#(pairs);
            result.Add(confItem.id, confItem);
        }
        return result;
    }
    #endregion
";
}
