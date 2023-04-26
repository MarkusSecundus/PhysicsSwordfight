using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using JsonSerializer = Newtonsoft.Json.JsonConvert;

namespace MarkusSecundus.PhysicsSwordfight.Input
{

    public class SwordInputRecordDeserializer : MonoBehaviour
    {
        public string[] paths;
        public SwordInputRecordPlayer[] players;

        private void Start()
        {
            foreach (var record in paths.Select(LoadRecording))
                foreach (var player in players)
                    player.AddRecord(record);
        }

        private static List<SwordInputRecorder.Frame> LoadRecording(string path)
        {
            using var rdr = new StreamReader(path);
            return JsonSerializer.DeserializeObject<List<SwordInputRecorder.Frame>>(rdr.ReadToEnd());
        }
    }
}