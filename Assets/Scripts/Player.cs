using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//プレーヤーとプレイヤー·コントローラーが付いているように実装するためにRequireComponent属性を追加する。
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
       

        //移動入力先
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Vector3 moveVelotcity = moveInput.normalized * moveSpeed;
        controller.Move(moveVelotcity);

        
        //カメラがプレーヤーを見る時点を指定するために
        //見渡す方向を入力するところ
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
        //無機操作入力
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
