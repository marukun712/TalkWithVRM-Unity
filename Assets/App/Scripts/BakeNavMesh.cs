using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.AI.Navigation.Samples //エラー回避
{
    public class BakeNavMesh : MonoBehaviour
    {
        [SerializeField]
        private OVRSceneManager _sceneManager;

        private void Awake()
        {
            //シーンロード後のコールバック関数を指定
            _sceneManager.SceneModelLoadedSuccessfully += Initialize;
        }

        // Room ModelからNavMeshを生成
        private void Initialize()
        {
            OVRSceneRoom room = FindAnyObjectByType<OVRSceneRoom>();
            NavMeshSurface navMesh = room.gameObject.AddComponent<NavMeshSurface>(); //NavMeshSurfaceを追加

            //ベイク
            navMesh.collectObjects = CollectObjects.Children; //OVRSceneManagerから生成されるメッシュを指定するために子要素をベイクの対象にする
            navMesh.BuildNavMesh();
        }
    }
}
