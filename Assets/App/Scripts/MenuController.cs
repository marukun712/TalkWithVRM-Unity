using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI text;

    public void SpawnCube()
    {
        text.SetText("Click!!!");
    }
}
