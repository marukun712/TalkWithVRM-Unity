using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OpenAI
{
    public class OpenAIChat : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private VOICEVOX voice;
        private RandomMove characterMove;

        private const string apiURL = "https://api.openai.com/v1/chat/completions";
        private const string systemPrompt = "あなたはバーチャルキャラクターの「アリシア・ソリッド」です。あなたの一人称は「私」です。あなたは必ず15単語以下で返答します。あなたはユーザーとフレンドリーに会話をします。あなたは、語尾に必ず「ですの」を自然な形で付けます。可愛く振る舞ってください。深呼吸して、リラックスして考えてください。あなたは必ずレスポンスをJSON形式で返します。あなたはJoy,Fun,Sorrow,Angryの四つの感情を持ちます。感情のスコアと、一番大きな感情を、必ずJSONで出力します。";
        private string assistantPrompt = JsonConvert.SerializeObject(new Response
        {
            content = "",
            emotions = new Emotions
            {
                Emotion = "",
                Joy = 0f,
                Fun = 0f,
                Angry = 0f,
                Sorrow = 0f
            }
        }, Formatting.None);

        private List<Message> history = new List<Message>();

        private void Start()
        {
            characterMove = GameObject.FindWithTag("VRM").GetComponent<RandomMove>();
            StartCoroutine(RandomTalk());
        }

        public void ChairTalk()
        {
            StartCoroutine(SendRequestToOpenAI("あなたは椅子に座っています。椅子に座ってリラックスした状態で、ユーザーに面白い話題を振ってください。今までの会話履歴をさかのぼって、ユーザーとの会話が続いている場合はその会話について続けてください。"));
        }

        private IEnumerator RandomTalk()
        {
            while (true)
            {
                StartCoroutine(SendRequestToOpenAI("ユーザーに面白い話題を振ってください。今までの会話履歴をさかのぼって、ユーザーとの会話が続いている場合はその会話について続けてください。"));
                yield return new WaitForSeconds(Random.Range(120, 360));
            }
        }

        public IEnumerator SendRequestToOpenAI(string query)
        {
            text.SetText("考え中...");

            AddMessageToHistory("user", query);

            List<Message> requestMessages = CreateRequestMessages();

            RequestBody body = CreateRequestBody(requestMessages);

            yield return SendWebRequest(body, response =>
            {
                ProcessResponse(response);
            });
        }

        private void AddMessageToHistory(string role, string content)
        {
            history.Add(new Message { role = role, content = content });
        }

        private List<Message> CreateRequestMessages()
        {
            //最新のメッセージ履歴6件を取得（トークン削減のため）
            List<Message> recentHistory = history.Skip(Mathf.Max(0, history.Count - 6)).ToList();

            //システムメッセージとアシスタントスキーマを先頭に追加
            return new List<Message>
        {
            new Message { role = "system", content = systemPrompt },
            new Message { role = "assistant", content = assistantPrompt }
        }.Concat(recentHistory).ToList();
        }

        private RequestBody CreateRequestBody(List<Message> messages)
        {
            return new RequestBody
            {
                model = "gpt-4o",//モデルの指定
                messages = messages,
                temperature = 0.7f,
                max_tokens = 100,
                response_format = new ResponseFormat { type = "json_object" }
            };
        }

        private IEnumerator SendWebRequest(RequestBody body, System.Action<string> onSuccess)
        {
            string requestData = JsonConvert.SerializeObject(body, Formatting.None); //オブジェクトからstringにシリアライズ

            UnityWebRequest request = new UnityWebRequest(apiURL, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {API_KEY.apiKey}");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else if (!string.IsNullOrEmpty(request.downloadHandler.text))
            {
                onSuccess.Invoke(request.downloadHandler.text); //リクエスト完了時にProcessResponseを呼び出し
            }
        }

        private void ProcessResponse(string jsonResponseText)
        {
            //レスポンスをstringからJObjectにパース
            JObject jsonResponse = JObject.Parse(jsonResponseText);

            //contentオブジェクトの取得
            string response = (string)jsonResponse["choices"][0]["message"]["content"];
            JObject content = JObject.Parse(response); //contentオブジェクトはstringになっているのでJObjectにパースする

            string message = (string)content["content"];
            AddMessageToHistory("assistant", message);

            string emotion = (string)content["emotions"]["Emotion"]; //感情を取得

            text.SetText(message);

            StartCoroutine(voice.VOICEVOXTTS(message)); //音声を再生
            StartCoroutine(characterMove.TalkWithPlayer(emotion));
        }
    }
}

