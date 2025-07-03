using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlaySound Cue", menuName = "Gameplay/PlaySound Cue")]
public class PlaySoundCue : GameplayCueDurational
{
    public AudioClip soundEffect;

    public bool isAttachToOwner = true;

    public override GameplayCueDurationalSpec CreateSpec(GameplayCueParameters parameters)
    {
        return new PlaySoundCueSpec(this, parameters);
    }
}

public class PlaySoundCueSpec : GameplayCueDurationalSpec<PlaySoundCue>
{
    private AudioSource _audioSource;

    public PlaySoundCueSpec(PlaySoundCue cue, GameplayCueParameters parameters) 
        : base(cue, parameters)
    {
        if (cue.isAttachToOwner)
        {
            _audioSource = Owner.gameObject.GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = Owner.gameObject.AddComponent<AudioSource>();
            }
        }
        else
        {
            var soundRoot = new GameObject("SoundRoot");
            soundRoot.transform.position = Owner.transform.position;
            _audioSource = soundRoot.AddComponent<AudioSource>();
        }
    }

    public override void OnAdd()
    {
        _audioSource.clip = cue.soundEffect;
        _audioSource.Play();
    }

    public override void OnRemove()
    {
        if (!cue.isAttachToOwner)
        {
            Object.Destroy(_audioSource.gameObject);
        }
        else
        {
            _audioSource.Stop();
            _audioSource.clip = null;
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
}
