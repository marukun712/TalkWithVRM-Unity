using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using VRM;

public class RandomMove : MonoBehaviour
{
    [SerializeField] public Transform central;
    [SerializeField] VRMBlendShapeProxy proxy;

    [SerializeField] TMPro.TextMeshProUGUI text;
    [SerializeField] OpenAIChat chat;
    [SerializeField] GameObject Chair;

    NavMeshAgent agent;
    [SerializeField] float radius = 3;

    bool sitting = false;
    bool talking = false;

    Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        agent.isStopped = false;

        //目標地点に近づいても速度を落とさなくなる
        agent.autoBraking = false;

        StartCoroutine(Move());
    }

    private IEnumerator RandomSit()
    {
        if (Random.Range(1, 10) == 5)
        {
            sitting = true;

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

            anim.SetBool("Sit", false);
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

    //会話時にプレイヤーの方向を向く
    public IEnumerator TalkWithPlayer()
    {
        talking = true;
        proxy.SetValue(BlendShapePreset.Joy, 1f);

        yield return new WaitForSeconds(25f);

        proxy.SetValue(BlendShapePreset.Joy, 0f);
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
        if (anim && sitting && agent.remainingDistance < 0.01f)
        {
            anim.SetBool("Sit", true);

            Vector3 pos = gameObject.transform.position;
            pos.y += 0.2f;

            GameObject.FindWithTag("Chair").transform.position = pos;
        }
        else
        {
            //NavMeshAgentのスピードの2乗でアニメーションを切り替える
            anim.SetFloat("Forward", agent.velocity.sqrMagnitude);
        }

        if (talking)
        {
            transform.LookAt(new Vector3(central.position.x, transform.position.y, central.position.z));
        }
    }
}
