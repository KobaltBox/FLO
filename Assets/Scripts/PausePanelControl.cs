using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PausePanelControl : MonoBehaviour
{
    // Start is called before the first frame update

    private CanvasGroup pausePanelGroup;
    private Material gameOverPanelMaterial;
    private Transform[] pausePanelElements;

    private PlayerController player;

    void Start()
    {
        player = GameObject.Find("PlayerSprite").GetComponent<PlayerController>();
        pausePanelElements = gameObject.GetComponentsInChildren<Transform>();
        pausePanelGroup = transform.Find("_group").gameObject.GetComponent<CanvasGroup>();

        //Initially we want to disable the game over panel and its elements
        pausePanelGroup.alpha = 0f;
        pausePanelGroup.blocksRaycasts = false;
        pausePanelGroup.interactable = false;
    }

    public void PauseGameToggle()
    {
        if(pausePanelGroup.interactable)
        {
            pausePanelGroup.alpha = 0f;
            pausePanelGroup.blocksRaycasts = false;
            pausePanelGroup.interactable = false;

            Time.timeScale = 1f;
            player.movementDisabled = false;
        }
        else
        {
            pausePanelGroup.alpha = 1f;
            pausePanelGroup.blocksRaycasts = true;
            pausePanelGroup.interactable = true;

            Time.timeScale = 0f;
            player.movementDisabled = true;
        }
    }

}

