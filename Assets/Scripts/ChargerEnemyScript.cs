using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargerEnemyScript : MonoBehaviour
{

    //Parameters
    public float chargeCooldown;
    public float chargeDuration;
    public int enemyHealth;
    public int scoreValue;
    public float enemySpeed;
    public float chargeSpeed;
    public float colorFadeSpeed;
    public GameObject ammodrop;
    public GameObject score_pop_up;
    public float spawn_behaviour_duration;

    public Sprite[] reticlespritesarray;


    //Components
    private GameObject player;
    private GameObject score_parent;
    private GameObject reticle;
    private SpriteRenderer reticleSprite;
    private BoxCollider2D collisionvol;
    private SpriteRenderer sprite;
    private float chargeTimestamp;
    private Rigidbody2D enemy_rb;
    private Vector3 targetPosition;
    private Vector3 desiredPosition;
    private float spawn_time;
    private GameOverPanel goPanel;

    //Audio
    public AudioClip clip_step1;
    public AudioClip clip_step2;
    public AudioClip clip_charge;
    public AudioClip clip_hit;


    //States
    private bool spawning;
    private bool dead;
    private bool orbiting;
    private bool charging;


    private void Awake()
    {
        //Get time we were instantiated at so we can control initial behaviour
        spawn_time = Time.time;
        spawning = true;
        collisionvol = gameObject.GetComponent<BoxCollider2D>();
        collisionvol.isTrigger = true;

        //Setting spawn effect
        /*        spawnScale = transform.localScale * 0.5f;
                finalScale = transform.localScale;
                transform.localScale *= .5f;*/

        //Set Initial Empty Reticle
        reticle = transform.Find("ChargeReticle").gameObject;
        reticleSprite = reticle.GetComponent<SpriteRenderer>();

        reticleSprite.sprite = reticlespritesarray[0];
    }


    // Start is called before the first frame update
    void Start()
    {
        enemy_rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        score_parent = GameObject.Find("UI");
        player = GameObject.Find("PlayerSprite");
        goPanel = GameObject.Find("GameOverPanel").GetComponent<GameOverPanel>();

        //Enable coroutine to periodically charge the player
        StartCoroutine("Charge");

        //Initial States
        transform.up = -(transform.position - player.transform.position);
        dead = false;
        orbiting = false;
        charging = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Are we dead?
        if (dead)
        {
            sprite.color = Vector4.Lerp(new Vector4(sprite.color.r, sprite.color.g, sprite.color.b, 1), new Vector4(1, 1, 1, 1), Time.deltaTime * colorFadeSpeed);
        }
        //If we are done spawning lets do our normal behaviour
        if (!spawning)
        {
            //Check if its time to charge
            if(Time.time > chargeTimestamp)
            {
                charging = true;
            }

            if(!charging)
            {
                //Rotate Around Player
                transform.position = RotatePointAroundPivot(transform.localPosition,
                               player.transform.position,
                               Quaternion.Euler(0, 0, 45 * Time.deltaTime));
            }
            //Look at Player as long as we aren't adding charge force
            if (Time.time < chargeTimestamp + chargeDuration)
            {
                transform.up = -(transform.position - player.transform.position);
            }

        }
        //Check if we are done spawning
        else if (Time.time - spawn_time >= spawn_behaviour_duration)
        {
            spawning = false;
            orbiting = true;
            //Enable Collision
            collisionvol.isTrigger = false;
            //Set First Charge timestamp
            chargeTimestamp = Time.time + chargeCooldown;
        }
        //Until then lets do the spawning behaviour
        else
        {
            enemy_rb.AddForce(Vector3.Normalize(player.transform.position - gameObject.transform.position) / 4, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "bounds")
        {
            targetPosition = Vector2.Reflect(targetPosition, collision.contacts[0].normal);
        }
        if (collision.collider.tag == "Player")
        {
            goPanel.SetCauseofDeath("charger");
            collision.gameObject.SendMessage("changeCapacity", -1);
            collision.gameObject.BroadcastMessage("ammoDamageParticleAnimation");
            AudioManager.Instance.PlaySoundAtPoint(clip_hit, gameObject);
            
        }

    }

    public void TakeDamage()
    {
        enemyHealth--;
        AudioManager.Instance.PlaySoundAtPoint(clip_hit, gameObject);
        if (enemyHealth <= 0)
        {
            StartCoroutine("Die");

        }
    }

    IEnumerator Charge()
    {
        //Waiting until spawning is complete AND when we stop orbiting
        yield return new WaitWhile(() => orbiting && spawning);
        yield return new WaitUntil(() => charging);
        while (true)
        {
            yield return new WaitUntil(() => charging);
            //Function to make enemy charge here...
            reticleSprite.sprite = reticlespritesarray[1];
            AudioManager.Instance.PlaySoundAtPoint(clip_step1, gameObject);
            yield return new WaitForSeconds(1f);
            reticleSprite.sprite = reticlespritesarray[2];
            AudioManager.Instance.PlaySoundAtPoint(clip_step2, gameObject);
            yield return new WaitForSeconds(1f);
            float distance = Vector2.Distance(gameObject.transform.position, player.transform.position);
            //Force applied is based on distance to player
            enemy_rb.AddForce(transform.up * (chargeSpeed * distance), ForceMode2D.Impulse);
            reticleSprite.sprite = reticlespritesarray[0];
            AudioManager.Instance.PlaySoundAtPoint(clip_charge, gameObject);
            yield return new WaitForSeconds(chargeDuration);
            charging = false;
            //Set Time for next charge
            chargeTimestamp = Time.time + chargeCooldown;
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

    public static Vector2 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        Vector3 rotation =  angle * (point - pivot) + pivot;
        return new Vector2(rotation.x, rotation.y);
    }
}
