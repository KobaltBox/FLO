using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum movementType : int
{
    Constant,
    Instant
}


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
    public movementType movementType;
    public GameObject ammodrop;
    public GameObject score_pop_up;
    public GameObject score_parent;

    private Vector2 targetDirection;
    private SpriteRenderer sprite;
    private float changeTimestamp;
    private Rigidbody2D enemy_rb;
    private Vector2 targetPosition;
    private bool dead;

    // Start is called before the first frame update
    void Start()
    {
        enemy_rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();

        float initialx = Random.Range(-1.0f, 1.0f);
        float initialy = Random.Range(-1.0f, 1.0f);

        targetDirection = new Vector2(initialx, initialy);

        dead = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        //Pick a new direction
        if (changeTimestamp <= Time.time)
        {
            //Generate random x and y factors -1.0 - 1.0
            float randx = Random.Range(-1.0f, 1.0f);
            float randy = Random.Range(-1.0f, 1.0f);

            targetDirection = new Vector2(randx, randy);
            changeTimestamp = Time.time + changeCooldown;

            targetPosition = targetDirection * enemySpeed;
        }


        if (!dead)
        {
            //Move to targetPoint
            gameObject.GetComponent<Rigidbody2D>().AddForce(targetPosition);
        }
        else
        {
            sprite.color = Vector4.Lerp(new Vector4(sprite.color.r, sprite.color.g, sprite.color.b, 1), new Vector4(1, 1, 1, 1), Time.deltaTime * colorFadeSpeed);
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
            Debug.Log("Hit Player");
            
            collision.gameObject.SendMessage("changeCapacity",-1);
            collision.gameObject.BroadcastMessage("ammoDamageParticleAnimation");
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
