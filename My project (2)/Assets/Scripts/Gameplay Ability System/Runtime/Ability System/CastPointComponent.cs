using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPointComponent : MonoBehaviour
{
    public bool IsHumanoid = true;

    [SerializeField]
    public GameObject SwordColliderPoint;

    public Transform _castPoint;

    public Transform facePoint;


    public GameObject GetSwordColliderPoint()
    {
        return SwordColliderPoint;
    }
}
