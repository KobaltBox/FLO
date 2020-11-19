using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupScoreEffect : MonoBehaviour
{
    // Start is called before the first frame update

    private RectTransform text_transform;
    private Text text;
    private Vector3 targetpoint;
    private GameObject gamemanager;

    //Timing
    private float start;
    public float delay;

    private void Awake()
    {
        //Store when the popup was instantiated
        start = Time.time;
        text = gameObject.GetComponent<Text>();
        text_transform = gameObject.GetComponent<RectTransform>();
        
    }
    void Start()
    {
        targetpoint = text_transform.localPosition + (new Vector3(0f, 200f));
    }

    // Update is called once per frame
    void Update()
    {
        text_transform.anchoredPosition = Vector3.Lerp(text_transform.anchoredPosition, targetpoint, 5 * Time.deltaTime);
        text.CrossFadeAlpha(0f, .5f, true);
        if(Time.time - start > delay)
        {
            //Destroy(gameObject);
        }
    }

    public void setValue(int scoreValue)
    {
        string score_text = scoreValue.ToString();
        text.text = score_text;
        SendMessageUpwards("IncreaseScore", scoreValue);
    }
}
