using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
public class HUDPlayerLife : MonoBehaviour {

    public Transform lifePrefab;

    private PlayerController player;
    private int lifeCount = 0;
    private List<Transform> lives = new List<Transform>();
    private float ySpacing = 20.0f;
    private float xSpacing = 20.0f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void SetLife(int life)
    {
        lifeCount = life;
        float offset = ySpacing;

        for (int i = 0; i < lifeCount; i++)
        {
            lives.Add(Instantiate(lifePrefab, gameObject.transform, false));
            lives[i].transform.position = new Vector3(Screen.width - offset, Screen.height - xSpacing, 0f);
            offset += lives[i].GetComponent<Image>().rectTransform.rect.width + ySpacing;
        }
    }

    public void updateLife(int life)
    {
        int hideCount = lifeCount - life;

        for (int i = 0; i < lifeCount; i++)
        {
            Color color = lives[i].GetComponent<Image>().color;
            color.a = 1.0f;
            lives[i].GetComponent<Image>().color = color;
        }

        for (int i = 0; i < hideCount; i++)
        {
            Color color = lives[i].GetComponent<Image>().color;
            color.a = 0.3f;
            lives[i].GetComponent<Image>().color = color;
        }
    }
}
