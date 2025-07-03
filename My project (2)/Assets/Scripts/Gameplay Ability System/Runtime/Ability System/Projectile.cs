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

    public bool AutoReDirection = false;

    public float TargetYOffset = 0.0f;

    private bool collided;

    private Rigidbody rb;
    private float timer = 0;
    public float turnRate = 100f;

    public Action<AbilitySystemCharacter> OnHit;
    [ReadOnly] public AbilitySystemCharacter source;

    private Vector3 targetPosition;
    [SerializeField, ReadOnly] private AbilitySystemCharacter target;

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

        rb = GetComponent<Rigidbody>();

        // 自動追蹤功能
        if (AutoReDirection)
        {
            // 尋找最近的目標
            target = FindClosestTarget();
            if (target != null)
            {
                targetPosition = target.transform.position;
            }
        }
    }

    private void FixedUpdate()
    {
        if (AutoReDirection && target != null)
        {
            timer += Time.deltaTime;

            if (target == null) return;
            targetPosition = target.transform.position;
            targetPosition.y += TargetYOffset;
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(targetPosition - transform.position), Time.deltaTime * turnRate * timer);
            //rb.velocity = transform.forward * ProjectileColliderSpeed;
            float step = timer;

            // 旋轉投射物朝向目標
            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, step);

            // 更新速度，保持前進方向
            rb.velocity = transform.forward * ProjectileColliderSpeed;
        }
    }

    private AbilitySystemCharacter FindClosestTarget()
    {
        // 尋找所有可能的目標
        Collider[] colliders = Physics.OverlapSphere(transform.position, 100f, ProjectileCollisionLayers);

        // 找出最近的目標
        AbilitySystemCharacter closestTarget = null;
        float shortestDistance = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            if (collider.GetComponent<AbilitySystemCharacter>() != null && collider.GetComponent<AbilitySystemCharacter>() != source)
            {
                float distance = Vector3.Distance(transform.position, collider.transform.position);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closestTarget = collider.GetComponent<AbilitySystemCharacter>();
                }
            }
        }

        return closestTarget;
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("碰撞到: "+other.gameObject.name);
        if (collided || other.gameObject.GetComponent<AbilitySystemCharacter>() == source)
        {
            // already collided, don't do anything
            return;
        }
        // stop the projectile
        collided = true;
        Stop();
        rb.velocity = Vector3.zero; // 停止投射物

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
