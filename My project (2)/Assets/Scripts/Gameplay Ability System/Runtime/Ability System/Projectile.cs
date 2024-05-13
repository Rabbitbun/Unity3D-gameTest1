using AbilitySystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 此腳本參考 ProjectileScript.cs
/// </summary>
public class Projectile : ProjectileBase
{
    public float speed;
    public float LifeTime;

    //[Tooltip("碰撞時的音效")]
    //public AudioSource ProjectileCollisionSound;

    [Tooltip("碰撞體")]
    public GameObject ProjectileColliderObject;

    //[Tooltip("碰撞時要應用的粒子特效")]
    //public ParticleSystem ProjectileExplosionParticleSystem;

    [Tooltip("碰撞時的爆炸力半徑")]
    public float ProjectileExplosionRadius = 50.0f;

    [Tooltip("碰撞時的爆炸力的強度")]
    public float ProjectileExplosionForce = 50.0f;

    [Tooltip("碰撞體被發送之前的可選延遲，以防效果具有預觸發動畫。")]
    public float ProjectileColliderDelay = 0.0f;

    //[Tooltip("碰撞體的速度")]
    //public float ProjectileColliderSpeed = 450.0f;

    [Tooltip("碰撞體前進的方向")]
    public Vector3 ProjectileDirection = Vector3.forward;

    [Tooltip("碰撞體可以與哪些層發生碰撞。")]
    public LayerMask ProjectileCollisionLayers = Physics.AllLayers;

    //[Tooltip("碰撞時要摧毀的粒子系統(例如trail、主體等等)")]
    //public ParticleSystem[] ProjectileDestroyParticleSystemsOnCollision;

    private bool collided;

    public Action<AbilitySystemCharacter> OnHit;
    [ReadOnly] public AbilitySystemCharacter source;



    private IEnumerator SendCollisionAfterDelay()
    {
        yield return new WaitForSeconds(ProjectileColliderDelay);

        Vector3 dir = ProjectileDirection * ProjectileColliderSpeed;
        dir = ProjectileColliderObject.transform.rotation * dir;
        ProjectileColliderObject.GetComponent<Rigidbody>().velocity = dir;
    }

    protected override void Start()
    {
        base.Start();

        StartCoroutine(SendCollisionAfterDelay());
    }

    private void OnCollisionEnter(Collision other)
    {
        if (collided || other.gameObject.GetComponent<AbilitySystemCharacter>() == source)
        {
            // already collided, don't do anything
            return;
        }
        // stop the projectile
        collided = true;
        Stop();

        if (other.gameObject.GetComponent<AbilitySystemCharacter>() != null && other.gameObject.GetComponent<AbilitySystemCharacter>() != source)
        {

            if (other.contacts.Length != 0)
            {
                CreateExplosion(other.contacts[0].point, ProjectileExplosionRadius, ProjectileExplosionForce);
            }

            OnHit?.Invoke(other.gameObject.GetComponent<AbilitySystemCharacter>());
            Destroy(gameObject);
        }
    }
}
