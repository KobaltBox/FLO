using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientEnemyScript : MonoBehaviour
{

    public float changeCooldown;
    public int enemyHealth;
    public int scoreValue;
    public float enemySpeed;
    public float boundary_x;
    public float margin_x;
    public float boundary_y;
    public float margin_y;
    public float colorFadeSpeed;
    public GameObject ammodrop;
    public GameObject score_pop_up;

    private GameObject player;
    private GameObject score_parent;
    private BoxCollider2D collisionvol;
    private Vector3 targetDirection;
    private SpriteRenderer sprite;
    private float changeTimestamp;
    private Rigidbody2D enemy_rb;
    private Vector3 targetPosition;
    private float spawn_time;
    private float spawn_behaviour_duration;
    private bool spawning;
    private bool dead;


    private void Awake()
    {
        //Get time we were instantiated at so we can control initial behaviour
        spawn_time = Time.time;
        spawn_behaviour_duration = 0.5f;
        spawning = true;
        collisionvol = gameObject.GetComponent<BoxCollider2D>();
        collisionvol.isTrigger = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        enemy_rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        score_parent = GameObject.Find("UI");
        player = GameObject.Find("PlayerSprite");

        float initialx = Random.Range(-1.0f, 1.0f);
        float initialy = Random.Range(-1.0f, 1.0f);

        targetDirection = new Vector2(initialx, initialy);

        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Are we done being spawned yet?
        if(Time.time - spawn_time >= spawn_behaviour_duration)
        {
            spawning = false;
        }
        //Or are we dead?
        if(dead)
        {
            sprite.color = Vector4.Lerp(new Vector4(sprite.color.r, sprite.color.g, sprite.color.b, 1), new Vector4(1, 1, 1, 1), Time.deltaTime * colorFadeSpeed);
        }
        //If we are done spawning lets do our normal behaviour
        if (!spawning)
        {
            //Pick a new direction
            if (changeTimestamp <= Time.time)
            {
                //Generate random x and y factors -1.0 - 1.0
                float randx = Random.Range(-1.0f, 1.0f);
                float randy = Random.Range(-1.0f, 1.0f);

                targetDirection = new Vector3(randx, randy, 0f);
                changeTimestamp = Time.time + Random.Range(1.0f, 3.0f);

                targetPosition = Vector3.Normalize(targetDirection);
            }

            //Move to targetPoint
            enemy_rb.AddForce(targetPosition * enemySpeed);
        }
        //Until then lets do the spawning behaviour
        else
        {
            enemy_rb.AddForce(Vector3.Normalize(player.transform.position - gameObject.transform.position)/4, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == "bounds")
        {
            targetPosition = Vector2.Reflect(targetPosition, collision.contacts[0].normal);
        }
        if (collision.collider.tag == "Player")
        {   
            collision.gameObject.SendMessage("changeCapacity",-1);
            collision.gameObject.BroadcastMessage("ammoDamageParticleAnimation");
        }
        
    }

    //Once we exit the bounds object we set trigger flag to false;
    private void OnTriggerExit2D (Collider2D collision)
    {
        if(collision.tag == "bounds")
        {
            collisionvol.isTrigger = false;
        }
    }

    public void TakeDamage()
    {
        enemyHealth--;
        if(enemyHealth <= 0)
        {
            StartCoroutine("Die");
       
        }
    }

    IEnumerator Die()
    {
        dead = true;
        enemy_rb.velocity = Vector2.zero;
        SendMessage("Emit");
        yield return new WaitForSeconds(0.3f);
        int numtodrop = Random.Range(1, 3);
        for (int c = 0; c < numtodrop; c++)
        {
            ammodrop = Instantiate(ammodrop, gameObject.transform.position, Quaternion.identity);
            ammodrop.transform.SetParent(gameObject.transform.parent);
        }
        //Converson of world space of enemy to screen space for gui
        Vector3 position = Camera.main.WorldToScreenPoint(transform.position);
        score_pop_up = Instantiate(score_pop_up, position, Quaternion.identity);
        score_pop_up.transform.SetParent(score_parent.transform);
        score_pop_up.GetComponent<PopupScoreEffect>().setValue(scoreValue);

        Destroy(gameObject);
    }

}
