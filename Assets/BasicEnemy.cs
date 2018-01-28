﻿using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class BasicEnemy : MonoBehaviour {

    public Transform target;
    public float updateRate = 2.0f;         // seconds before the path updates
    public Path path;
    public float speed = 300.0f;
    public ForceMode2D forceMode;
    public float nextWaypointDist = 3.0f;   // Max distance from AI to a waypoint for it to continue to the next waypoint

    [HideInInspector]
    public bool pathEnded = false;

    private Seeker seeker;
    private Rigidbody2D rb;
    private int currentWaypoint = 0;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        if (!target)
        {
            Debug.LogError("No target found!!");
            return;
        }

        // start nenw path to the target and return the result to OnPathComplete
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
    }

    private IEnumerator UpdatePath()
    {
        if (!target)
        {
            yield return false;
        }

        seeker.StartPath(transform.position, target.position, OnPathComplete);
        yield return new WaitForSeconds(1.0f / updateRate);
        StartCoroutine(UpdatePath());
    }

    private void OnPathComplete(Path p)
    {
        Debug.Log("Path found");
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (!target)
        {
            return;
        }

        // TODO: face player
        if (path == null)
        {
            return;
        }

        if (currentWaypoint > path.vectorPath.Count)
        {
            if (pathEnded)
            {
                return;
            }
            pathEnded = true;
            return;
        }

        pathEnded = false;

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;

        rb.AddForce(dir, forceMode);

        float dist = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

        if (dist < nextWaypointDist)
        {
            currentWaypoint++;
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}