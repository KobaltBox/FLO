using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ParticleSystemController : MonoBehaviour
{
    // Start is called before the first frame update

    private PlayerController player;

    private ParticleSystem fullCapacitySystem;
    private ParticleSystem breakDamageSystem;

    void Start()
    {
        player = GameObject.Find("PlayerSprite").GetComponent<PlayerController>();


        ParticleSystem[] PS = GetComponentsInChildren<ParticleSystem>();

        fullCapacitySystem = Array.Find<ParticleSystem>(PS, element => element.gameObject.name.Equals("FullCapacityPS"));
        breakDamageSystem = Array.Find<ParticleSystem>(PS, element => element.gameObject.name.Equals("BreakDamagePS"));
    }

    // Update is called once per frame
    void Update()
    {
        //Full Capacity Idle Anim
        if(fullCapacitySystem)
        {
            if (player.currentCapacity == player.maxCapacity)
            {
                var emission = fullCapacitySystem.emission;
                emission.enabled = true;
            }
            else
            {
                var emission = fullCapacitySystem.emission;
                emission.enabled = false;
            }
        }

    }

    public void breakDamageParticleAnimation()
    {
        if(!breakDamageSystem.gameObject.activeSelf)
        {
            breakDamageSystem.gameObject.SetActive(true);
        }
        //Player Recieves Break Damage Anim
        breakDamageSystem.Emit(30);
        
    }

    public void ammoDamageParticleAnimation()
    {
        if (!fullCapacitySystem.gameObject.activeSelf)
        {
            fullCapacitySystem.gameObject.SetActive(true);
        }
        //Player Recieves Break Damage Anim
        fullCapacitySystem.Emit(10);
    }
}
