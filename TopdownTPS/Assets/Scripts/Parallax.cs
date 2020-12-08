using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    Material material;
    public float xOffset, yOffset;
    Vector2 offset;
    // Start is called before the first frame update
    void Awake()
    {
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset = new Vector2(xOffset, yOffset);
        material.mainTextureOffset += offset * Time.deltaTime;
        if (material.mainTextureOffset.x >= 1 || material.mainTextureOffset.y >= 1)
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }
}
