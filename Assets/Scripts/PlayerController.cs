using System.Collections;
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

    //Initial States
    public int startingCapacity;

    public GameObject reticle;
    public GameObject healthMask;
    public GameObject shootingSprite;

    //*-----------------------------------------------*

    private Rigidbody2D playerPhysics;

    private float offsetSpeedx;
    private float offsetSpeedy;
    private float dashTimestamp;
    private float movementTimestamp;

    private bool horizontalMovement;
    private bool verticalMovement;
    public bool movementDisabled;

    private Vector3 aimUp;
    private Vector3 aimDown;
    private Vector3 aimLeft;
    private Vector3 aimRight;
    private GameObject playerHealth;
    private SpriteRenderer[] healthSprites;

    //Player States
    public int currentCapacity;
    public bool firingCooldown;
    private float firingCooldownTimestamp;

    private Vector3[] capacityScale;

    private GameObject mainCamera;

    private GameObject psController;
    // Start is called before the first frame update
    void Start()
    {
        //Player Initialisation
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
        //healthMask.transform.localScale = capacityScale[currentCapacity];
        //GameObjects
        playerPhysics = gameObject.GetComponent<Rigidbody2D>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        psController = GameObject.Find("PlayerParticleSystemsController");
        playerHealth = GameObject.Find("PlayerHealth");
        healthSprites = playerHealth.GetComponentsInChildren<SpriteRenderer>();

        //Misc
        aimUp = new Vector3(0.0f, 0.0f, 0.0f);
        aimDown = new Vector3(0.0f, 0.0f, 180.0f);
        aimLeft = new Vector3(0.0f, 0.0f, 90.0f);
        aimRight = new Vector3(0.0f, 0.0f, 270.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //Set movement disabled based on dash duration
        if (movementTimestamp >= Time.time)
        {
            movementDisabled = true;
        }
        else
        {
            movementDisabled = false;
        }

        //Movement
        offsetSpeedx = xSpeed;
        offsetSpeedy = ySpeed;
        if (!movementDisabled)
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
                //Consume ammo
                changeCapacity(-1);
            }

        }


        //Aiming

        if (Input.GetKey(KeyCode.UpArrow))
        {
            reticle.transform.eulerAngles = aimUp;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            reticle.transform.eulerAngles = aimDown;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            reticle.transform.eulerAngles = aimLeft;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            reticle.transform.eulerAngles = aimRight;
        }

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
            shootingSprite.GetComponent<SpriteRenderer>().enabled = false;
            shootingSprite.GetComponent<BoxCollider2D>().enabled = false;
        }
        if (firingCooldown)
        {
           //TODO: firing cooldown vfx?
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                //Check for ammo
                if (currentCapacity > 0)
                {
                    //Active Collider and sprite
                    shootingSprite.GetComponent<BoxCollider2D>().enabled = true;
                    shootingSprite.GetComponent<SpriteRenderer>().enabled = true;
                }
                //We still attempt to take ammo in order to do break damage and put firing on cooldown
                firingCooldownTimestamp = Time.time + firingCooldownDuration;
                changeCapacity(-1);
            }
        }
    }

    void changeCapacity(int capacity)
    {
        ChangeType type;

        if (capacity > 0)
        {
             type = ChangeType.Add;
        }
        else
        {
            type = ChangeType.Subtract;
        }

        //Switch on type of change to make to capacity
        switch (type) {
            case ChangeType.Subtract:
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

                        changeHealth(-1);
                        mainCamera.GetComponent<CameraShake>().Shake(0.8f);
                        psController.SendMessage("breakDamageParticleAnimation");
                    }
                    else if (breakHealth <= 0)
                    {
                        Debug.Log("Game Over");
                        GameOver();
                    }
                }
                break;
            case ChangeType.Add:
                //Increase current capacity and put fire on cooldown
                if(currentCapacity<9)
                {
                    currentCapacity += capacity;
                }
                //Update sprite mask
                healthMask.transform.localScale = capacityScale[currentCapacity];
                break;
            default:
                break;
        }
    }

    void changeHealth(int health)
    {
        ChangeType type;

        if (health > 0)
        {
            type = ChangeType.Add;
        }
        else
        {
            type = ChangeType.Subtract;
        }

        //Switch on type of change to make to health
        switch (type)
        {
            case ChangeType.Subtract:
                breakHealth += health;
                break;
            case ChangeType.Add:
                if (breakHealth < 3)
                {
                    breakHealth += health;
                }
                break;
            default:
                break;
        }

        //Update Health UI
        foreach(SpriteRenderer sprite in healthSprites)
        {
            var number = int.Parse(sprite.gameObject.name.Substring(sprite.gameObject.name.Length - 1, 1));
            if(number > breakHealth)
            {
                sprite.enabled = false;
            }
        }
    }

    void GameOver()
    { }

        public enum ChangeType : int
    {
        Add,
        Subtract
    }
}
