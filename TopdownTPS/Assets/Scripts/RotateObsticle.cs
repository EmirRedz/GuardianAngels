using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObsticle : MonoBehaviour
{
    public float obsticleRotSpeed = 50f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * obsticleRotSpeed * Time.deltaTime);
    }
}
