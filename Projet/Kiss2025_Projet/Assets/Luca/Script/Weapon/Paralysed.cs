using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralysed : MonoBehaviour
{
    [SerializeField]
    public float TimeLife;
    public float damage;
    public float speed;
    void Start()
    {
        Destroy(gameObject, TimeLife);
    }

    

    void OnTriggerEnter(Collider other)
    {
        EnemyStateManager enemyStateManager = other.GetComponent<EnemyStateManager>();
        
        if (other.CompareTag("Ennemy"))
        {
            enemyStateManager.Stunned = true;
            Destroy(gameObject);
        }
    }
}
