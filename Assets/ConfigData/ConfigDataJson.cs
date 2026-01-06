using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ConfigDataJson<T> : IConfigDataHandler<T> where T : BaseConfig, new()
{
    private string tablePath;
    private Dictionary<int, T> configTable;
    public ConfigDataJson() { }
    public void SetTablePath(string path) => tablePath = path;
    public T GetConfigData(int id)
    {
        configTable ??= LoadConfigTable();
        if (!configTable.ContainsKey(id)) return null;
        return configTable[id];
    }
    public List<T> GetConfigData(Func<T, bool> select, int count = 1)
    {
        configTable ??= LoadConfigTable();
        return configTable.Values.Where(v => select(v)).Take(count).ToList();
    }
    public Dictionary<int, T> LoadConfigTable()
    {
        Dictionary<int, T> result = new Dictionary<int, T>();
        string json = File.ReadAllText(tablePath);
        JsonData _data = JsonMapper.ToObject(File.ReadAllText(tablePath));
        for (int i = 0; i < _data.Count; i++)
        {
            int index = i;
            Dictionary<string, object> pairs = new Dictionary<string, object>();
            foreach (string key in _data[index].Keys) pairs.Add(key, _data[index][key]);
            T confItem = new T();
            confItem.Initialize(pairs);
            result.Add(confItem.id, confItem);
        }
        return result;
    }
}

