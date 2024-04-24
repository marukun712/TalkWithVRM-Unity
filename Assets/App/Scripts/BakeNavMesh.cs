using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Unity.AI.Navigation.Samples
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
            NavMeshSurface navMesh = room.gameObject.AddComponent<NavMeshSurface>();

            // NavMeshをGlobalMeshにベイク
            navMesh.collectObjects = CollectObjects.Children;
            navMesh.BuildNavMesh();
        }
    }
}