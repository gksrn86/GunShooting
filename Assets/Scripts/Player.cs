using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*player와 playercontroller가 붙어 있을 수 있도록 구현 하기위해서 RequireComponent속성을 추가한다.*/
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GunController))]
public class Player : LivingEntity {


    public float moveSpeed = 5;

    public Crosshairs crosshairs;

    Camera viewCamera;
    PlayerController controller;
    GunController gunController;

    protected override void Start() {
        base.Start();
        
    }

    void Awake()
    {
        controller = GetComponent<PlayerController>();
        gunController = GetComponent<GunController>();
        viewCamera = Camera.main;
        FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
    }

    void OnNewWave(int waveNumber)
    {
        health = startingHealth;
        gunController.EquipGun (waveNumber -1);
    }

    void Update() {
       

        //이동 입력 받는 곳 Movement input
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelotcity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelotcity);

        /*카메라가 플레이어를 바라보는 시점을 지정하기 위해서*/
        //바라보는 방향을 입력 받는 곳  Look input
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.up * gunController.GunHeight);
        float rayDistance;

        if (groundPlane.Raycast(ray, out rayDistance)) {
                Vector3 point = ray.GetPoint(rayDistance);
                //Debug.DrawLine(ray.origin, point, Color.red);
                controller.LookAt(point);
                crosshairs.transform.position = point;
                crosshairs.DetectTargets(ray);
            if ((new Vector2(point.x, point.z) - new Vector2(transform.position.x, transform.position.z)).sqrMagnitude > 1)
            {
                gunController.Aim(point);
            }   
        }
        //무기 조작 입력 Weapon input
        if (Input.GetMouseButton(0)){
            gunController.OnTriggerHold();
        }
        if(Input.GetMouseButtonUp(0))
        {
            gunController.OnTriggerRelease();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            gunController.Reload();
        }
        if(transform.position.y < -10)
        {
            TakeDamage(health);
        }
    }
    public override void Die()
    {
        AudioMananger.instance.PlaySound("Player Death", transform.position);
        base.Die();
    }
}
