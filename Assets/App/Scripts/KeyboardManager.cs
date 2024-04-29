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

    [SerializeField] RandomMove CharacterMoveManager;

    private TouchScreenKeyboard overlayKeyboard;

    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, _controller))
        {
            overlayKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        }

        //文字入力完了時の処理
        if (overlayKeyboard != null && overlayKeyboard.status == TouchScreenKeyboard.Status.Done)
        {
            FetchLLMReply(overlayKeyboard.text);
        }
    }

    void FetchLLMReply(string text)
    {
        //Workers AIのAPIをFetch
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
            else if (req.downloadHandler.text != "")
            {
                //吹き出しにテキストを追加
                text.SetText(req.downloadHandler.text);

                //プレイヤーの方向を向いて会話する
                CharacterMoveManager.TalkWithPlayer();
            }
        }
    }
}
