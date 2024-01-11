using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public enum AbilityCastType
{
    Targeted, //瞄準施放
    Instant, //直接施放
}

[CreateAssetMenu(fileName = "New Spell", menuName = "Spells")]
[SerializeField]
public class AbilityObject : ScriptableObject
{
    public float DamageAmount;
    public float ManaCost;
    public float LifeTime;
    public float Speed; // collider speed
    public float EffectRadius;
    public float ExplosionForce;
    public float Cooldown;

    public AbilityCastType CastType;

    public Texture2D texture;
    public Sprite spriteTexture;
    public Image icon; //UI icon
    [HideInInspector] public Image darkMask; //icon Background

    public string Name;
    public int ID;
    //[SerializeField] public Text coolDownText;
    [HideInInspector] public TextMeshProUGUI coolDowntextMeshPro;



}
