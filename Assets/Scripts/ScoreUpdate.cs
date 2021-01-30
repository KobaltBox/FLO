using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreUpdate : MonoBehaviour
{
    // Start is called before the first frame update
    private Text scoreText;
    private Text levelName;
    void Start()
    {
        levelName = GameObject.Find("LevelName").GetComponent<Text>();
        scoreText = gameObject.GetComponent<Text>();
        levelName.text = GameManager.Instance.levelname.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = GameManager.Instance.getScore().ToString();
    }
}
