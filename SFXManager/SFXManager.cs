using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SFX
{
    public string Trigger { get; private set; }
    public string Path { get; private set; }

    public SFX(string trigger, string path)
    {
        Trigger = trigger;
        Path = path;
    }
}

public class SFXPathComparer : IEqualityComparer<SFX>
{
    public bool Equals(SFX x, SFX y)
    {
        if (ReferenceEquals(x, y))
            return true;
        if (ReferenceEquals(x, null) || ReferenceEquals(y, null))
            return false;
        return string.Equals(x.Path, y.Path, StringComparison.OrdinalIgnoreCase);
    }

    public int GetHashCode(SFX obj)
    {
        if (obj is null || obj.Path is null)
            return 0;
        return obj.Path.ToLowerInvariant().GetHashCode();
    }
}

public class CPHInline
{
    public bool Execute()
    {
        CPH.SendMessage("Attempting to update SFX list...");
        try
        {
            CPH.TryGetArg("SFX_Folder", out string sfxPath);
            string jsonFilePath = $"{sfxPath}\\sfx.json";
            List<SFX> SFX_List = GetSFXFilesFromDir(sfxPath);
            List<SFX> Json_SFX_List = GetSFXRecordsFromJson(jsonFilePath);
            var pathComparer = new SFXPathComparer();
            List<SFX> recordsToRemove = Json_SFX_List.Except(SFX_List, pathComparer).ToList();
            List<SFX> recordsToAdd = SFX_List.Except(Json_SFX_List, pathComparer).ToList();
            List<SFX> keptRecords = Json_SFX_List.Except(recordsToRemove, pathComparer).ToList();
            List<SFX> finalSFXList = keptRecords.Concat(recordsToAdd).ToList();
            UpdateSFXJson(jsonFilePath, finalSFXList);
            CPH.SendMessage("SFX list updated!");
        }
        catch
        {
            CPH.SendMessage("SFX List update failed D:");
        }

        return true;
    }

    private void UpdateSFXJson(string jsonFilePath, List<SFX> newList)
    {
        string jsonStr = JsonConvert.SerializeObject(newList, Formatting.Indented);
        File.WriteAllText(jsonFilePath, jsonStr);
    }

    private List<SFX> GetSFXFilesFromDir(string directory)
    {
        List<SFX> SFX_List = new List<SFX>();
        string[] sfx_array = Directory.GetFiles(directory, "*.mp3");
        foreach (var sfx in sfx_array)
        {
            SFX w = new SFX(Path.GetFileNameWithoutExtension(sfx), sfx);
            SFX_List.Add(w);
        }

        return SFX_List;
    }

    private List<SFX> GetSFXRecordsFromJson(string jsonFile)
    {
        if (!File.Exists(jsonFile))
        {
            return new List<SFX>();
        }

        var json = File.ReadAllText(jsonFile);
        var jsonList = JsonConvert.DeserializeObject<List<SFX>>(json);
        return jsonList;
    }
}