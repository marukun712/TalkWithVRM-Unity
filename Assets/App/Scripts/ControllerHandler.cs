using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerHandler : MonoBehaviour
{
    [SerializeField] OVRInput.Controller LController;
    [SerializeField] OVRInput.Controller RController;

    [SerializeField] GameObject Menu;
    [SerializeField] GameObject Pointer;
    [SerializeField] OpenAIChat chat;

    private TouchScreenKeyboard overlayKeyboard;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A, RController))
        {
            StartCoroutine(WaitForReturnKey());
        }
        if (OVRInput.GetDown(OVRInput.RawButton.B, RController))
        {
            Menu.SetActive(!Menu.activeSelf);
            Pointer.SetActive(!Pointer.activeSelf);
        }
    }

    private IEnumerator WaitForReturnKey()
    {
        TouchScreenKeyboard keyboard = TouchScreenKeyboard.Open("",
            TouchScreenKeyboardType.Default, true, true);

        yield return new WaitUntil(() => keyboard.text.Contains(System.Environment.NewLine));
        string result = keyboard.text.Replace(System.Environment.NewLine, "");

        if (result != "")
        {
            StartCoroutine(chat.SendRequestToOpenAI(result));
        }

        keyboard.active = false;
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
