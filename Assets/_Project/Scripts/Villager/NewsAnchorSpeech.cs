using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewsAnchorSpeech : MonoBehaviour
{
    private string path = "";
    private string persistentPath = "";

    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "_Project/Data/" + "NewsData.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "_Project/Data/" + "NewsData.json";
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(path);
        string json = reader.ReadToEnd();
        SimpleJSON.JSONNode information = SimpleJSON.JSON.Parse(json);
        string action = information["count"].Value;
        //var data = JsonConvert.DeserializeObject<Root>(json);
        //Root data = JsonUtility.FromJson<Root>(json.count);
        //Root data = JsonConvert.DeserializeObject<Root>(json);

        Debug.Log(action);
    }

    // pull a list of scripts from the json file
    // download the respective audio matching the script
    // if the audio match the name of the character the character then will play the audio with relevant emotion
    // other characters in the anchor room will face the talking character on idle

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            LoadData();
    }
}
