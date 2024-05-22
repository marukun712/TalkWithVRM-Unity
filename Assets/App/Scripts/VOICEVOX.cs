using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

public class VOICEVOX : MonoBehaviour
{
    // 音声再生用のAudioSourceコンポーネント
    [SerializeField] AudioSource audioSource;

    public IEnumerator VOICEVOXTTS(string text)
    {
        // APIにリクエストを送信
        using (UnityWebRequest request = UnityWebRequest.Get($"https://api.tts.quest/v3/voicevox/synthesis?text={text}&speaker=20"))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + request.error);
                yield break;
            }

            // レスポンスをパースしてダウンロードURLとステータスURLを取得
            var response = JsonConvert.DeserializeObject<VoiceVoxResponse>(request.downloadHandler.text);
            if (response.success)
            {
                string audioStatusUrl = response.audioStatusUrl;

                // 音声が準備できるまで待機
                bool isAudioReady = false;
                while (!isAudioReady)
                {
                    using (UnityWebRequest statusRequest = UnityWebRequest.Get(audioStatusUrl))
                    {
                        yield return statusRequest.SendWebRequest();

                        if (statusRequest.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError($"Error fetching audio status: {statusRequest.error}");
                            yield break;
                        }

                        var audioStatus = JsonConvert.DeserializeObject<AudioStatus>(statusRequest.downloadHandler.text);
                        if (audioStatus != null)
                        {
                            isAudioReady = audioStatus.isAudioReady;
                            if (!isAudioReady)
                            {
                                // 少し待機して再度チェック
                                yield return new WaitForSeconds(1);
                            }
                        }
                        else
                        {
                            Debug.LogError("Invalid audio status response");
                            yield break;
                        }
                    }
                }

                // 音声ファイルをダウンロードして再生
                string audioUrl = response.wavDownloadUrl;
                using (UnityWebRequest audioRequest = UnityWebRequestMultimedia.GetAudioClip(audioUrl, AudioType.WAV))
                {
                    yield return audioRequest.SendWebRequest();

                    if (audioRequest.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError("Error downloading audio: " + audioRequest.error);
                        yield break;
                    }

                    AudioClip audioClip = DownloadHandlerAudioClip.GetContent(audioRequest);
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
            }
            else
            {
                Debug.LogError("API response indicates failure");
            }
        }
    }

    // レスポンスのJSONデータをパースするためのクラス
    public class VoiceVoxResponse
    {
        public bool success { get; set; }
        public string wavDownloadUrl { get; set; }
        public string audioStatusUrl { get; set; }
    }

    // 音声ステータスのJSONデータをパースするためのクラス
    public class AudioStatus
    {
        public bool isAudioReady { get; set; }
    }
}
