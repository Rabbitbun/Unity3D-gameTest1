using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootItem : MonoBehaviour
{
    [field: SerializeField] public ItemData InventoryItem { get; set; }
    [field: SerializeField] public int Quantity { get; set; } = 1;

    [SerializeField] private GameObject lootPrefab;

    [SerializeField] private AudioSource audioSource;

    [SerializeField] private float duration;

    void Start()
    {
        
    }

    public void DestoryItem()
    {
        GetComponent<Collider>().enabled = false;
        StartCoroutine(AnimateItemPickUp());
    }

    private IEnumerator AnimateItemPickUp()
    {
        audioSource.Play();

        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
