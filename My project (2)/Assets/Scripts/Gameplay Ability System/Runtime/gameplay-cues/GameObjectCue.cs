using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameObject Cue", menuName = "Gameplay/GameObject Cue")]
public class GameObjectCue : GameplayCueDurational
{
    public string ObjectName;
    public GameObject prefab;

    public enum UseType
    {
        Spawn,
        Trigger
    }
    public UseType useType;

    public bool IsAttachToTarget;



    public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
    {
        return new GameObjectCueSpec(this, parameters);
    }
}

public class GameObjectCueSpec : GameplayCueDurationalSpec<GameObjectCue>
{
    public GameObject Instance;
    private ParticleSystem[] particleSystems;
    private CastPointComponent castPointComponent;

    public GameObjectCueSpec(GameObjectCue cue, GameplayCueParameters parameters) : base(cue,
        parameters)
    {
    }

    public override void OnAdd()
    {
        if (cue.useType == GameObjectCue.UseType.Spawn)
        {
            if (cue.IsAttachToTarget)
            {
                Instance = Object.Instantiate(cue.prefab, Owner.transform);
            }
            else
            {
                Instance = Object.Instantiate(cue.prefab, Owner.transform.position, Quaternion.identity);
            }
        }
        else if (cue.useType == GameObjectCue.UseType.Trigger)
        {
            Transform[] children = Owner.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in children)
            {
                if (child.name == cue.ObjectName)
                {
                    Instance = child.gameObject;
                    Instance.SetActive(true);
                }
            }
        }

    }

    public override void OnRemove()
    {
        if (Instance != null)
        {
            if (cue.useType == GameObjectCue.UseType.Spawn)
            {
                Object.Destroy(Instance);
            }
            else if (cue.useType == GameObjectCue.UseType.Trigger)
            {
                Instance.SetActive(false);
            }
        }
    }

    public override void OnGameplayEffectActivate()
    {
        
    }

    public override void OnGameplayEffectDeactivate()
    {
        
    }

    public override void OnTick()
    {
    }

    public void SetVisible(bool visible)
    {
        
    }
}