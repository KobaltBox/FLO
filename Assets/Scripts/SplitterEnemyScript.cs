using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitterEnemyScript: MonoBehaviour
{

    //Parameters
    public float fireCoolDown;
    public float fireDuration;
    public int enemyHealth;
    public int scoreValue;
    public float enemySpeed;
    public float colorFadeSpeed;
    public GameObject ammodrop;
    public GameObject score_pop_up;
    public float spawn_behaviour_duration;


    //Components
    private GameObject player;
    private GameObject score_parent;
    private GameObject v_reticle;
    private GameObject h_reticle;
    private GameObject v_beam;
    private GameObject h_beam;


    private SpriteRenderer vertical_reticle;
    private SpriteRenderer horizontal_reticle;


    private BoxCollider2D collisionvol;
    private SpriteRenderer sprite;
    private float fireTimestamp;
    private Rigidbody2D enemy_rb;
    private float spawn_time;
    private Vector3 targetPosition;


    //Audio
    public AudioClip clip_hit;
    public AudioClip clip_fire;

    //States
    private bool spawning;
    private bool dead;
    private bool navigating;
    private bool firing;


    private void Awake()
    {
        //Get time we were instantiated at so we can control initial behaviour
        spawn_time = Time.time;
        spawning = true;

        //TODO make this a thing
        //Setting spawn effect
        /*        spawnScale = transform.localScale * 0.5f;
                finalScale = transform.localScale;
                transform.localScale *= .5f;*/

        //Get Reticle Sprites
        v_reticle = transform.Find("SplitterReticle_V").gameObject;
        vertical_reticle = v_reticle.GetComponent<SpriteRenderer>();

        h_reticle = transform.Find("SplitterReticle_H").gameObject;
        horizontal_reticle = h_reticle.GetComponent<SpriteRenderer>();

        //Get Beams
        v_beam = transform.Find("Beam_V").gameObject;
        h_beam = transform.Find("Beam_H").gameObject;

        //initial states
        v_reticle.SetActive(false);
        h_reticle.SetActive(false);
        v_beam.SetActive(false);
        h_beam.SetActive(false);


    }


    // Start is called before the first frame update
    void Start()
    {
        enemy_rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        score_parent = GameObject.Find("UI");
        player = GameObject.Find("PlayerSprite");

        //Initial States
        dead = false;
        navigating = false;
        firing = false;


        StartCoroutine("Fire");
        SetTargetPosition();
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
            if (Time.time > fireTimestamp)
            {
                firing = true;
            }

            if (!firing)
            {
                //Move to Random Screenspace position
                transform.position = Vector3.Lerp(transform.position, targetPosition, enemySpeed * Time.deltaTime);

            }

        }
        //Check if we are done spawning
        else if (Time.time - spawn_time >= spawn_behaviour_duration)
        {
            spawning = false;
            navigating = true;
            //Set First Fire timestamp
            fireTimestamp = Time.time + fireCoolDown;
        }
        //Until then lets do the spawning behaviour
        else
        {
            enemy_rb.AddForce(Vector3.Normalize(player.transform.position - gameObject.transform.position) / 4, ForceMode2D.Impulse);
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

    private void SetTargetPosition()
    {
        float y = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);
        float x = Random.Range
                (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
        targetPosition =  new Vector2(x, y);
    }

    IEnumerator Fire()
    {
        //Waiting until spawning is complete AND when we stop orbiting
        yield return new WaitWhile(() => navigating && spawning);
        Debug.Log("Ready to Fire");
        yield return new WaitUntil(() => navigating);
        while (true)
        {
            yield return new WaitUntil(() => firing);
            Debug.Log("Beginning Firing");
            //Decide what direction we are firing in 0: horizontal 1: vertical
            int direction = Random.Range(0, 2);
            //Telegraph attack direction
            float blink = 1f;
            //Assign 'reticle' based on the direction
            GameObject reticle = direction == 0 ? h_reticle : v_reticle;
            for(int i=1; i < 15; i++)
            {
                //Toggle appropriate article
                if(reticle.activeSelf)
                {
                    reticle.SetActive(false);
                }
                else
                {
                    reticle.SetActive(true);
                }
                yield return new WaitForSeconds(1f / i);
            }
            if(!reticle.activeSelf)
            {
                reticle.SetActive(true);
            }



            //Activate Beam
            if(direction == 0)
            {
                h_beam.SetActive(true);
            }
            else if(direction == 1)
            {
                v_beam.SetActive(true);
            }
            //fx
            AudioManager.Instance.PlaySoundAtPoint(clip_fire, gameObject);
            Camera.main.GetComponent<CameraShake>().Shake(0.2f);

            //Functions to make enemy fire here...
            SendMessage("EmitFire");
            Debug.Log("Fire Started");
            yield return new WaitForSeconds(fireDuration);
            SendMessage("EmitFire");
            v_beam.SetActive(false);
            h_beam.SetActive(false);
            reticle.SetActive(false);
            Debug.Log("Fire Stopped");
            yield return new WaitForSeconds(.5f);
            firing = false;
            //Set Time for next charge
            fireTimestamp = Time.time + fireCoolDown;
            SetTargetPosition();
        }


    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Player")
        {
            collision.gameObject.SendMessage("changeCapacity", -1);
            collision.gameObject.BroadcastMessage("ammoDamageParticleAnimation");
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
