using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorTweener : MonoBehaviour
{
    public Color startColor;
    public Color targetTarget;
    public bool loop;
    public float changeTime = 1.0f;
    public iTween.EaseType easeType = iTween.EaseType.easeInExpo;
    private string loopType = "none";

    void Start()
    {
        if (loop)
        {
            loopType = "pingpong";
        }

        iTween.ValueTo(gameObject, iTween.Hash("from", startColor, "to", targetTarget, "time", changeTime, "easetype", "easeInCubic", "onUpdate", "UpdateColor", "loopType", loopType));
    }
    void UpdateColor(Color newColor)
    {
        Image image = GetComponent<Image>();
        image.color = newColor;
    }
}
