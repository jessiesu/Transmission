using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {
    public float levelStartDelay = 2f;

    public Image alertImg;
    public Animator anim;

    private bool doingSetup;

    IEnumerator Fading()
    {
        anim.SetBool("Fade", true);
        yield return new WaitUntil(() => alertImg.color.a == 1);
    }

    void StartWave()
    {
        GameObject.Find("alertText").SetActive(true);
        GameObject.Find("waveText").SetActive(true);
        doingSetup = true;
        StartCoroutine(Fading());
        //GameObject.Find("alertText").SetActive(false);
        //GameObject.Find("waveText").SetActive(false);
    }

    private void HideAlertImage()
    {
        //levelImage.enabled = false;
        doingSetup = false;
    }

    public void GameOver()
    {
        
    }



	// Use this for initialization
	void Start () {
        StartWave();
    }

    

    // Update is called once per frame
    void Update () {
		
	}
}
