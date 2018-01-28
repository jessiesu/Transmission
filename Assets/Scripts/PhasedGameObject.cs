using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhasedGameObject : MonoBehaviour {

    public PhaseState objectPhase = PhaseState.Red;
    public bool isPlayerProjectile = false;
    private GameManager gm;

    public void Start()
    {
        GameObject gmGo = GameObject.Find("_GM");
        gm = (GameManager)gmGo.GetComponent<GameManager>();
        this.PlayerPhaseSwitched(gm.CurrentPhase);
    }

    public void PlayerPhaseSwitched (PhaseState playerPhase) {
        float newAlpha = 0.5f;
        if (
            (!isPlayerProjectile && (playerPhase & objectPhase) == 0) ||
            (isPlayerProjectile && (playerPhase & objectPhase) > 0)
        )
            newAlpha = 1.0f;
        SpriteRenderer [] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in spriteRenderers)
        {
            Color tmp = sr.color;
            tmp.a = newAlpha;
            sr.color = tmp;
        }
	}
}
