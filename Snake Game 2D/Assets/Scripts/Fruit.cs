using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : MonoBehaviour
{
    private Snake snake; // Acessa o script atrelada ao GameObject do player atual

    private void Start()
    {
        snake = FindObjectOfType<Snake>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.layer == 7)
            {
                snake.RestorativeFruit(); // Restaura a vida do player em 1
                Destroy(gameObject);
            }
        }
    }  
}
