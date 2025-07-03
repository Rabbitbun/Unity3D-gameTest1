using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New VFX Cue", menuName = "Gameplay/VFX Cue")]
public class VFXCue : GameplayCueDurational
{
    [Tooltip("可接受有層級的物件")]
    public GameObject VfxPrefab;

    public bool IsAttachToTarget = true;

    public bool OverrideSettings = false;

    public bool UsePrefabTransform = false;

    public Vector3 Offset;

    public Vector3 Rotation;

    public Vector3 Scale = Vector3.one;

    public enum OffsetType
    {
        Head,
        CastPoint,
        None
    }

    public OffsetType offsetType;

    public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
    {
        return new VFXCueSpec(this, parameters);
    }
}
public class VFXCueSpec : GameplayCueDurationalSpec<VFXCue>
{
    public GameObject vfxInstance;
    private ParticleSystem[] particleSystems;
    private CastPointComponent castPointComponent;

    public VFXCueSpec(VFXCue cue, GameplayCueParameters parameters) : base(cue,
        parameters)
    {
    }

    public override void OnAdd()
    {
        castPointComponent = Owner.GetComponent<CastPointComponent>();
        if (cue.IsAttachToTarget)
        {
            vfxInstance = Object.Instantiate(cue.VfxPrefab, Owner.transform);
            if (castPointComponent != null && cue.offsetType == VFXCue.OffsetType.Head)
            {
                vfxInstance.transform.position = castPointComponent.facePoint.position;
            }
            else if (castPointComponent != null && cue.offsetType == VFXCue.OffsetType.CastPoint)
            {
                vfxInstance.transform.position = castPointComponent._castPoint.position;
            }
            else if (castPointComponent != null && cue.offsetType == VFXCue.OffsetType.None)
            {
                vfxInstance.transform.localPosition = cue.Offset;
            }
        }
        else
        {
            vfxInstance = Object.Instantiate(cue.VfxPrefab, Owner.transform.position, Quaternion.identity);
            vfxInstance.transform.localPosition = cue.Offset;
            vfxInstance.transform.localEulerAngles = cue.Rotation;
            vfxInstance.transform.localScale = cue.Scale;
        }

        if (cue.OverrideSettings)
        {
            vfxInstance.transform.localEulerAngles = cue.Rotation;
            vfxInstance.transform.localScale = cue.Scale;
        }
        if (cue.UsePrefabTransform)
        {
            vfxInstance.transform.localPosition = cue.VfxPrefab.transform.position;
            vfxInstance.transform.localRotation = cue.VfxPrefab.transform.rotation;
        }


        particleSystems = vfxInstance.GetComponentsInChildren<ParticleSystem>();
        if (particleSystems != null)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }
        }

    }

    public override void OnRemove()
    {
        Object.Destroy(vfxInstance);
    }

    public override void OnGameplayEffectActivate()
    {
        vfxInstance.SetActive(true);
        if (particleSystems != null)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Play();
            }
        }
    }

    public override void OnGameplayEffectDeactivate()
    {
        vfxInstance.SetActive(false);
        if (particleSystems != null)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].Stop();
            }
        }
    }

    public override void OnTick()
    {
    }

    public void SetVisible(bool visible)
    {
        vfxInstance?.SetActive(visible);
    }
}
