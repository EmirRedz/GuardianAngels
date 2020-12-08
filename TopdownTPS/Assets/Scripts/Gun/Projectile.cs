using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public float speed = 10;
    public float damage = 1;
    public float lifeTime=3;
    public float skinWidth = .1f;

    public ParticleSystem bloodEffect;
    public float impactForce = 30;
    TrailRenderer trailRenderer;
    public Color trailColor;
    private void Start()
    {
        Destroy(gameObject, lifeTime);

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
        if(initialCollisions.Length > 0)
        {
            OnHitObject(initialCollisions[0], transform.position);
        }

        trailRenderer = GetComponent<TrailRenderer>();
        trailRenderer.startColor = trailColor;
    }
    void Update()
    {
        float moveDistance = speed * Time.deltaTime;
        CheckCollision(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }


    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }

    void CheckCollision(float moveDistance)
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, moveDistance + skinWidth, collisionMask, QueryTriggerInteraction.Collide))
        {
            OnHitObject(hit.collider,hit.point);
            Destroy(Instantiate(bloodEffect.gameObject, hit.point, Quaternion.LookRotation(hit.normal)) as GameObject, bloodEffect.main.startLifetime.constant);
        }
    }
    void OnHitObject(Collider collider, Vector3 hitPoint)
    {
        IDamagable damagableObject = collider.GetComponent<IDamagable>();
        if (damagableObject != null)
        {
            damagableObject.TakeDamage(damage,hitPoint,transform.forward);
        }
        Destroy(gameObject);
    }

    
}
