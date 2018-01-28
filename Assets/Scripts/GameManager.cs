﻿using System.Collections;
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

    private bool doingSetup;

    private PhaseState currentPhase = PhaseState.Red;
    private List<PhasedGameObject> phasedObjectList = new List<PhasedGameObject>();

    public PhaseState CurrentPhase { get { return currentPhase; } }

    IEnumerator Fading()
    {
        anim.SetBool("Fade", true);
        yield return new WaitForSeconds(5);
        GameObject.Find("alertText").SetActive(false);
        GameObject.Find("waveText").SetActive(false);
    }

    void StartWave()
    {
        GameObject.Find("alertText").SetActive(true);
        GameObject.Find("waveText").SetActive(true);
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

    // Use this for initialization
    void Start() {
        StartWave();
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
        }
        else if (phase == PhaseState.Blue)
        {
            Physics2D.IgnoreLayerCollision(layerDefault, layerEnemyRed, false);
            Physics2D.IgnoreLayerCollision(layerDefault, layerEnemyBlue, true);
        }

        List<PhasedGameObject> newList = new List<PhasedGameObject>();
        foreach(PhasedGameObject phasedObject in phasedObjectList)
        {
            if (phasedObject != null)
            {
                phasedObject.PlayerPhaseSwitched(phase);
                newList.Add(phasedObject);
            }
        }
        phasedObjectList = newList;
    }

    public void RegisterPhasedObject(PhasedGameObject phasedObject)
    {
        phasedObjectList.Add(phasedObject);
    }
}
