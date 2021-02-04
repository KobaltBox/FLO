using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.LWRP;

public class GameManager : MonoBehaviour
{

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private Button nextLevelButton;

    //Enemy Game Objects
    [SerializeField]
    private GameObject[] enemytypes;

    //Enemy Spawn Parent
    private Transform spawnParent;
    //Enemy Spawn Locations
    private List<Transform> spawnLocations = new List<Transform>();

    //Score
    private static int game_score;
    //PauseState
    public bool game_paused;
    //Game Over State
    public bool game_over;
    //Level over State
    public bool level_over;
    //Player
    private GameObject player;
    //Level Name
    public string levelname;
    //Custom Game Timer
    public float increment_timer;
    //Spawn block flag
    public bool blocked;

    private LevelCompletePanel levelCompletePanel;

    [SerializeField] LevelInfo levelinfo;

    [System.Serializable]
    public class LevelInfo
    {
        public int levelindex;
        public string levelname;
        public string description;
        public List<Increment> increments;

        [System.Serializable]
        public class Increment
        {
            public float time;
            public List<EnemySpawns> spawns;

            [System.Serializable]
            public class EnemySpawns
            {
                public int position;
                public int type;
                public int quantity;
            }
        }
    }

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
        //Set Spawn Parent
        spawnParent = GameObject.Find("Entities").transform;
        //Get Spawn Locations
        player = GameObject.Find("PlayerSprite");
        foreach (Transform t in GameObject.Find("SpawnLocations").transform)
        {
            print(t.gameObject.name);
            if(t.gameObject.name == "SpawnLocations")
            {
                break;
            }
            else
            {
                spawnLocations.Add(t);
            }
        }
        levelCompletePanel = GameObject.Find("LevelComplete").GetComponent<LevelCompletePanel>();
        blocked = false;

        //UI
        nextLevelButton = GameObject.Find("Next_Level_Button").GetComponent<Button>();
        nextLevelButton.interactable = true;

        game_score = 0;

        //Read in JSON
        TextAsset levelfile = Resources.Load("levels/" + levelname) as TextAsset;
        levelinfo = JsonUtility.FromJson<LevelInfo>(levelfile.text);
        increment_timer = 0f;
        StartCoroutine("Level");
    }

    // Update is called once per frame
    void Update()
    {
        //If we are not blocking spawns
        if(!blocked)
        {
            increment_timer += Time.deltaTime;
        }
    }

    public void IncreaseScore(int value)
    {
        game_score += value;
    }
    public int getScore()
    {
        return game_score;
    }

    public int resetScore()
    {
        game_score = 0;
        return game_score;
    }

    public void OpenScene(string Scene)
    {
        var LoadingCanvas = GameObject.Find("LoadingCanvas");
        var LoadingCanvasGroup = LoadingCanvas.GetComponent<CanvasGroup>();
        var loadingvideo = GameObject.Find("LoadingVideo").GetComponent<VideoPlayer>();

        loadingvideo.Stop();
        loadingvideo.frame = 0;
        loadingvideo.Play();
        LoadingCanvasGroup.alpha = 1.0f;
        StartCoroutine(LoadScene(Scene));
    }

    public void NextScene()
    {
        var sceneIndexes = SceneManager.sceneCountInBuildSettings -1;
        var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if(sceneIndexes >= nextSceneIndex)
        {
        SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            nextLevelButton.interactable = false;
        }
    }

    public void ResetScene()
    {
        //Reload the scene
        resetScore();
        increment_timer = 0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public IEnumerator LoadScene(String scenename)
    {

        yield return new WaitForSeconds(3);

        try
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(scenename);
        }
        catch (Exception e)
        {
            Debug.Log(string.Format("An Exception: {0} occured trying to load scene: {0}", e, scenename));
        }

    }


    IEnumerator Level()
    {
        float last_increment = levelinfo.increments[levelinfo.increments.Count - 1].time;
        blocked = false;
        foreach(LevelInfo.Increment increment in levelinfo.increments)
        {
            //Wait until the time has passed (WaitUntil evaluated after each Update() Call)
            // TODO Add addtional 'blocked' condition to force wait for all enemies to be killed + add to serialized classes (if blocked check for no enemies left)
            //If blocked false and last increment time is less than time.time then -> game over is true.
            yield return new WaitUntil(() => increment_timer > increment.time);
            foreach (LevelInfo.Increment.EnemySpawns spawn in increment.spawns)
            {
                if(spawn.type == -1f)
                {
                    blocked = true;
                    yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("enemy").Length == 0);
                    //After a block period we need to clear our timer
                    increment_timer = 0f;
                    blocked = false;
                }
                else
                {
                    //Enable Portal Graphics
                    enablePortal(spawnLocations[spawn.position].gameObject);
                    yield return new WaitForSeconds(0.5f);
                    for (int i = 0; i < spawn.quantity; ++i)
                    {
                        Instantiate(enemytypes[spawn.type], position: spawnLocations[spawn.position].position, rotation: Quaternion.identity, parent: spawnParent);
                        yield return new WaitForSeconds(0.3f);
                    }
                    //Disable Portal Graphics
                    yield return new WaitForSeconds(0.3f);
                    disablePortal(spawnLocations[spawn.position].gameObject);
                }
            }
        }
        yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("enemy").Length == 0);
        level_over = true;
        levelCompletePanel.CompleteLevelPanel();
        player.GetComponent<PlayerController>().gameover = true;
        yield return true;
    }

    void enablePortal(GameObject go)
    {
        go.GetComponent<ParticleSystemRenderer>().enabled = true;
        go.GetComponent<Light2D>().enabled = true;
        go.GetComponent<SpriteRenderer>().enabled = true;
    }

    void disablePortal(GameObject go)
    {
        go.GetComponent<ParticleSystemRenderer>().enabled = false;
        go.GetComponent<Light2D>().enabled = false;
        go.GetComponent<SpriteRenderer>().enabled = false;
    }
}
