using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingEntity
{
    public bool FollowCameraControlls;

    public Crosshair crosshair;

    public float verticalSpeed = 3;
    public float horizontalSpeed = 1;
    Camera mainCamera;
    GunController gunController;
    PlayerMovment controller;
    Vector3 movmentInput;

    private void Awake()
    {
        controller = GetComponent<PlayerMovment>();
        gunController = GetComponent<GunController>();
        mainCamera = Camera.main;
        if (FindObjectOfType<Spawner>() != null)
        {
            FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        }
    }

    // Start is called before the first frame update
    protected override void  Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (dead)
        {
            controller.Ragdoll(true);
            AudioListener.pause = true;
            return;
        }
        if (!LevelManager.isPaused)
        {
            Movments();
            Looking();
            Shooting();
        }
    }

    void Movments()
    {
        Vector3 moveX = transform.right * Input.GetAxisRaw("Horizontal") * verticalSpeed;
        Vector3 moveZ = transform.forward * Input.GetAxisRaw("Vertical") * verticalSpeed;
        if (FollowCameraControlls)
        {
            Vector3 moveVelocity = (moveX + moveZ);
            controller.Move(moveVelocity);
        }
        else
        {
            //if (!Gun.isReloading)
            {
                if (transform.rotation.eulerAngles.y >= 80f && transform.rotation.eulerAngles.y <= 105f ||
                    transform.rotation.eulerAngles.y <= 280f && transform.rotation.eulerAngles.y >= 255f)
                {
                    movmentInput = new Vector3(Input.GetAxisRaw("Horizontal") * verticalSpeed, 0, Input.GetAxisRaw("Vertical") * horizontalSpeed);
                }
                else
                {
                    movmentInput = new Vector3(Input.GetAxisRaw("Horizontal") * horizontalSpeed, 0, Input.GetAxisRaw("Vertical") * verticalSpeed);
                }

                Vector3 moveVelocity = (movmentInput);
                controller.Move(moveVelocity);
            }
        }
    }
    void Looking()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);
            controller.LookAt(point);
            crosshair.transform.position = point;
            crosshair.DetectTargets(ray);

            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                //gunController.Aim(point);
            }
        }
    }

    void Shooting()
    {
        if (Input.GetMouseButton(0))
        {
            gunController.OnTriggerHold();
        }
        if (Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }
    }

    public override void TakeDamage(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        base.TakeDamage(damage, hitPoint, hitDirection);
    }

    void OnNewWave(int waveNumber)
    {
        currentHealth = health;
    }
}

