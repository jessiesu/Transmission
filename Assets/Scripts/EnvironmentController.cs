﻿using System.Collections;
using UnityEngine;

public class EnvironmentController : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("EnemyProjectile"))
            Destroy(other.gameObject);
    }
}
