using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    //Score
    private static int game_score;
    //PauseState
    public bool game_paused;
    //GameOverState
    public bool game_over;
    //Player
    private GameObject player;


    private void Awake()
    {
        //Singleton Pattern for GameManger Script
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreaseScore(int value)
    {
        game_score += value;
    }
    public int getScore()
    {
        return game_score;
    }
}
