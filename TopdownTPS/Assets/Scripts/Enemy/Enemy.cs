using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
public class Enemy : LivingEntity
{
    public enum STATE { Idle, Chasing, Attacking }
    STATE currentState;
    public static event System.Action OnDeathStatic;

    [Header("Navigation")]
    public float refreshRate = 0.25f;
    Vector3 lastPos;
    float moveSpeed;
    float angularSpeed;
    public Animator animEnemy;
    NavMeshAgent pathfinder;
    Transform target;
    LivingEntity targetEntity;
    public static bool hasTarget;

    [Header("Attack")]
    public float damage = 1f;
    public float attackDistanceThreshold = 1.5f;
    public float attackSpeed;
    public float timeBetweenAttacks;
    private float nextTimeToAttack;
    private float enemyColRadius;
    private float targetColRadius;
    Color originalColor;
    Material skinMaterial;

    [Header("Particle system")]
    public ParticleSystem deathEffect;

    [Header("Ragdoll Physics")]
    public Collider mainCollider;
    public Collider[] allColliders;
    public Rigidbody[] allRigidbodies;

    [Header("Sound FX")]
    public AudioSource Chasing;
    private void Awake()
    {
        pathfinder = GetComponent<NavMeshAgent>();
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            hasTarget = true;

            enemyColRadius = GetComponent<CapsuleCollider>().radius;
            targetColRadius = target.GetComponent<CapsuleCollider>().radius;
        }

        mainCollider = GetComponent<Collider>();
        allColliders = GetComponentsInChildren<Collider>(true);
        allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        GetComponentInChildren<Renderer>().material.EnableKeyword("_EmissionColor");
        if (hasTarget)
        {
            currentState = STATE.Chasing;
            targetEntity.OnDeath += OnTargetDeath;

            StartCoroutine(UpdatePath());
            StartCoroutine(CalculateSpeed());
        }
        Chasing.PlayOneShot(Chasing.clip, AudioManager.Instance.sfxVolumePercent * AudioManager.Instance.masterVolumePercent);
        //Ragdoll(false);
        animEnemy.SetBool("dead", false);

    }

    public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        if (damage >= currentHealth)
        {
            pathfinder.SetDestination(transform.position);
            if (OnDeathStatic != null)
            {
                OnDeathStatic();
            }
            animEnemy.SetBool("dead", true);
            Destroy(Instantiate(deathEffect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward, hitDirection)) as GameObject, deathEffect.main.startLifetime.constant);
            Destroy(gameObject, 3f);
        }
        base.TakeDamage(damage, hitPoint, hitDirection);
    }

    // Update is called once per frame
    void Update()
    {
        angularSpeed = Mathf.Clamp(angularSpeed, -1, 1);
        if (hasTarget)
        {
            if (nextTimeToAttack <= 0)
            {
                float squaredDistanceToTarget = (target.position - transform.position).sqrMagnitude;
                if (squaredDistanceToTarget < Mathf.Pow(attackDistanceThreshold + enemyColRadius + targetColRadius, 2))
                {
                    nextTimeToAttack = timeBetweenAttacks;
                    StartCoroutine(Attack());
                }

            }
            else
            {
                nextTimeToAttack -= Time.deltaTime;
            }

            animEnemy.SetFloat("speed", moveSpeed);
            if (transform.rotation.eulerAngles.y > 0 && transform.rotation.eulerAngles.y <= 170)
            {
                angularSpeed += moveSpeed * Time.deltaTime;
                animEnemy.SetFloat("angularspeed", angularSpeed);
            }
            else if (transform.rotation.eulerAngles.y >= 190 && transform.rotation.eulerAngles.y <= 350)
            {
                angularSpeed -= moveSpeed * Time.deltaTime;
                animEnemy.SetFloat("angularspeed", angularSpeed);
            }
            else
            {
                angularSpeed = 0;
            }
        }   
    }

    IEnumerator Attack()
    {
        currentState = STATE.Attacking;
        pathfinder.enabled = false;
        Vector3 originalPos = transform.position;
        Vector3 dirToTarget = (target.position - transform.position).normalized;
        Vector3 attackPos = target.position - dirToTarget * (enemyColRadius);

        float Percent = 0;

        animEnemy.SetBool("attack", true);
        bool hasAppliedDamage = false;

        while (Percent <= 1)
        {
            if (Percent <= .5f && !hasAppliedDamage)
            {
                //transform.LookAt(target);
                hasAppliedDamage = true;
                targetEntity.TakeDmg(damage);
            }
            Percent += Time.deltaTime * attackSpeed;
            //float interpolation = (-Mathf.Pow(Percent,2) * Percent + Percent) * 4f;
            //transform.position = Vector3.Lerp(originalPos, attackPos, interpolation);
            yield return null;
        }
        animEnemy.SetBool("attack", false);
        currentState = STATE.Chasing;
        pathfinder.enabled = true;
    }
    IEnumerator UpdatePath()
    {
        while (hasTarget)
        {
            if (currentState == STATE.Chasing)
            {
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                Vector3 targetPos = target.position - dirToTarget * (enemyColRadius + targetColRadius + attackDistanceThreshold / 2);
                if (!dead && hasTarget)
                {
                    pathfinder.SetDestination(targetPos);
                }
            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    IEnumerator CalculateSpeed()
    {
        while (Application.isPlaying)
        {
            lastPos = transform.position;
            yield return new WaitForFixedUpdate();
            moveSpeed = Mathf.RoundToInt(Vector3.Distance(transform.position, lastPos) / Time.fixedDeltaTime);
        }
    }

    void OnTargetDeath()
    {
        hasTarget = false;
        currentState = STATE.Idle;
    }

    public void SetStats(float moveSpeed, int hitsToKillPlayer, float newHealth, Color skinColor)
    {
        pathfinder.speed = moveSpeed;

        if (hasTarget)
        {
            damage = Mathf.Ceil(targetEntity.health / hitsToKillPlayer);
        }

        health = newHealth;

        //ParticleSystem.MainModule deathEffectMain = deathEffect.main;
        //deathEffectMain.startColor = new Color(skinColor.r, skinColor.g, skinColor.b, 1);

        //skinMaterial = GetComponentInChildren<Renderer>().material;
        //skinMaterial.SetColor("_EmissionColor", skinColor);
        //skinMaterial.color = skinColor;
        //originalColor = skinMaterial.color;
    }

    public void Ragdoll(bool isRagdoll)
    {
        StartCoroutine(DoRagdoll(isRagdoll));
    }

    private IEnumerator DoRagdoll(bool isRagdoll)
    {
        foreach (Collider col in allColliders.Skip(1))
        {

            col.enabled = isRagdoll;
        }
        foreach (Rigidbody r in allRigidbodies.Skip(1))
        {
            if (isRagdoll)
            {
                r.velocity = Vector3.zero;
            }
        }
        yield return new WaitForSeconds(0.01f);
        //mainCollider.enabled = !isRagdoll;
        //rb.useGravity = !isRagdoll;
        animEnemy.enabled = !isRagdoll;
    }
}

