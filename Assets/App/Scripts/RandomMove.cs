using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using VRM;
using OpenAI;

public class RandomMove : MonoBehaviour
{
    //会話モード(椅子固定)かどうか
    [SerializeField] bool talkMode;

    //プレイヤー
    [SerializeField] public Transform central;

    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] OpenAIChat chat;
    [SerializeField] GameObject Chair;

    //ユーザーを中心とした移動可能範囲
    [SerializeField] float radius;

    bool sitting = false;
    bool talking = false;

    NavMeshAgent agent;
    Animator anim;
    VRMBlendShapeProxy proxy;

    private void Start()
    {
        anim = GetComponent<Animator>();
        proxy = GetComponent<VRM.VRMBlendShapeProxy>();
        agent = GetComponent<NavMeshAgent>();

        agent.isStopped = false;

        //目標地点に近づいても速度を落とさなくなる
        agent.autoBraking = false;

        if (!talkMode)
        {
            StartCoroutine(Move());
        }
        else
        {
            //TalkModeではランダム移動を停止して常に座る
            sitting = true;

            Vector3 pos = gameObject.transform.position;

            Instantiate(Chair);

            agent.destination = pos;
        }
    }

    private IEnumerator RandomSit()
    {
        if (Random.Range(1, 10) == 5)
        {
            sitting = true;

            //ランダム地点を指定して椅子を設置
            float posX = Random.Range(-1 * radius, radius);
            float posZ = Random.Range(-1 * radius, radius);

            Vector3 pos = central.position;
            pos.x += posX;
            pos.z += posZ;

            Instantiate(Chair);

            agent.destination = pos;

            chat.ChairTalk();

            //60~360秒後にランダム移動を再開
            yield return new WaitForSeconds(Random.Range(60, 360));

            Destroy(GameObject.FindWithTag("Chair"));
            sitting = false;
        }
    }

    private void GotoNextPoint()
    {
        //目標地点のX軸、Z軸をランダムで決める
        float posX = Random.Range(-1 * radius, radius);
        float posZ = Random.Range(-1 * radius, radius);

        //CentralPointの位置にPosXとPosZを足す
        Vector3 pos = central.position;
        pos.x += posX;
        pos.z += posZ;

        //NavMeshAgentに目標地点を設定する
        agent.destination = pos;
    }

    private void SetEmotion(string emotion)
    {
        proxy.SetValue(BlendShapePreset.Fun, 0f);
        proxy.SetValue(BlendShapePreset.Angry, 0f);
        proxy.SetValue(BlendShapePreset.Sorrow, 0f);
        proxy.SetValue(BlendShapePreset.Joy, 0f);

        switch (emotion)
        {
            case "Joy":
                proxy.SetValue(BlendShapePreset.Joy, 1f);
                break;
            case "Fun":
                proxy.SetValue(BlendShapePreset.Fun, 1f);
                break;
            case "Angry":
                proxy.SetValue(BlendShapePreset.Angry, 1f);
                break;
            case "Sorrow":
                proxy.SetValue(BlendShapePreset.Sorrow, 1f);
                break;
        }
    }

    public IEnumerator TalkWithPlayer(string Emotion)
    {
        talking = true;

        //表情をセット
        SetEmotion(Emotion);

        yield return new WaitForSeconds(25f);

        talking = false;
    }

    private IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(10, 15));

            if (!sitting)
            {
                GotoNextPoint();
                StartCoroutine(RandomSit());
            }
        }
    }

    private void Update()
    {
        //座り時の処理
        if (anim && sitting && agent.remainingDistance < 0.01f)
        {
            anim.SetBool("Sit", true);

            Vector3 pos = gameObject.transform.position;
            pos.y += 0.2f;

            GameObject.FindWithTag("Chair").transform.position = pos; //実機環境ではなぜか椅子の位置がずれるので常にモデル位置に固定
        }
        else
        {
            //NavMeshAgentのスピードの2乗で歩きアニメーション
            anim.SetFloat("Forward", agent.velocity.sqrMagnitude);
        }

        //会話中は常にプレイヤーを向く
        if (talking)
        {
            transform.LookAt(new Vector3(central.position.x, transform.position.y, central.position.z));
        }
    }
}
