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

[System.Serializable]
class PlayerStats
{
    private int maxLife;
    private int life;
    private PlayerController player;
    private GameManager gm;

    public PlayerStats(int maxLife, GameManager gameManager, PlayerController playerController)
    {
        this.maxLife = maxLife;
        this.life = maxLife;
        this.gm = gameManager;
        this.player = playerController;
    }

    public void takeDamage(int damage)
    {
        life--;
        gm.UpdateLife(life);
        player.gameObject.SetActive(false);
        gm.RespawnPlayer(player.gameObject, 1);

        if (life == 0)
        {
            life = maxLife;
            gm.UpdateLife(maxLife);
            // TODO: Game over
        }
    }
}

public class PlayerController : MonoBehaviour {

    // Tunable parameters
    public int maxLife = 3;
    public float speed = 10;
    public float boostCooldown = 6;
    public float boostSpeedMultiplier = 3;
    public float boostDuration = 3;
    public float shootCooldown = 0.2f;
    public float phaseCooldown = 0.5f;
    public float shootSpeed = 20;
    public GameObject shootPrefabBlue;
    public GameObject shootPrefabRed;
    public Sprite spriteBlue;
    public Sprite spriteRed;
    public AudioClip shootSound;
    public AudioClip deathSound;

    private GameManager gm;
    private List<PlayerAction> moveset = new List<PlayerAction>();
	private Rigidbody2D rb2d;		//Store a reference to the Rigidbody2D component required to use 2D Physics.
    private PlayerStats playerStats;

    private Vector2 boostVector = new Vector2();
    private float boostRemaining = 0.0f;
    private bool shielded = false; //damage immunity (activated by boost)
    private SpriteRenderer shieldSprite; //damage immunity (activated by boost)
    private Color redShieldColor = new Color(1.0f, 0.5f, 0.5f);
    private Color blueShieldColor = new Color(0.5f, 0.5f, 1.0f);
    private AudioSource audioSourceShoot;
    private AudioSource audioSourceDeath;

    private void Awake()
    {
        GameObject shieldGO = GameObject.Find("PlayerShield");
        shieldSprite = shieldGO.GetComponent<SpriteRenderer>();
    }

    void Start()
	{
		//Get and store a reference to the Rigidbody2D component so that we can access it.
		rb2d = GetComponent<Rigidbody2D> ();
        moveset.Add(new PlayerAction(KeyCode.LeftShift, boostCooldown, Boost));
        moveset.Add(new PlayerAction(KeyCode.Mouse0, shootCooldown, Shoot));
        moveset.Add(new PlayerAction(KeyCode.Space, phaseCooldown, PhaseSwitch));

        GameObject gmGo = GameObject.Find("_GM");
        gm = gmGo.GetComponent<GameManager>();
        gm.ChangePhase(gm.CurrentPhase);
        gm.SetLife(maxLife);

        playerStats = new PlayerStats(maxLife, gm, this);

        GameObject shieldGO = GameObject.Find("PlayerShield");
        shieldSprite = shieldGO.GetComponent<SpriteRenderer>();

        audioSourceShoot = GameObject.Find("SoundEffectsPlayerShoot").GetComponent<AudioSource>();
        audioSourceShoot.clip = shootSound;
        audioSourceDeath = GameObject.Find("SoundEffectsPlayerDeath").GetComponent<AudioSource>();
        audioSourceDeath.clip = deathSound;
    }

    Vector2 GetMouseVector()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (new Vector2(mousePos.x, mousePos.y) - rb2d.position).normalized;
    }

    void Boost()
    {
        boostVector = GetMouseVector() * boostSpeedMultiplier * speed;
        boostRemaining = boostDuration;
        shielded = true;
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 bulletVector = (new Vector2(mousePos.x, mousePos.y) - rb2d.position).normalized;

        GameObject projectile = null;
        if (gm.CurrentPhase == PhaseState.Red)
            projectile = Instantiate(shootPrefabRed);
        else if(gm.CurrentPhase == PhaseState.Blue)
            projectile = Instantiate(shootPrefabBlue);

        Rigidbody2D projRb2d = projectile.GetComponent<Rigidbody2D>();
        Collider2D playerCollider = this.GetComponent<Collider2D>();
        projRb2d.position = rb2d.position + (gameObject.transform.localScale.magnitude * bulletVector);
        projRb2d.velocity = bulletVector * shootSpeed;
        audioSourceShoot.Play();
    }

    void PhaseSwitch()
    {
        PhaseState newState = gm.CurrentPhase == PhaseState.Blue ? PhaseState.Red : PhaseState.Blue;
        gm.ChangePhase(newState);

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (gm.CurrentPhase == PhaseState.Red)
            sr.sprite = spriteRed;
        else if (gm.CurrentPhase == PhaseState.Blue)
            sr.sprite = spriteBlue;
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
        if (boostRemaining > 0)
        {
            boostRemaining -= Time.deltaTime;
            Color tmp = shieldSprite.color;
            Color shieldColor = redShieldColor;
            if (gm.CurrentPhase == PhaseState.Blue)
                shieldColor = blueShieldColor;
            tmp.r = shieldColor.r;
            tmp.g = shieldColor.g;
            tmp.b = shieldColor.b;
            tmp.a = (boostRemaining / boostDuration) * 0.4f;
            shieldSprite.color = tmp;
            boostVector -= boostVector * (boostSpeedMultiplier - 1.0f) * (Time.deltaTime / boostDuration);
        }
        else
        {
            Color tmp = shieldSprite.color;
            tmp.a = 0;
            shieldSprite.color = tmp;
            boostVector.x = 0;
            boostVector.y = 0;
            shielded = false;
        }

		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector2 movement = new Vector2 (moveHorizontal, moveVertical);
		rb2d.velocity = movement * speed + boostVector;

        // rotate the player toward the mouse direction
        Vector2 mouseVector = GetMouseVector();
        float targetRotation = (Mathf.Atan2(mouseVector.y, mouseVector.x) * Mathf.Rad2Deg - 90) % 360;
        rb2d.rotation = targetRotation;
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("EnemyProjectile"))
        {
            PhasedGameObject pso = other.GetComponent<PhasedGameObject>();
            if ((pso.objectPhase & gm.CurrentPhase) == 0 && !shielded)
            {
                playerStats.takeDamage(1);
                audioSourceDeath.Play();
                gameObject.SetActive(false);
                gm.RespawnPlayer(gameObject, 1);
            }
        }
    }
}
