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
        CPH.TryGetArg("SFX_Folder", out string sfxPath);
        string jsonFilePath = $"{sfxPath}\\sfx.json";
        List<SFX> Json_SFX_List = GetSFXRecordsFromJson(jsonFilePath);
        var paginator = new SfxPaginator(Json_SFX_List);
        int page = 1;
        try
        {
            CPH.TryGetArg("input0", out int inputPage);
            page = inputPage;
        }
        catch
        {
            page = 1;
        }

        var pageStr = paginator.GetSFXPage(page);
        if (pageStr == "")
        {
            CPH.SendMessage("No SFX found on this page...");
        }
        else
        {
            CPH.SendMessage($"SFX Page {page} : {pageStr}");
        }

        return true;
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

// --- Helper Class to Manage Pagination ---
public class SfxPaginator
{
    private const int MaxPageLength = 450;
    private readonly Lazy<string> _fullTriggerString;
    // The list of all SFX records
    private readonly List<SFX> _sfxList;
    public SfxPaginator(List<SFX> sfxList)
    {
        _sfxList = sfxList ?? new List<SFX>();
        // Use Lazy<T> to build the single, long string only when first accessed.
        _fullTriggerString = new Lazy<string>(() =>
        {
            // Join all Trigger values into one long string, separated by ", "
            return string.Join(", ", _sfxList.Select(s => s.Trigger));
        });
    }

    /// <summary>
    /// Returns a string page of SFX triggers, up to 500 characters long.
    /// </summary>
    /// <param name = "page">The page number (1-based).</param>
    /// <returns>A comma-separated string of triggers for the requested page, or an empty string.</returns>
    public string GetSFXPage(int page)
    {
        // Get the single, concatenated string of all triggers
        string fullString = _fullTriggerString.Value;
        // Calculate the starting character index (0-based)
        int startIndex = (page - 1) * MaxPageLength;
        // Validate the page number
        if (page < 1 || startIndex >= fullString.Length)
        {
            return string.Empty; // Requested page is out of bounds
        }

        // --- Core Pagination Logic ---
        // 1. Calculate the maximum length remaining in the string from the startIndex.
        int remainingLength = fullString.Length - startIndex;
        // 2. Determine the length of the current slice (MaxPageLength or less).
        int lengthToTake = Math.Min(MaxPageLength, remainingLength);
        // 3. Take the raw slice of the string.
        string rawSlice = fullString.Substring(startIndex, lengthToTake);
        // 4. Clean up the slice for the final output.
        // If the slice ends exactly on a comma-space (", "), remove it to avoid
        // an incomplete trigger at the end of the page and a leading ", " on the next.
        if (rawSlice.EndsWith(", "))
        {
            return rawSlice.Substring(0, rawSlice.Length - 2);
        }

        // If the slice ends inside a trigger, or inside a delimiter, we must 
        // find the last complete delimiter (", ") and return everything before it.
        if (startIndex + MaxPageLength < fullString.Length) // Only necessary if this isn't the last page
        {
            int lastDelimiterIndex = rawSlice.LastIndexOf(", ");
            if (lastDelimiterIndex != -1)
            {
                // Return everything up to (but not including) the last delimiter
                return rawSlice.Substring(0, lastDelimiterIndex);
            }
        }

        // For the last page, or if the slice is perfectly aligned/small
        return rawSlice;
    }

    /// <summary>
    /// Calculates the total number of pages available.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)_fullTriggerString.Value.Length / MaxPageLength);
}