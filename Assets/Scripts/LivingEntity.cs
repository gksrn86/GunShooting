using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LivingEntity : MonoBehaviour, IDamageable {
    
    public float startingHealth;
    public float health { get; protected set; }
    protected bool dead;

    public event System.Action OnDeath;

    protected virtual void Start() {
        health = startingHealth;
       
           
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint , Vector3 hitDirection) {
        //do some stuff here with hit var
        //히트 변수와 함께 여기서 몇몇의 처리를 한다
	//ヒット変数とともにここでいくつかの処理を行う。
        TakeDamage(damage);
    }
    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }

    }
   
	[ContextMenu("Self Destruct")]
    public virtual void Die() {
        dead = true;
        if(OnDeath != null) {
            OnDeath();
        }
        GameObject.Destroy(gameObject);
       
    }
}
