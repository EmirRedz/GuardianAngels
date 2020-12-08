using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public float forceMin, forceMax;
    float force;
    Rigidbody rb;

    public float lifeTime = 4;
    public float fadeTime = 2;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        force = Random.Range(forceMin, forceMax);
        rb.AddForce(transform.right * force);
        rb.AddTorque(Random.insideUnitSphere * force);

        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(lifeTime);

        float fadePercent = 0;
        float fadeSpeed = 1 / fadeTime;
        Material mat = GetComponent<Renderer>().material;
        Color originalColor = mat.color;

        while(fadePercent < 1)
        {
            fadePercent += Time.deltaTime * fadeSpeed;
            mat.color = Color.Lerp(originalColor, Color.clear, fadePercent);
            yield return null;
        }

        Destroy(gameObject);
    }
}
