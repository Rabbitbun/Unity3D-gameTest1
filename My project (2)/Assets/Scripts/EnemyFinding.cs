using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyFinding : MonoBehaviour
{
    public Collider Trigger;

    public List<GameObject> EnemiesList;

    public Transform PlayerTransform;

    private void Awake()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //print(other.gameObject.name);
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            if (EnemiesList.Contains(enemy.gameObject) == false)
                EnemiesList.Add(enemy.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            EnemiesList.Remove(enemy.gameObject);
        }
    }

    public GameObject FindClosestEnemy()
    {
        GameObject closestEnemy;
        if (EnemiesList.Count == 0)
        {
            return null;
        }

        if (EnemiesList.Count == 1)
        {
            closestEnemy = EnemiesList[0];
            return closestEnemy;
        }

        closestEnemy = EnemiesList
                        .OrderBy(enemy => (PlayerTransform.position - enemy.transform.position).sqrMagnitude)
                        .First();
        return closestEnemy;
    }

}
