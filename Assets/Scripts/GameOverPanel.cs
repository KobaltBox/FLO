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

    private Image EnemyPortrait;
    private Text EnemyTip;

    public Sprite[] enemySprites;
    public String[] Tips;

    public Sprite KillReportImage = null;
    public String KillReportTip = null;

    private PlayerController player;

    void Start()
    {
        scoreToDisable = GameObject.Find("Score");
        gameOverPanelMaterial = gameObject.GetComponent<Image>().material;
        gameOverPanelMaterial.SetFloat("_Dissolve", 0f);
        gameOverGroup = GameObject.Find("_group").GetComponent<CanvasGroup>();
        player = GameObject.Find("PlayerSprite").GetComponent<PlayerController>();

        //Kill Report
        EnemyPortrait = GameObject.Find("GO_Enemy_Portrait").GetComponent<Image>();
        EnemyTip = GameObject.Find("GO_Tip").GetComponent<Text>();

        //Initially we want to disable the game over panel and its elements
        gameOverGroup.alpha = 0f;
        gameOverGroup.blocksRaycasts = false;
        gameOverGroup.interactable = false;
        gameOverPanelMaterial.SetFloat("_Dissolve", 0f);
    }

    public void ActivateGameOverPanel()
    {
        //turn it all on...
        StartCoroutine(Fade("In", 2f));
    }

    //Set by last enemy collision
    public void SetCauseofDeath(string enemyname)
    {
        if (!player.gameover)
        {
            switch (enemyname)
            {
                case "ambient":
                    KillReportImage = enemySprites[0];
                    KillReportTip = Tips[0];
                    break;
                case "charger":
                    KillReportImage = enemySprites[1];
                    KillReportTip = Tips[1];
                    break;
                case "splitter":
                    KillReportImage = enemySprites[2];
                    KillReportTip = Tips[2];
                    break;
                case "self":
                    KillReportImage = enemySprites[3];
                    KillReportTip = Tips[3];
                    break;
            }
        }
    }

    IEnumerator Fade(string direction, float delay)
    {
        yield return new WaitForSeconds(delay);
        scoreToDisable.SetActive(false);

        EnemyPortrait.sprite = KillReportImage;
        EnemyTip.text = KillReportTip;

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
        gameOverGroup.blocksRaycasts = true;
    }
}
