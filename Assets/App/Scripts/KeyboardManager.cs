using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class KeyboardManager : MonoBehaviour
{
    [Header("LTouchかRTouchを選択する")]
    [SerializeField] OVRInput.Controller _controller;

    [SerializeField] TMPro.TextMeshProUGUI text;

    private TouchScreenKeyboard overlayKeyboard;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, _controller))
        {
            overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        }

        if (overlayKeyboard != null && overlayKeyboard.status == TouchScreenKeyboard.Status.Done)
        {
            FetchLLMReply(overlayKeyboard.text);
        }
    }

    void FetchLLMReply(string text)
    {
        StartCoroutine(GET($"https://hono-chat-api.marukun530.workers.dev/ai?text={text}"));
    }

    private IEnumerator GET(string URL)
    {
        using (var req = UnityWebRequest.Get(URL))
        {
            yield return req.SendWebRequest();
            if (req.isNetworkError)
            {
                Debug.Log(req.error);
            }
            else if (req.isHttpError)
            {
                Debug.Log(req.error);
            }
            else
            {
                text.SetText(req.downloadHandler.text);
            }
        }
    }
}
