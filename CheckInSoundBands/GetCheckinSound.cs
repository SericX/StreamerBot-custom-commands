using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class SFX
{
    public int CounterFrom { get; set; }
    public int CounterTo { get; set; }
    public string Path { get; set; }

    public SFX(int counterFrom, int counterTo, string path)
    {
        CounterFrom = counterFrom;
        CounterTo = counterTo;
        Path = path;
    }
}

public class CPHInline
{
    public bool Execute()
    {
        CPH.TryGetArg("userCounter", out int counter);
        CPH.TryGetArg("jsonFilePath", out string jsonFilePath);
        List<SFX> Json_Checkin_SFX_List = GetCheckinSFXRecordsFromJson(jsonFilePath);
        SFX sfxToPlay = Json_Checkin_SFX_List.Where(x => counter >= x.CounterFrom && counter <= x.CounterTo).FirstOrDefault();
        CPH.SetArgument("SFXToPlay", sfxToPlay.Path);
        return true;
    }

    private List<SFX> GetCheckinSFXRecordsFromJson(string jsonFile)
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