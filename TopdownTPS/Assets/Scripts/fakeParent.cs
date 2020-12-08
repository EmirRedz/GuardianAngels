using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fakeParent : MonoBehaviour
{
    public Transform parent;
    public Vector3 offset;
    private void Start()
    {
    }
    void LateUpdate()
    {
        //parent = GameObject.Find("AssaultRifle(Clone)").transform;
        transform.SetParent(parent);
    }
}
