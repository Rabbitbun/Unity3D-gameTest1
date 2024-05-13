using UnityEngine;
using System.Collections;

public class ProjectileBase : MonoBehaviour
{
    //[Tooltip("腳本啟動時播放一次的音訊來源(可選)。")]
    //public AudioSource AudioSource;

    [Tooltip("腳本完全啟動需要多長時間。這用於淡入動畫和聲音等。")]
    public float StartTime = 1.0f;

    [Tooltip("腳本完全停止需要多長時間。這用於淡出動畫和聲音等。")]
    public float StopTime = 3.0f;

    [Tooltip("效果持續多久。一旦持續時間結束，腳本將在 StopTime 內生存，然後物件將被銷毀。")]
    public float Duration = 2.0f;

    [Tooltip("一開始在中心產生多少力（爆炸），0 表示無。")]
    public float ForceAmount;

    [Tooltip("一開始力的半徑，0 表示無。")]
    public float ForceRadius;

    [Tooltip("向腳本使用者提示物件是否為投射物")]
    public bool IsProjectile;

    [Tooltip("必須手動啟動且不會在啟動時播放的粒子系統。")]
    public ParticleSystem[] ManualParticleSystems;

    [Tooltip("碰撞體的速度")]
    public float ProjectileColliderSpeed = 450.0f;

    private float startTimeMultiplier;
    private float startTimeIncrement;

    private float stopTimeMultiplier;
    private float stopTimeIncrement;

    private IEnumerator CleanupEverythingCoRoutine()
    {
        // 2 extra seconds just to make sure animation and graphics have finished ending
        yield return new WaitForSeconds(StopTime + 1.0f);
        GameObject.Destroy(gameObject);
    }

    private void StartParticleSystems()
    {
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            if (ManualParticleSystems == null) continue;
            if (ManualParticleSystems.Length == 0) continue;
            if (System.Array.IndexOf(ManualParticleSystems, p) < 0) continue;
            //if (ManualParticleSystems == null || ManualParticleSystems.Length == 0 ||
            //    System.Array.IndexOf(ManualParticleSystems, p) < 0)
            {
                if (p.main.startDelay.constant == 0.0f)
                {
                    // wait until next frame because the transform may change
                    var m = p.main;
                    var d = p.main.startDelay;
                    d.constant = 0.01f;
                    m.startDelay = d;
                    m.startSpeed = ProjectileColliderSpeed;
                }
                p.Play();
            }
        }
    }

    protected virtual void Awake()
    {
        Starting = true;
        //int fireLayer = UnityEngine.LayerMask.NameToLayer("FireLayer");
        //if (fireLayer >= 0 && fireLayer < 32)
        //    UnityEngine.Physics.IgnoreLayerCollision(fireLayer, fireLayer);
    }

    protected virtual void Start()
    {
        //if (AudioSource != null)
        //{
        //    AudioSource.Play();
        //}

        // precalculate so we can multiply instead of divide every frame
        stopTimeMultiplier = 1.0f / StopTime;
        startTimeMultiplier = 1.0f / StartTime;

        // if this effect has an explosion force, apply that now 一開始的爆炸
        //CreateExplosion(gameObject.transform.position, ForceRadius, ForceAmount);

        // start any particle system that is not in the list of manual start particle systems
        StartParticleSystems();

        // If we implement the ICollisionHandler interface, see if any of the children are forwarding
        // collision events. If they are, hook into them.
        //ICollisionHandler handler = (this as ICollisionHandler);
        //if (handler != null)
        //{
        //    CollisionForwardScript collisionForwarder = GetComponentInChildren<CollisionForwardScript>();
        //    if (collisionForwarder != null)
        //    {
        //        collisionForwarder.CollisionHandler = handler;
        //    }
        //}
    }

    protected virtual void Update()
    {
        // reduce the duration
        Duration -= Time.deltaTime;
        if (Stopping)
        {
            // increase the stop time
            stopTimeIncrement += Time.deltaTime;
            if (stopTimeIncrement < StopTime)
            {
                StopPercent = stopTimeIncrement * stopTimeMultiplier;
            }
        }
        else if (Starting)
        {
            // increase the start time
            startTimeIncrement += Time.deltaTime;
            if (startTimeIncrement < StartTime)
            {
                StartPercent = startTimeIncrement * startTimeMultiplier;
            }
            else
            {
                Starting = false;
            }
        }
        else if (Duration <= 0.0f)
        {
            // time to stop, no duration left
            Stop();
        }
    }

    public static void CreateExplosion(Vector3 pos, float radius, float force)
    {
        if (force <= 0.0f || radius <= 0.0f)
        {
            return;
        }

        // find all colliders and add explosive force
        Collider[] objects = UnityEngine.Physics.OverlapSphere(pos, radius);
        foreach (Collider h in objects)
        {
            Rigidbody r = h.GetComponent<Rigidbody>();
            if (r != null)
            {
                r.AddExplosionForce(force, pos, radius);
            }
        }
    }

    public virtual void Stop()
    {
        if (Stopping)
        {
            return;
        }
        Stopping = true;

        // cleanup particle systems
        foreach (ParticleSystem p in gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            p.Stop();
        }

        StartCoroutine(CleanupEverythingCoRoutine());
    }

    public bool Starting
    {
        get;
        private set;
    }

    public float StartPercent
    {
        get;
        private set;
    }

    public bool Stopping
    {
        get;
        private set;
    }

    public float StopPercent
    {
        get;
        private set;
    }
}