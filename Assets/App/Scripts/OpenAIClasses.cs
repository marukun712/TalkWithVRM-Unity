using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//OpenAI APIのリクエストに使うJSONのスキーマ

[JsonObject]
class Message
{
    [JsonProperty("role")]
    public string role { get; set; }

    [JsonProperty("content")]
    public string content { get; set; }
}

class ResponseFormat
{
    [JsonProperty("type")]
    public string type { get; set; }
}

class RequestBody
{
    [JsonProperty("model")]
    public string model { get; set; }

    [JsonProperty("messages")]
    public List<Message> messages { get; set; }

    [JsonProperty("temperature")]
    public float temperature { get; set; }

    [JsonProperty("max_tokens")]
    public int max_tokens { get; set; }

    [JsonProperty("response_format")]
    public ResponseFormat response_format { get; set; }
}

class Response
{
    [JsonProperty("content")]
    public string content { get; set; }

    [JsonProperty("emotions")]
    public Emotions emotions { get; set; }
}

class Emotions
{
    [JsonProperty("Emotion")]
    public string Emotion { get; set; }

    [JsonProperty("Joy")]
    public float Joy { get; set; }

    [JsonProperty("Fun")]
    public float Fun { get; set; }

    [JsonProperty("Angry")]
    public float Angry { get; set; }

    [JsonProperty("Sorrow")]
    public float Sorrow { get; set; }
}