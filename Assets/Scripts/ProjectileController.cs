using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : PhasedGameObject {

    public float lifetime = 10;
    private float age = 0;

    new void Start()
    {
        // call base "Start" function (PhasedGameObject)
        base.Start();
    }
	
	// Update is called once per frame
	void Update () {
        age += Time.deltaTime;
        if (age > lifetime)
            Destroy(this.gameObject);
	}
}
