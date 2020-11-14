using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCooldownEffect : MonoBehaviour
{
    // Start is called before the first frame update

    private PlayerController player;

    void Start()
    {
        player = GameObject.Find("PlayerSprite").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player)
        {

        }
    }
}
