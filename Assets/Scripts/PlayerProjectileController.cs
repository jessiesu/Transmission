using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileController : MonoBehaviour {

    public GameObject playerObject;
    private Collider2D playerCollider;

	void Start()
    {
        playerCollider = playerObject.GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject p = GameObject.Find("Player");
        Collider2D pc = p.GetComponent<Collider2D>();
        if (collision.collider != pc && collision.otherCollider != playerCollider)
            gameObject.SetActive(false);
    }

}
