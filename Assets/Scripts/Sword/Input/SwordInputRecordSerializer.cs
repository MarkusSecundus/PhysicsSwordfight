using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using JsonSerializer = Newtonsoft.Json.JsonConvert;

public class SwordInputRecordSerializer : MonoBehaviour
{
    public string filePattern = "record", fileExtension = ".json";
    private int counter = 1;

    public void SaveRecording(List<SwordInputRecorder.Frame> toSave)
    {
        
        var json = JsonSerializer.SerializeObject(toSave, Newtonsoft.Json.Formatting.Indented);

        Debug.Log($"Saving record with length {toSave.Count}");

        var path = Path.GetFullPath(filePattern + counter + fileExtension);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        using var file = new StreamWriter(path);
        ++counter;
        file.WriteLine(json);
        Debug.Log($"Saved record to file {path}");
    }
}
