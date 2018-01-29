using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum PhaseState {
    Blue = 0x01,
    Red = 0x02,
    Magenta = 0x03
};

public class GameManager : MonoBehaviour {
    public float levelStartDelay = 2f;

    public Image alertImg;
    public Animator anim;

    private int playerScore = 0;
    private GameObject playerScoreText;

    private GameObject alertText;
    private GameObject waveText;
    private bool doingSetup;

    private PhaseState currentPhase = PhaseState.Red;

    public PhaseState CurrentPhase { get { return currentPhase; } }

    PhaseMusic phaseMusic;

    private void Awake()
    {
        phaseMusic = gameObject.GetComponent<PhaseMusic>();
    }

    // Use this for initialization
    void Start() {
        playerScoreText = GameObject.Find("Score");
        alertText = GameObject.Find("alertText");
        waveText = GameObject.Find("waveText");

        ResetScore();
        StartWave(1);
        phaseMusic.PlayRedTrack();
    }

    IEnumerator Fading()
    {
        anim.SetBool("Fade", true);
        yield return new WaitForSeconds(5);
        alertText.SetActive(false);
        waveText.SetActive(false);
    }

    public void StartWave(int waveNumber)
    {
        Text waveTextText = waveText.GetComponent<Text>();

        waveTextText.text = "INCOMING WAVE " + waveNumber;
        alertText.SetActive(true);
        waveText.SetActive(true);
        doingSetup = true;
        StartCoroutine(Fading());
    }

    private void HideAlertImage()
    {
        //levelImage.enabled = false;
        doingSetup = false;
    }

    public void GameOver()
    {
        
    }

    public void ResetScore()
    {
        playerScore = 0;
        UpdateScore();
    }

    public void UpdateScore(int change=0)
    {
        playerScore += change;
        Text text = playerScoreText.GetComponent<Text>();
        text.text = ("Score: " + playerScore);
    }

    // respawn player after a delay, destroy all "phased objects" in the scene
    public void RespawnPlayer(GameObject player, float delay)
    {
        StartCoroutine(Respawn(player, delay));
    }

    IEnumerator Respawn(GameObject player, float delay)
    {
        yield return new WaitForSeconds(delay);

        player.SetActive(true);
        Rigidbody2D playerRb2d = player.GetComponent<Rigidbody2D>();
        playerRb2d.velocity = new Vector2();
        playerRb2d.position = new Vector2();

        foreach(PhasedGameObject phasedObject in FindPhasedGameObjects())
            Destroy(phasedObject.gameObject);
    }

    // Update is called once per frame
    void Update() {
    }

    public void ChangePhase(PhaseState phase)
    {
        currentPhase = phase;

        int layerDefault = LayerMask.NameToLayer("Default");
        int layerEnemyRed = LayerMask.NameToLayer("EnemyRed");
        int layerEnemyBlue = LayerMask.NameToLayer("EnemyBlue");
        if (phase == PhaseState.Red)
        {
            Physics2D.IgnoreLayerCollision(layerDefault, layerEnemyRed, true);
            Physics2D.IgnoreLayerCollision(layerDefault, layerEnemyBlue, false);
            phaseMusic.PlayRedTrack();
        }
        else if (phase == PhaseState.Blue)
        {
            Physics2D.IgnoreLayerCollision(layerDefault, layerEnemyRed, false);
            Physics2D.IgnoreLayerCollision(layerDefault, layerEnemyBlue, true);
            phaseMusic.PlayBlueTrack();
        }

        foreach(PhasedGameObject phasedObject in FindPhasedGameObjects())
            phasedObject.PlayerPhaseSwitched(phase);
    }

    // get all the phased game objects behaviour scripts
    public List<PhasedGameObject> FindPhasedGameObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("Projectile"));
        gameObjects.AddRange(GameObject.FindGameObjectsWithTag("EnemyProjectile"));

        List<PhasedGameObject> phasedObjects = new List<PhasedGameObject>();
        foreach (GameObject go in gameObjects)
            phasedObjects.Add(go.GetComponent<PhasedGameObject>());
        return phasedObjects;
    }
}
