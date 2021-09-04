using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionAreaScript : MonoBehaviour
{

    public float fadeTime;
    public float fadeSpeed;
    public float collectionSpeed;

    private bool area_enabled;
    private float fadeDelta;
    private SpriteRenderer sprite;
    private CircleCollider2D collectionarea;
    private ParticleSystemRenderer collectPS;
    private GameObject player;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        collectionarea = gameObject.GetComponent<CircleCollider2D>();
        player = GameObject.Find("PlayerSprite");
        collectPS = gameObject.GetComponentInChildren<ParticleSystemRenderer>();
        collectPS.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: This should have a slight fade effect...
        //Check for collection input
        if(Input.GetMouseButton(1))
        {
            timer += Time.deltaTime;
            fadeDelta = Mathf.SmoothDamp(1f, 0f, ref fadeSpeed, fadeTime);
            sprite.color = new Color(1f, 1f, 1f, fadeDelta);
            collectionarea.enabled = true;

            //Mery Mechanic, If player has less than 2 ammo alllow generation of ammo by holding collection for 1 second
            if(player.GetComponent<PlayerController>().currentCapacity < 2)
            {
                collectPS.enabled = true;
                //Enable ps
                if(timer >= 1.0f)
                {
                    Debug.Log("Mercy Ammo Granted");
                    player.SendMessage("changeCapacity", 1);
                    timer = 0f;
                    //If this increase gets us to 2 ammo disable PS
                    if(player.GetComponent<PlayerController>().currentCapacity >= 2)
                    {
                        collectPS.enabled = false;
                    }
                }
            }
        }
        else
        {
            fadeDelta = Mathf.SmoothDamp(0f, 1f, ref fadeSpeed, fadeTime);
            sprite.color = new Color(1f, 1f, 1f, fadeDelta);
            collectionarea.enabled = false;
        }

        //On Up key press reset timer and disable ps
        if(Input.GetKeyUp(KeyCode.CapsLock))
        {
            timer = 0f;
            collectPS.enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "ammo")
        {
            other.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            other.gameObject.transform.position = Vector2.Lerp(other.gameObject.transform.position, gameObject.transform.position, Time.deltaTime * collectionSpeed);
        }
    }
}
