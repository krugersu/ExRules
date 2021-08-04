using Newtonsoft.Json;

using System.IO;


namespace ExRules
{


    internal class mSettings

    {

        public string PathLog { get; set; }
        public string jsonString { get; set; }
        // public mSettings()
        // {
        //     string jsonString = File.ReadAllText("setting.json");
        //  JsonConvert.DeserializeObject<mSettings>(jsonString);
        // }
    }
}