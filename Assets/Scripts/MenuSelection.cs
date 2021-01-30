using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MenuSelection : MonoBehaviour, IPointerEnterHandler
 
{
    public GameObject SelectionSprite;

    private GameObject MainMenuCanvas;
    private GameObject HowToPlayCanvas;
    private GameObject LoadingCanvas;

    private CanvasGroup MMCanvasGroup;
    private CanvasGroup HTPCanvasGroup;
    private CanvasGroup LoadingCanvasGroup;
    private VideoPlayer loadingvideo;

    public AudioClip clip_mouseover;
    public AudioClip clip_click;

    private Image selectionRenderer;
    private Transform selectionPosition;
    // Start is called before the first frame update
    void Start()
    {
        selectionRenderer = SelectionSprite.GetComponent<Image>();
        selectionPosition = gameObject.transform.GetChild(0);

        MainMenuCanvas = GameObject.Find("MainMenuCanvas");
        HowToPlayCanvas = GameObject.Find("HowtoPlayCanvas");
        LoadingCanvas = GameObject.Find("LoadingCanvas");
        loadingvideo = GameObject.Find("LoadingVideo").GetComponent<VideoPlayer>();

        MMCanvasGroup = MainMenuCanvas.GetComponent<CanvasGroup>();
        HTPCanvasGroup = HowToPlayCanvas.GetComponent<CanvasGroup>();
        LoadingCanvasGroup = LoadingCanvas.GetComponent<CanvasGroup>();

    }

    void Update()
    {

    }


    public void OnPointerEnter(PointerEventData pointerdata)
    {
        SelectionSprite.transform.position = selectionPosition.transform.position;
        AudioManager.Instance.PlaySoundClip(clip_mouseover, 0.3f);
    }

    //Button Click Event Handlers

    public void OpenScene(string Scene)
    {
        loadingvideo.Stop();
        loadingvideo.frame = 0;
        loadingvideo.Play();
        LoadingCanvasGroup.alpha = 1.0f;
        StartCoroutine(LoadScene(Scene));
        AudioManager.Instance.PlaySoundClip(clip_click, 0.3f);
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

    //Toggle MainMenu Canvas and Toggle HTP Canvas
    public void ToggleHTPCanvas()
    {
        AudioManager.Instance.PlaySoundClip(clip_mouseover, 0.3f);

        if (MMCanvasGroup.interactable)
        {
            MMCanvasGroup.interactable = false;
            MMCanvasGroup.blocksRaycasts = false;
            MMCanvasGroup.alpha = 0f;

            HTPCanvasGroup.interactable = true;
            HTPCanvasGroup.blocksRaycasts = true;
            HTPCanvasGroup.alpha = 1f;
        }
        else
        {
            HTPCanvasGroup.interactable = false;
            HTPCanvasGroup.blocksRaycasts = false;
            HTPCanvasGroup.alpha = 0f;

            MMCanvasGroup.interactable = true;
            MMCanvasGroup.blocksRaycasts = true;
            MMCanvasGroup.alpha = 1f;
        }
    }

    public void ToggleAboutCanvas()
    {

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
