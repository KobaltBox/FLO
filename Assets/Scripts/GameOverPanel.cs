using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    // Start is called before the first frame update

    public float dissolve_time;

    private GameObject scoreToDisable;
    private CanvasGroup gameOverGroup;
    private Material gameOverPanelMaterial;
    private Transform[] gameOverPanelElements;

    private PlayerController player;

    void Start()
    {
        scoreToDisable = GameObject.Find("Score");
        gameOverPanelMaterial = gameObject.GetComponent<Image>().material;
        gameOverPanelElements = gameObject.GetComponentsInChildren<Transform>();
        gameOverGroup = GameObject.Find("_group").GetComponent<CanvasGroup>();
        player = GameObject.Find("PlayerSprite").GetComponent<PlayerController>();

        //Initially we want to disable the game over panel and its elements
        gameOverGroup.alpha = 0f;
        gameOverGroup.interactable = false;
        gameOverPanelMaterial.SetFloat("_Dissolve", 0f);
    }

    public void ActivateGameOverPanel()
    {
        //turn it all on...
        StartCoroutine(Fade("In", 2f));
    }

    IEnumerator Fade(string direction, float delay)
    {
        yield return new WaitForSeconds(delay);
        scoreToDisable.SetActive(false);

        var t = 0f;

        //configure direction
        float start;
        float end;
        start = direction.Contains("In") ? 0f : 1f;
        end = direction.Contains("In") ? 1f : 0f;
        while (t < 1f)
        {
            var dissolve_lerp = Mathf.Lerp(start, end, t);
            gameOverPanelMaterial.SetFloat("_Dissolve", dissolve_lerp);
            gameOverGroup.alpha = dissolve_lerp;
            t += Time.deltaTime / dissolve_time;
            yield return null;
        }
        gameOverPanelMaterial.SetFloat("_Dissolve", end);
        gameOverGroup.interactable = true;
    }

    public void PlayAgain()
    {
        //Reload the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OpenScene(string Scene)
    {
        StartCoroutine(Fade("Out",0f));
        var LoadingCanvas = GameObject.Find("LoadingCanvas");
        var LoadingCanvasGroup = LoadingCanvas.GetComponent<CanvasGroup>();
        var loadingvideo = GameObject.Find("LoadingVideo").GetComponent<VideoPlayer>();

        loadingvideo.Stop();
        loadingvideo.frame = 0;
        loadingvideo.Play();
        LoadingCanvasGroup.alpha = 1.0f;
        StartCoroutine(LoadScene(Scene));
    }


    public IEnumerator LoadScene(String scenename)
    {

        yield return new WaitForSeconds(4);

        try
        {
            AsyncOperation load = SceneManager.LoadSceneAsync(scenename);
        }
        catch (Exception e)
        {
            Debug.Log(string.Format("An Exception: {0} occured trying to load scene: {0}", e, scenename));
        }

    }
}
