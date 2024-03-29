﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    //Control
    public float xSpeed;
    public float ySpeed;
    public float diagonalOffset;
    public float dashCooldown;
    public float dashDuration;
    public float dashMagnitude;
    public float firingCooldownDuration;
    public int breakHealth;
    public float die_fade_time;

    //Initial States
    public int maxCapacity;
    public int startingCapacity;
    public bool gameover;

    public GameObject reticle;
    public GameObject healthMask;
    public GameObject projectile;
    public GameObject playerBorder;

    //*-----------------------------------------------*

    private Rigidbody2D playerPhysics;

    private float offsetSpeedx;
    private float offsetSpeedy;
    private float dashTimestamp;
    private float movementTimestamp;
    private Vector2 mouseScreenPosition;
    private Vector2 playerDirection;


    private bool horizontalMovement;
    private bool verticalMovement;
    public bool movementDisabled;

    private GameObject playerHealth;
    private TrailRenderer trail;
    private SpriteRenderer borderSprite;
    private SpriteRenderer[] healthSprites;

    //Audio
    public AudioClip clip_fire;
    public AudioClip clip_dodge;
    public AudioClip clip_hit;
    public AudioClip clip_true_damage;
    public AudioClip clip_collect;

    //Player States
    public int currentCapacity;
    public bool firingCooldown;
    public Material deatheffect;
    private float firingCooldownTimestamp;

    private Vector3[] capacityScale;
    //Vector between mouse position and this gameobject
    private Vector3 mousePlayerCharacter;

    private GameObject mainCamera;
    private GameObject gameOverPanel;
    private GameObject pauseGamePanel;

    private GameObject psController;
    private GameObject entityParent;
    // Start is called before the first frame update
    void Start()
    {
        //Player Initialisation
        gameover = false;
        currentCapacity = startingCapacity;
        capacityScale = new Vector3[]
        {
            new Vector3(0.0f, 0.0f, 1.0f),
            new Vector3(0.1f, 0.1f, 1.0f),
            new Vector3(0.2f, 0.2f, 1.0f),
            new Vector3(0.3f, 0.3f, 1.0f),
            new Vector3(0.4f, 0.4f, 1.0f),
            new Vector3(0.5f, 0.5f, 1.0f),
            new Vector3(0.6f, 0.6f, 1.0f),
            new Vector3(0.7f, 0.7f, 1.0f),
            new Vector3(0.8f, 0.8f, 1.0f),
            new Vector3(0.9f, 0.9f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
        };
        healthMask.transform.localScale = capacityScale[currentCapacity];
        //GameObjects
        playerPhysics = gameObject.GetComponent<Rigidbody2D>();
        borderSprite = GameObject.Find("PlayerBorder").GetComponent<SpriteRenderer>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        gameOverPanel = GameObject.Find("GameOverPanel");
        psController = GameObject.Find("PlayerParticleSystemsController");
        entityParent = GameObject.Find("Entities");
        playerHealth = GameObject.Find("PlayerHealth");
        pauseGamePanel = GameObject.Find("PausePanel");
        healthSprites = playerHealth.GetComponentsInChildren<SpriteRenderer>();
        borderSprite = playerBorder.GetComponent<SpriteRenderer>();
        trail = gameObject.GetComponent<TrailRenderer>();
        trail.emitting = false;

    }

    // Update is called once per frame
    void Update()
    {
        //Set movement disabled based on dash duration
        //Also enable emission of trail
        if (movementTimestamp >= Time.time)
        {
            movementDisabled = true;
            trail.widthMultiplier = capacityScale[currentCapacity].x;
            trail.emitting = true;
        }
        else
        {
            movementDisabled = false;
            trail.emitting = false;
        }

        //Movement
        offsetSpeedx = xSpeed;
        offsetSpeedy = ySpeed;
        if (!movementDisabled && !gameover)
        {
            if (Input.GetKey(KeyCode.W))
            {
                verticalMovement = true;
                offsetSpeedy = ySpeed;
                if (horizontalMovement)
                {
                    offsetSpeedy *= diagonalOffset;
                }
                playerPhysics.AddForce(new Vector2(0.0f, 1.0f) * offsetSpeedy);
            }
            if (Input.GetKey(KeyCode.S))
            {
                verticalMovement = true;
                offsetSpeedy = ySpeed;
                if (horizontalMovement)
                {
                    offsetSpeedy *= diagonalOffset;
                }
                playerPhysics.AddForce(new Vector2(0.0f, -1.0f) * offsetSpeedy);
            }
            if (Input.GetKey(KeyCode.A))
            {
                horizontalMovement = true;
                offsetSpeedx = xSpeed;
                if (verticalMovement)
                {
                    offsetSpeedx *= diagonalOffset;
                }
                playerPhysics.AddForce(new Vector2(-1.0f, 0.0f) * offsetSpeedx);
            }
            if (Input.GetKey(KeyCode.D))
            {
                horizontalMovement = true;
                offsetSpeedx = xSpeed;
                if (verticalMovement)
                {
                    offsetSpeedx *= diagonalOffset;
                }
                playerPhysics.AddForce(new Vector2(1.0f, 0.0f) * offsetSpeedx);
            }


            //Pause Input Logic
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseGamePanel.SendMessage("PauseGameToggle");
            }

            horizontalMovement = false;
            verticalMovement = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(playerPhysics.velocity.x) > 0.1f || Mathf.Abs(playerPhysics.velocity.y) > 0.1f))
        {
            //If dash off cooldown
            if (dashTimestamp <= Time.time)
            {
                dashTimestamp = Time.time + dashCooldown;
                //Perform dash and add cooldown
                playerPhysics.AddForce(playerPhysics.velocity.normalized * dashMagnitude, ForceMode2D.Impulse);
                //Lock Movement for duration
                movementTimestamp = Time.time + dashDuration;
                //sfx
                AudioManager.Instance.PlaySoundAtPoint(clip_dodge, gameObject);
            }

        }


        //Aiming
        //Season 2: We doing mouse controls now lads...

        mouseScreenPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        playerDirection = (mouseScreenPosition - (Vector2)transform.position).normalized;

        transform.up = playerDirection;

        //Firing
        //Check we have resource and then fire, decrease current capacity by 1 increment
        //If not animate strain on player character and put firing in 1 second cooldown + add break damage

        if (firingCooldownTimestamp >= Time.time)
        {
            firingCooldown = true;
        }
        else
        {
            firingCooldown = false;
        }
        if (firingCooldown)
        {
            //TODO: firing cooldown vfx?
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                //Check for ammo
                if (currentCapacity > 0)
                {
                    //Instanitate Projectile here
                    GameObject thisProjectile = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation, entityParent.transform);
                    //TODO: Projectile feels wonky need to send it on a vector to mouse position.
                    AudioManager.Instance.PlaySoundAtPoint(clip_fire, gameObject);
                    firingCooldownTimestamp = Time.time + firingCooldownDuration;
                    changeCapacity(-1);
                }
            }
        }
    }

    //Season 2 
    // Capacity and Health are now independent concepts, we are no longer punishing the player for collecting too much energy,
    // and no longer taking away health on attempt to perform an action that requires energy when the reserves are empty.

    void changeCapacity(int capacity)
    {
        //We should not affect capacity and therefore break health if the game is over
        if (!gameover)
        {
            //Switch on type of change to make to capacity
            switch (Mathf.Sign(capacity))
            {
                case -1:
                    //If we have the capacity
                    if (currentCapacity > 0)
                    {
                        //Reduce capacity
                        currentCapacity += capacity;
                        //Update sprite mask
                        healthMask.transform.localScale = capacityScale[currentCapacity];

                    }
                    else
                    {
                        //Handle 'Break' Damage
                        if (breakHealth > 0 && (!firingCooldown))
                        {
                            gameOverPanel.GetComponent<GameOverPanel>().SetCauseofDeath("self");
                            changeHealth(-1);
                        }
                        else if (breakHealth <= 0)
                        {
                            Debug.Log("Game Over");
                            GameOver();
                        }
                    }
                    break;
                case 1:
                    //Increase current capacity and put fire on cooldown
                    if (currentCapacity < maxCapacity)
                    {
                        currentCapacity += capacity;
                        AudioManager.Instance.PlaySoundAtPoint(clip_collect, gameObject);
                    }
                    //Update sprite mask
                    healthMask.transform.localScale = capacityScale[currentCapacity];
                    break;
                default:
                    break;
            }
        }
        
    }
    //Handle Trigger collion for splitter enemy beam
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(gameObject.tag != "collect")
        {
            if(collision.tag == "enemy")
            {
                changeCapacity(-1);
                gameOverPanel.GetComponent<GameOverPanel>().SetCauseofDeath("splitter");
                psController.SendMessage("ammoDamageParticleAnimation");
                Debug.Log("Hit by enemy");
                AudioManager.Instance.PlaySoundAtPoint(clip_hit, gameObject);
            }
        }
    }


    void changeHealth(int health)
    {
        //Switch on type of change to make to health
        switch (Mathf.Sign(health))
        {
            case -1:
                breakHealth += health;
                mainCamera.GetComponent<CameraShake>().Shake(0.8f);
                AudioManager.Instance.PlaySoundAtPoint(clip_true_damage, gameObject);
                psController.SendMessage("breakDamageParticleAnimation");
                break;
            case 1:
                if (breakHealth < 3)
                {
                    breakHealth += health;
                }
                break;
            default:
                break;
        }

        //Update Health UI
        foreach (SpriteRenderer sprite in healthSprites)
        {
            var number = int.Parse(sprite.gameObject.name.Substring(sprite.gameObject.name.Length - 1, 1));
            if (number > breakHealth)
            {
                sprite.enabled = false;
            }
        }
    }

    void GameOver()
    {
        //Set sprite material to 2d dissolve death shader
        gameover = true;
        gameOverPanel.SendMessage("ActivateGameOverPanel");
        reticle.gameObject.SetActive(false);
        movementDisabled = true;
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        //Animate player dissolve material
        var t = 0f;
        while (t < 1f)
        {
            var dissolve_lerp = Mathf.Lerp(1f, 0f, t);
            borderSprite.material.SetFloat("_Dissolve", dissolve_lerp);
            t += Time.deltaTime / die_fade_time;
            yield return null;
        }
        Time.timeScale = Mathf.Lerp(Time.timeScale, 0f, 0.8f * Time.deltaTime);
    }
}
