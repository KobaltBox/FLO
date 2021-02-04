using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompletePanel : MonoBehaviour
{
    // Start is called before the first frame update

    private GameObject scoreToDisable;
    private GameManager gameManager;
    private CanvasGroup canvasGroup;
    private RectTransform recTransform;

    private float moveSpeed;
    void Start()
    {
        scoreToDisable = GameObject.Find("Score");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        recTransform = gameObject.GetComponent<RectTransform>();
        canvasGroup.interactable = false;

        moveSpeed = 0.8f;
    }

    // Update is called once per frame
    public void CompleteLevelPanel()
    {
        StartCoroutine("LevelComplete");
    }

    IEnumerator LevelComplete()
    {
        var t = 0f;
        Vector2 StartPosition = new Vector2(0f, -800f);

        //Move from off screen to center
        while (t < 3)
        {
            recTransform.anchoredPosition = Vector3.Lerp(StartPosition, Vector2.zero, t);
            t += Time.deltaTime / moveSpeed;
            yield return null;
        }
        canvasGroup.alpha = 1.0f;
        canvasGroup.interactable = true;
    }
}
