using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NewsAnchorSpeech : MonoBehaviour
{
    private string URL = "http://localhost:3000/news/getScripts?limit=1&skip=0";
    private string path = "";
    private string persistentPath = "";
    private bool FirstTimeLoading = true;
    private bool Cached = false;
    private bool Loaded = false;
    private SimpleJSON.JSONNode information;
    private int NewsTopics = 0;
    private int CurrentNewsTopic = 0;
    private int CurrentNewsConversationThreads = 0;
    private int CurrentNewsCurrentConversationThread = 0;

    // Start is called before the first frame update
    void Start()
    {   
        // File path for debugging
        path = Application.dataPath + Path.AltDirectorySeparatorChar + "_Project/Data/" + "NewsData.json";
        // File path for production
        persistentPath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "NewsData.json";
        StartCoroutine(GetData());
    }

    // Update is called once per frame
    void Update()
    {
        // If first time loading = true then clear news data.json then make first time loading false
        if (FirstTimeLoading == true)
        {
            // Clear news data.json
            System.IO.File.WriteAllText(persistentPath, string.Empty);
            FirstTimeLoading = false;
        }

        // If file have stuff in it & loaded === false then -> use load data method to cache data and update loaded === true & delete the file
        if (System.IO.File.ReadAllText(persistentPath) != "" && Cached == false)
        {
            LoadData();
            Cached = true;
            Loaded = false;
            System.IO.File.WriteAllText(persistentPath, string.Empty);
        }

        // If cached == true & loaded == false then get data & make loaded == true
        if (Cached == true && Loaded == false)
        {
            //get news data
            StartCoroutine(GetData());
            //making loaded true
            Loaded = true;
        }

        // If currentnewstopics === newstopics then make cached == false && reset news count to 0
        if (CurrentNewsTopic == NewsTopics && Cached == true)
        {
            Cached = false;
            CurrentNewsTopic = 0;
        }
    }

    // if the audio match the name of the character the character then will play the audio with relevant emotion
    // other characters in the anchor room will face the talking character on idle

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
                Debug.Log("News Response: ");
                Debug.Log(json);

                // Save the data to a file
                string savePath = persistentPath;
                using StreamWriter writer = new StreamWriter(savePath);
                writer.Write(json);
                Debug.Log("Saving Data at: ");
                Debug.Log(savePath);
            }
        }
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();
        information = SimpleJSON.JSON.Parse(json);        
        NewsTopics = int.Parse(information["count"].Value);

        Debug.Log("Cached News Response: ");
        Debug.Log(information);

        CurrentNewsConversationThreads = information["scripts"][CurrentNewsTopic]["lines"].Count;
        
        // Conversation generation on a loop
        for (int i = 0; i < CurrentNewsConversationThreads; i++)
        {
            StartCoroutine(DownloadAudio(information["scripts"][CurrentNewsTopic]["lines"][i]["fileUrl"].Value)); 
            // Debug.Log(information["scripts"][CurrentNewsTopic]["lines"][i]["name"]);
            // Debug.Log(information["scripts"][CurrentNewsTopic]["lines"][i]["text"]);
            // Debug.Log(information["scripts"][CurrentNewsTopic]["lines"][i]["fileUrl"]);
        }

        // string actionTwo = information["scripts"][0]["lines"][0]["name"].Value;
        // string actionTwo = information["scripts"][0]["lines"].Count;

        // foreach (SimpleJSON.JSONNode item in information["scripts"].AsArray)
        // {
        //     //Debug.Log(item["lines"][0]["name"]);
        //     //do another foreach loop here to get the rest of the data
        //     foreach (SimpleJSON.JSONNode itemTwo in item["lines"].AsArray)
        //     {
        //         Debug.Log(itemTwo["name"]);
        //         Debug.Log(itemTwo["text"]);
        //         Debug.Log(itemTwo["fileUrl"]);
        //     }
        //     //check the length of item["lines"] to see how many lines there are
        //     //Debug.Log(item["lines"].Count);
        // }
    }

    IEnumerator DownloadAudio(string url)
    {
        WWW www = new WWW(url);
        yield return www;
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = www.GetAudioClip(false, true,AudioType.MPEG);
        audio.Play();
    }
}
