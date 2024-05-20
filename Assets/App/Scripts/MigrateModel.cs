using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRM;

//TODO 複数モデル対応
public class MigrateModel : MonoBehaviour
{
    [SerializeField] GameObject currentModel;
    [SerializeField] GameObject prefab;

    private void Start()
    {
        GameObject obj = Instantiate(prefab);
        MigrateComponents(currentModel, obj);
        //元のモデルを破棄
        Destroy(currentModel);
    }

    //コンポーネントの移行
    private void MigrateComponents(GameObject source, GameObject newModel)
    {
        // すべてのコンポーネントを取得
        Component[] components = source.GetComponents<Component>();

        foreach (Component component in components)
        {
            // Transformコンポーネントはスキップ
            if (component is Transform) continue;
            if (component is Animator) continue;
            if (component is VRM.VRMMeta) continue;
            if (component is VRM.VRMHumanoidDescription) continue;
            if (component is VRM.VRMBlendShapeProxy) continue;
            if (component is VRM.VRMFirstPerson) continue;
            if (component is VRM.VRMLookAtHead) continue;
            if (component is VRM.VRMLookAtBoneApplyer) continue;

            GameObject fukidashi = GameObject.FindWithTag("Fukidashi");
            fukidashi.transform.SetParent(newModel.transform);

            System.Type type = component.GetType();
            Component copy = newModel.AddComponent(type);

            foreach (var field in type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance))
            {
                field.SetValue(copy, field.GetValue(component));
            }
        }
    }
}
