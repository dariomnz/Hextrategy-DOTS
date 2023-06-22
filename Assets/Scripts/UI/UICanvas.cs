using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;

public class UICanvas : MonoBehaviour
{

    public static UICanvas Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public Button button;

    void Start()
    {
        // var action = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<CombineMeshSystem>().OnCombineButtonClicked;

        // button.onClick.AddListener(() => { action.Invoke(); });
    }
}
