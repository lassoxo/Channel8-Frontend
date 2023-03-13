using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NewsAPI : MonoBehaviour
{

    private string URL = "http://localhost:3000/news/getScripts?limit=1&skip=0";
    //private PlayerData playerData;
    private string path = "";
    private string persistentPath = "";

    //public Text Response;

    // Start is called before the first frame update
    void Start()
    {
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "_Project/Data/" + "NewsData.json";
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "_Project/Data/" + "NewsData.json";
        StartCoroutine(GetData());
        // SetPaths();
        //CreatePlayerData();
        //SaveData();
    }

    // private void CreatePlayerData()
    // {
    //     playerData = new PlayerData("Nico", 200f, 10f, 3);
    // }

    // private void SetPaths()
    // {
    //     path = Application.dataPath + Path.AltDirectorySeparatorChar + "_Project/Data/" + "NewsData.json";
    //     persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "_Project/Data/" + "NewsData.json";
    // }

//    public void SaveData()
//     {
//         string savePath = path;

//         Debug.Log("Saving Data at " + savePath);
//         string json = JsonUtility.ToJson(playerData);
//         Debug.Log(json);

//         using StreamWriter writer = new StreamWriter(savePath);
//         writer.Write(json);
//     }

    // public void LoadData()
    // {
    //     using StreamReader reader = new StreamReader(persistentPath);
    //     string json = reader.ReadToEnd();

    //     PlayerData data = JsonUtility.FromJson<PlayerData>(json);
    //     Debug.Log(data.ToString());
    // }

    // Update is called once per frame
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.S))
    //         SaveData();

    //     if (Input.GetKeyDown(KeyCode.L))
    //         LoadData();
    // }

    IEnumerator GetData()
    {
        using(UnityWebRequest request= UnityWebRequest.Get(URL))
        {
            yield return request.SendWebRequest();

            if(request.result==UnityWebRequest.Result.ConnectionError)
                Debug.LogError(request.error);
            else
            {
                string json = request.downloadHandler.text;
                //SimpleJSON.JSONNode information = SimpleJSON.JSON.Parse(json);
                Debug.Log("News Response: ");
                Debug.Log(json);

                string savePath = path;
                using StreamWriter writer = new StreamWriter(savePath);
                writer.Write(json);
                Debug.Log("Saving Data at: ");
                Debug.Log(savePath);

                //Response.text = information;
            }
        }
    }
}
