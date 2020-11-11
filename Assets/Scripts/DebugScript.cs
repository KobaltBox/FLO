using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugScript : MonoBehaviour
{
    //UI
    public GameObject playerCharacter;
    public Text velocityText;
    public Text fireCooldownText;
    public Text dashCooldownText;
    public Text breakHealthText;


    //Data
    private Rigidbody2D playerPhysics;
    private PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        playerPhysics = playerCharacter.GetComponent<Rigidbody2D>();
        playerController = playerCharacter.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        velocityText.text = playerPhysics.velocity.ToString();
        fireCooldownText.text = playerController.firingCooldown.ToString();
        dashCooldownText.text = playerController.movementDisabled.ToString();
        breakHealthText.text = playerController.breakHealth.ToString();
    }
}
