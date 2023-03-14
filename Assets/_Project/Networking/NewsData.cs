using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
public class Line
{
    public string name { get; set; }
    public string text { get; set; }
    public string effect { get; set; }
    public string fileUrl { get; set; }
}

public class Root
{
    public int count { get; set; }
    public List<Script> scripts { get; set; }
}

public class Script
{
    public string _id { get; set; }
    public List<string> images { get; set; }
    public List<Line> lines { get; set; }
    public string originalPrompt { get; set; }
    public int __v { get; set; }
}