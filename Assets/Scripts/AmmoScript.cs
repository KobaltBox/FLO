using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoScript : MonoBehaviour
{
    public float forceTime;
    public float forceSize;
    public GameObject player;

    private Rigidbody2D rb;
    private Vector2 direction;
    private float forceTimeStamp;
    private bool disableForce;

    // Start is called before the first frame update
    void Start()
    {

        //TODO: ADD life expectancy to instances of this object.
        rb = gameObject.GetComponent<Rigidbody2D>();
        float randx = Random.Range(-1.0f, 1.0f);
        float randy = Random.Range(-1.0f, 1.0f);
        direction = new Vector2(randx, randy);
        forceTimeStamp = Time.time + forceTime;
        disableForce = false;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if(forceTimeStamp > Time.time && !disableForce)
        {
            rb.AddForce(direction * forceSize, ForceMode2D.Impulse);
        }

        //Gross hard coded way of keeping ammo on screen... TODO: Fix this hardcodedness
        if(gameObject.transform.position.y + 0.1f >= 4.8 || gameObject.transform.position.y - 0.1f <= -4.8)
        {
            rb.velocity = Vector2.zero;
            disableForce = true;
        }
        if (gameObject.transform.position.x + 0.1f >= 8.6 || gameObject.transform.position.x - 0.1f <= -8.6)
        {
            rb.velocity = Vector2.zero;
            disableForce = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "collect")
        {
            Debug.Log("Ammo Got");
            GameObject.FindGameObjectWithTag("Player").SendMessage("changeCapacity", 1);
            Destroy(gameObject);
        }
    }
}
