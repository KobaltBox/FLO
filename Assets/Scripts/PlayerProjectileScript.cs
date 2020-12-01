using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectileScript : MonoBehaviour
{
    // Start is called before the first frame update

    public float kill_delay;
    public float speed;


    private Rigidbody2D rb;
    private Vector2 direction;
    private float kill_time;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        direction = gameObject.transform.up;
        kill_time = Time.time + kill_delay;
    }

    private void Update()
    {
        //Lifetime check
        if(Time.time > kill_time)
        {
            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        rb.AddForce(direction * speed, ForceMode2D.Impulse);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "enemy")
        {
            collision.gameObject.SendMessage("TakeDamage", 1);
            Destroy(gameObject);
        }
    }
}
