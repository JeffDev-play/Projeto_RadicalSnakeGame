using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float timerExplosion;
    
    void Start()
    {
        Destroy(gameObject, timerExplosion);
    }

   
}
