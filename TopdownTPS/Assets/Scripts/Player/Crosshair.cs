using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public SpriteRenderer dot;
    public float rotationSpeed;
    public LayerMask targetMask;
    Color originalColor;
    public Color hightlightColor;
    // Use this for initialization
    void Start()
    {
        originalColor = dot.color;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }

    public void DetectTargets(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, 100, targetMask))
        {
            if(Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                   hightlightColor = hit.collider.gameObject.GetComponentInChildren<Renderer>().material.color;
                }
            }
            dot.color = hightlightColor;
        }
        else
        {
            dot.color = originalColor;
        }
    }
}
