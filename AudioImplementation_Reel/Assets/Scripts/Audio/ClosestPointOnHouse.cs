using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosestPointOnHouse : MonoBehaviour
{
    public GameObject go;
    public Collider collider;
    Renderer rend;
    private void Start()
    {
        rend = GetComponent<Renderer>();
        collider = GetComponent<Collider>();
        Debug.Log(collider);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            go.transform.position = rend.bounds.ClosestPoint(AudioManager.Instance.playerGO.transform.position);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            go.transform.position = collider.ClosestPointOnBounds(AudioManager.Instance.playerGO.transform.position);
        }
    }
}
