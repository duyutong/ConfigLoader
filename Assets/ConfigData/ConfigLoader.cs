
using LitJson;
using System.Collections.Generic;
using System.IO;

public class ConfigLoader 
{
    public static string binaryPath
    {
        get
        {
            if (_binaryPath == null)
            {
                string pathLibStr = File.ReadAllText("D:/Study/ConfigLoader/Assets/ConfigData/PathLibrary.json");
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
    
    #region MyNewExcel
    private static Dictionary<int, MyNewExcel> configMyNewExcelTable = new Dictionary<int, MyNewExcel>();
    public static MyNewExcel GetMyNewExcelConfig(int _id)
    {
        if (configMyNewExcelTable.Count == 0) configMyNewExcelTable = LoadMyNewExcelConfig();
        if (!configMyNewExcelTable.ContainsKey(_id)) return null;
        return configMyNewExcelTable[_id];
    }
    private static Dictionary<int, MyNewExcel> LoadMyNewExcelConfig() 
    {
        Dictionary<int, MyNewExcel> result = new Dictionary<int, MyNewExcel>();
        string filePath = Path.Combine(binaryPath, "MyNewExcel.bytes");
        tempStream = LoadFileAsMemoryStream(filePath);

        tempStream.Position = 0;
        List<MyNewExcel> table = ProtoBuf.Serializer.Deserialize<List<MyNewExcel>>(tempStream);

        foreach (MyNewExcel item in table) 
        {
            int key = item.ID;
            result[key] = item;
        }

        return result;
    }
    #endregion

}
