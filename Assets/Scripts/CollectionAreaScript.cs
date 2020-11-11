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

    // Start is called before the first frame update
    void Start()
    {
        sprite = gameObject.GetComponent<SpriteRenderer>();
        collectionarea = gameObject.GetComponent<CircleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //TODO: This should have a slight fade effect...
        //Check for collection input
        if(Input.GetKey(KeyCode.CapsLock))
        {
            fadeDelta = Mathf.SmoothDamp(1f, 0f, ref fadeSpeed, fadeTime);
            sprite.color = new Color(1f, 1f, 1f, fadeDelta);
            collectionarea.enabled = true;
        }
        else
        {
            fadeDelta = Mathf.SmoothDamp(0f, 1f, ref fadeSpeed, fadeTime);
            sprite.color = new Color(1f, 1f, 1f, fadeDelta);
            collectionarea.enabled = false;
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
