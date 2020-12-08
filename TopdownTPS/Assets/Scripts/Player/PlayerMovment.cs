using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerMovment : MonoBehaviour
{
    Vector3 _velocity;
    Rigidbody rb;

    [Header("Ragdoll")]
    public Collider mainCollider;
    public Collider[] allColliders;
    public Rigidbody[] allRigidbodies;
    [Header ("Animations")]
    public Animator playerAnim;
    public GameObject muzzleFlash;
    public float threshold;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<Collider>();
        allColliders = GetComponentsInChildren<Collider>(true);
        allRigidbodies = GetComponentsInChildren<Rigidbody>(true);
    }
    // Start is called before the first frame update
    void Start()
    {
        Ragdoll(false);

    }

    // Update is called once per frame
    void Update()
    {
        #region Animation controlls
        Vector3 inputVector = (Vector3.forward * Input.GetAxis("Vertical")) + (Vector3.right * Input.GetAxis("Horizontal"));

        Vector3 animationVector = transform.InverseTransformDirection(inputVector);

        float VelocityX = animationVector.x;
        float VelocityZ = animationVector.z;

        

        playerAnim.SetFloat("horizontal", VelocityX);
        playerAnim.SetFloat("vertical", VelocityZ);

        #endregion
    }

    private void LateUpdate()
    {
        //float x = Input.GetAxis("Horizontal");
        //float z = Input.GetAxis("Vertical");
        //if (x >= threshold || x <= -threshold || z <= -threshold)
        //{
        //    muzzleFlash.transform.localPosition = newMuzzlePosition;
        //}
        //else
        //{
        //    muzzleFlash.transform.localPosition = oldMuzzlePos;
        //}
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + _velocity * Time.fixedDeltaTime);
    }

    public void LookAt(Vector3 lookPoint)
    {
        transform.LookAt(lookPoint);
    }

    public void Move(Vector3 velocity)
    {
        _velocity = velocity;
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
        playerAnim.enabled = !isRagdoll;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Offworld"))
        {
            transform.position = Vector3.up * 3;
        }
    }
}
