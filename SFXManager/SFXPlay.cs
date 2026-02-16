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
public class CPHInline
{
    public bool Execute()
    {
        CPH.TryGetArg("SFX_Folder", out string dir);
        string jsonFilePath = $"{dir}/sfx.json";

        if (File.Exists(jsonFilePath))
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            List<SFX> SfxList = JsonConvert.DeserializeObject<List<SFX>>(jsonString);
            CPH.TryGetArg("input0", out string commandString);
            string soundToPlay = SfxList.Where(x => x.Trigger == commandString).FirstOrDefault().Path;
            CPH.PlaySound(soundToPlay);
        }



        return true;
    }

}