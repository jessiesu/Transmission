using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Adding this allows us to access members of the UI namespace including Text.
using UnityEngine.UI;

internal delegate void PlayerActionEffect();

internal class PlayerAction
{
    public UnityEngine.KeyCode keycode;
    public float cooldown;
    public float cooldownRemaining = 0.0f;
    public PlayerActionEffect effect;

    public PlayerAction(UnityEngine.KeyCode keycode, float cooldown, PlayerActionEffect effect)
    {
        this.keycode = keycode;
        this.cooldown = cooldown;
        this.effect = effect;
    }
}

public class PlayerController : MonoBehaviour {

    // Tunable parameters
	public float speed = 10;
    public float boostCooldown = 6;
    public float boostSpeedMultiplier = 3;
    public float boostDuration = 3;
    public float shootCooldown = 0.2f;
    public float phaseCooldown = 0.5f;
    public float shootSpeed = 20;
    public GameObject shootPrefab;

    private GameManager gm;
    private List<PlayerAction> moveset = new List<PlayerAction>();
    private float speedMultiplier = 1.0f;
	private Rigidbody2D rb2d;		//Store a reference to the Rigidbody2D component required to use 2D Physics.

	void Start()
	{
		//Get and store a reference to the Rigidbody2D component so that we can access it.
		rb2d = GetComponent<Rigidbody2D> ();
        moveset.Add(new PlayerAction(KeyCode.LeftShift, boostCooldown, Boost));
        moveset.Add(new PlayerAction(KeyCode.Mouse0, shootCooldown, Shoot));
        moveset.Add(new PlayerAction(KeyCode.Space, phaseCooldown, PhaseSwitch));

        GameObject gmGo = GameObject.Find("_GM");
        gm = (GameManager)gmGo.GetComponent<GameManager>();
	}

    void Boost()
    {
        speedMultiplier = boostSpeedMultiplier;
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 bulletVector = (new Vector2(mousePos.x, mousePos.y) - rb2d.position).normalized;
        GameObject projectile = Instantiate(shootPrefab);
        Rigidbody2D projRb2d = (Rigidbody2D) projectile.GetComponent<Rigidbody2D>();
        CircleCollider2D projCollider = (CircleCollider2D) this.GetComponent<CircleCollider2D>();
        projRb2d.position = rb2d.position + (projCollider.radius * bulletVector);
        projRb2d.velocity = bulletVector * shootSpeed;
    }

    void PhaseSwitch()
    {
        PhaseState newState = gm.CurrentPhase == PhaseState.Blue ? PhaseState.Red : PhaseState.Blue;
        gm.ChangePhase(newState);
    }

	//FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
	void FixedUpdate()
	{
        foreach (var move in moveset)
        {
            if (Input.GetKey(move.keycode) && move.cooldownRemaining <= 0)
            {
                move.effect();
                move.cooldownRemaining = move.cooldown;
            }
            else if (move.cooldownRemaining > 0)
            {
                move.cooldownRemaining -= Time.deltaTime;
            }
        }

        // make boost decay linearly over time
        if (speedMultiplier > 1.0f)
            speedMultiplier -= (boostSpeedMultiplier - 1.0f) * (Time.deltaTime / boostDuration);

		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector2 movement = new Vector2 (moveHorizontal, moveVertical);
		rb2d.velocity = movement * speed * speedMultiplier;

        // rotate the player toward the mouse direction
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mouseVector = (new Vector2(mousePos.x, mousePos.y) - rb2d.position).normalized;
        float targetRotation = (Mathf.Atan2(mouseVector.y, mouseVector.x) * Mathf.Rad2Deg - 90) % 360;
        rb2d.rotation = targetRotation;
	}

	//OnTriggerEnter2D is called whenever this object overlaps with a trigger collider.
	void OnTriggerEnter2D(Collider2D other) 
	{
		//Check the provided Collider2D parameter other to see if it is tagged "PickUp", if it is...
		if (other.gameObject.CompareTag ("PickUp")) 
		{
			//... then set the other object we just collided with to inactive.
			other.gameObject.SetActive(false);
		}
	}
}
