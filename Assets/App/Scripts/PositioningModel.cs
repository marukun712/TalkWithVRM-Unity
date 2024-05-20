using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PositioningModel : MonoBehaviour
{
    [SerializeField] private Transform anchor;
    [SerializeField] private Transform target;
    [SerializeField] private LineRenderer lineRenderer;

    private float maxDistance = 100f;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        //Rayを飛ばしてhitしたpointにモデルを移動
        float distance = 100f;
        float duration = 3f;
        lineRenderer.SetWidth(0f, 0f);

        Ray ray = new Ray(anchor.position, anchor.forward);

        //レーザーの描画
        lineRenderer.SetPosition(0, ray.origin);

        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red, duration, false);

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            lineRenderer.SetPosition(1, hit.point);

            // 中指トリガーでモデルの位置をRayが当たった場所に移動する
            if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
            {
                lineRenderer.SetWidth(0.01f, 0.01f);

                target.transform.position = hit.point;
            }
        }
        else
        {
            lineRenderer.SetPosition(1, ray.origin + (ray.direction * maxDistance));
        }
    }
}
