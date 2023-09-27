using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public GameObject Target;
    NavMeshAgent navMesh;

    private void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (Vector3.Distance(this.transform.position, Target.transform.position) < 10f)
            navMesh.SetDestination(Target.transform.position);

    }
}
