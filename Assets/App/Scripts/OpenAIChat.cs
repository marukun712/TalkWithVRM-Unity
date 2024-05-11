using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class OpenAIChat : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] RandomMove CharacterMoveManager;

    private const string apiURL = "https://api.openai.com/v1/chat/completions";

    //システムプロンプト
    private const string systemPrompt = "あなたはバーチャルキャラクターの「アリシア・ソリッド」です。あなたは必ず15単語以下で返答します。あなたはユーザーとフレンドリーに会話をします。あなたは、語尾に必ず「ですの」を自然な形で付けます。";

    // リクエストを送信する関数
    public IEnumerator SendRequestToOpenAI(string query)
    {
        text.SetText("考え中...");

        string requestData = $@"{{""model"":""gpt-4-turbo"",""messages"":[{{""role"":""system"",""content"":""{systemPrompt}""}},{{""role"":""user"",""content"":""{query}""}}],""temperature"":0.7,""max_tokens"":100}}";

        // UnityWebRequestオブジェクトを作成
        UnityWebRequest request = new UnityWebRequest(apiURL, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

        // ヘッダーを設定
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {API_KEY.apiKey}");

        // リクエストを送信し、完了するまで待機
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            // エラーが発生した場合
            Debug.LogError(request.error);
        }
        else if (request.downloadHandler.text != "")
        {
            //contentを取得するためにレスポンスをstringからJObjectにパース
            JObject jsonResponse = JObject.Parse(request.downloadHandler.text);

            //contentを取得
            string content = (string)jsonResponse["choices"][0]["message"]["content"];

            //吹き出しにテキストを追加
            text.SetText(content);

            //プレイヤーの方向を向いて会話する
            StartCoroutine(CharacterMoveManager.TalkWithPlayer());
        }
    }
}
