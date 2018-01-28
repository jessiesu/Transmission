using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Seeker))]
public class BasicEnemy : PhasedGameObject {

    public float updateRate = 2.0f;         // seconds before the path updates
    public Path path;
    public float speed = 300.0f;
    public ForceMode2D forceMode;
    public float nextWaypointDist = 3.0f;   // Max distance from AI to a waypoint for it to continue to the next waypoint

    [HideInInspector]
    public bool pathEnded = false;

    private Transform target;
    private Seeker seeker;
    private Rigidbody2D rb;
    private int currentWaypoint = 0;

    public GameObject bulletPrefab;

    private float fireRate = 0.5f;
    private float timeToFire = 0;
    private float bulletSpeed = 15.0f;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();

        target = GameObject.FindGameObjectWithTag("Player").transform;

        if (!target)
        {
            Debug.LogError("No target found!!");
            return;
        }

        // start nenw path to the target and return the result to OnPathComplete
        seeker.StartPath(transform.position, target.position, OnPathComplete);

        StartCoroutine(UpdatePath());
        // call base "Start" function (PhasedGameObject)
        base.Start();
    }

    private void Update()
    {
        // shoot player
        Vector3 delta = (target.position - transform.position).normalized;

        float rotZ = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0.0f, 0.0f, rotZ);

        if (Time.time > timeToFire)
        {
            timeToFire = Time.time + (1 / fireRate);
            Shoot(delta);
        }
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

        if (path == null)
        {
            return;
        }

        if (currentWaypoint >= path.vectorPath.Count)
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
            PhasedGameObject pso = other.GetComponent<PhasedGameObject>();

            if ((pso.objectPhase & objectPhase) == 0)
            {
                Destroy(gameObject);
                Destroy(other.gameObject);
            }
        }
    }

    private void Shoot(Vector3 direction)
    {
        Debug.Log("Shooting");
        GameObject clone = Instantiate(bulletPrefab, transform.position, transform.rotation);
        Rigidbody2D cloneRb = clone.GetComponent<Rigidbody2D>();
        cloneRb.velocity = bulletSpeed * direction;
    }
}
