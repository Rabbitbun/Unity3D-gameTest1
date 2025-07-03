using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Boss_TriggerEnter : MonoBehaviour
{
    public GameObject DoorObject;

    public GameObject BossHPbar;

    public Enemy Boss;

    [SerializeField] bool isTriggered = false;

    private async void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && isTriggered == false)
        {
            isTriggered = true;
            await Task.Delay(TimeSpan.FromSeconds(0.5f));
            DoorObject.SetActive(true);
            BossHPbar.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Boss.IsDead && isTriggered)
        {
            DoorObject.SetActive(false);
            BossHPbar.SetActive(false);
            this.gameObject.SetActive(false);
        }
    }
}
