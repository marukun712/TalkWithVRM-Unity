using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OpenAI;

public class ControllerHandler : MonoBehaviour
{
    [SerializeField] OVRInput.Controller LController;
    [SerializeField] OVRInput.Controller RController;

    [SerializeField] GameObject Menu;
    [SerializeField] GameObject Pointer;
    [SerializeField] Button voiceButton;
    [SerializeField] Text voiceText;
    [SerializeField] OpenAIChat chat;

    private bool isPushedButton = false;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A, RController)) //音声認識
        {
            if (isPushedButton)
            {
                StartCoroutine(chat.SendRequestToOpenAI(voiceText.text)); //認識結果を確定
                isPushedButton = false;
            }
            else
            {
                voiceButton.onClick.Invoke(); //音声認識を開始
                isPushedButton = true;
            }
        }
        if (OVRInput.GetDown(OVRInput.RawButton.B, RController))
        {
            //メニューとレーザーの表示/非表示
            Menu.SetActive(!Menu.activeSelf);
            Pointer.SetActive(!Pointer.activeSelf);
        }
    }

    /* CloudFlare Workersの無料枠を使った実装
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
    */
}
