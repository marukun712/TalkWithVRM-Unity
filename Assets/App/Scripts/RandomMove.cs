using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomMove : MonoBehaviour
{
    [SerializeField] Transform central;
    [SerializeField] GameObject target;

    [SerializeField] TMPro.TextMeshProUGUI text;

    NavMeshAgent agent;
    [SerializeField] float radius = 3;

    bool isTalking = false;

    Animator anim;

    private void Start()
    {
        //初期位置を固定するためにプレイヤー位置にモデルを移動
        gameObject.transform.position = target.transform.position;

        anim = GetComponent<Animator>();

        agent = GetComponent<NavMeshAgent>();

        agent.isStopped = false;

        //目標地点に近づいても速度を落とさなくなる
        agent.autoBraking = false;

        StartCoroutine(Move());
    }

    private void RandomSit()
    {
        //TODO
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
        isTalking = true;

        //NavMeshを停止
        agent.isStopped = true;

        //プレイヤーの方向を向く
        transform.LookAt(new Vector3(central.position.x, transform.position.y, central.position.z));

        //15~20秒後にランダム移動を再開
        yield return new WaitForSeconds(Random.Range(15, 20));

        isTalking = false;

        agent.isStopped = false;

        //吹き出しをクリア
        text.SetText("");
    }

    private IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3, 5));
            if (!isTalking)
            {
                GotoNextPoint();
            }
        }
    }

    private void Update()
    {
        if (anim)
        {
            //NavMeshAgentのスピードの2乗でアニメーションを切り替える
            anim.SetFloat("Forward", agent.velocity.sqrMagnitude);
        }
    }
}
