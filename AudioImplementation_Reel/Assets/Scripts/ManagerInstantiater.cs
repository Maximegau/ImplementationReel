using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerInstantiater : MonoBehaviour
{
    public static ManagerInstantiater Instance = null;

    private void Awake()
    {
        if (ManagerInstantiater.Instance == null) Instance = this;
        else Destroy(gameObject);
        Instantiate(Resources.Load<GameObject>("Prefab/Managers"), gameObject.transform);
    }
}
