using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    void Start()
    {
        Destroy(gameObject, 4f); // Destrói a bala a cada 10 segundos
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.transform.parent != null && collision.transform.parent.gameObject.layer == 9)
            {
                Enemy pos = collision.transform.parent.GetComponent<Enemy>();
                GameObject explosion = Instantiate(explosionPrefab, pos.bodyParts[0].transform.position, Quaternion.identity);
                pos.Die();
                Destroy(gameObject);
            }
        }
    }
}
