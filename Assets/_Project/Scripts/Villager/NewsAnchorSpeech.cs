using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NewsAnchorSpeech : MonoBehaviour
{
    private int newsLimit = 2; // Number of news topics to load at a time (Adjustable)
    private string url;
    private string path;
    private string persistentPath;
    private SimpleJSON.JSONNode information;

    private bool FirstTimeLoading = true;
    private bool Cached = false;
    private bool Loaded = false;
    private int TotalNewsTopics = 0;
    private int CurrentNewsTopic = 0;
    private int CumulativeNewsTopics = 0;

    // Start is called before the first frame update
    void Start()
    {   
        // Initial url
        url = "http://localhost:3000/news/getScripts?limit=" + newsLimit.ToString() + "&skip=0";
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
            // Delete all previous audio files in the audio folder
        }

        // If file have stuff in it & loaded === false then -> use load data method to cache data and update loaded === true & delete the file
        if (System.IO.File.ReadAllText(persistentPath) != "" && Cached == false)
        {
            //Update url considering CumulativeNewsTopics
            CumulativeNewsTopics = CumulativeNewsTopics + newsLimit;
            url = "http://localhost:3000/news/getScripts?limit=" + newsLimit.ToString() + "&skip=" + CumulativeNewsTopics.ToString();
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

        // If currentnewstopics === totalnewstopics then make cached == false && reset news count to 0
        if (CurrentNewsTopic == TotalNewsTopics && Cached == true)
        {
            Cached = false;
            CurrentNewsTopic = 0;
        }
    }

    // if the audio match the name of the character the character then will play the audio with relevant emotion
    // other characters in the anchor room will face the talking character on idle

    IEnumerator GetData()
    {
        using(UnityWebRequest request= UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if(request.result==UnityWebRequest.Result.ConnectionError)
                Debug.LogError(request.error);
            else
            {
                string Json = request.downloadHandler.text;
                Debug.Log("News Response: ");
                Debug.Log(Json);

                // Save the data to a file
                string SavePath = persistentPath;
                using StreamWriter writer = new StreamWriter(SavePath);
                writer.Write(Json);
                Debug.Log("Saving Data at: ");
                Debug.Log(SavePath);

                // Download all the audio files in a seperate folder along the same path, yield to wait until its all downloaded
            }
        }
    }

    public void LoadData()
    {
        using StreamReader reader = new StreamReader(persistentPath);
        string json = reader.ReadToEnd();
        information = SimpleJSON.JSON.Parse(json);        
        TotalNewsTopics = int.Parse(information["count"].Value);

        Debug.Log("Cached News Response: ");
        Debug.Log(information);

        StartCoroutine(DownloadAndPlayAudioSequentially());
    }

    IEnumerator DownloadAndPlayAudioSequentially() // Get MP3 from file then delete it once its played, add wait time between speech
    {
        //1.Loop through each AudioClip
        for (int i = 0; i < TotalNewsTopics;)
        {
            for (int j = 0; j < information["scripts"][i]["lines"].Count; j++)
            {
                //2.Assign current AudioClip to audiosource            
                WWW www = new WWW(information["scripts"][i]["lines"][j]["fileUrl"].Value);
                yield return www;
                AudioSource audio = GetComponent<AudioSource>();
                audio.clip = www.GetAudioClip(false, true,AudioType.MPEG);

                //3.Play Audio
                audio.Play();
                Debug.Log("Audio Being Played: ");
                Debug.Log(information["scripts"][i]["lines"][j]["fileUrl"].Value);

                //4.Wait for it to finish playing
                while (audio.isPlaying)
                {
                    yield return null;
                }

                //5. Go back to #2 and play the next audio in the adClips array
            }
            i++;
            CurrentNewsTopic = i;            
        }
        CumulativeNewsTopics = CumulativeNewsTopics + newsLimit;
    }
}
