using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyParticleScript : MonoBehaviour
{

    private ParticleSystem enemyDestroyParticleSystem;
    private ParticleSystem splitterFireSystem;

    void Start()
    {


        ParticleSystem[] PS = GetComponentsInChildren<ParticleSystem>();

        enemyDestroyParticleSystem = Array.Find<ParticleSystem>(PS, element => element.gameObject.name.Equals("DestroyPS"));
        splitterFireSystem = Array.Find<ParticleSystem>(PS, element => element.gameObject.name.Equals("FiringPS"));
    }

    public void Emit()
    {
        if (!enemyDestroyParticleSystem.gameObject.activeSelf)
        {
            enemyDestroyParticleSystem.gameObject.SetActive(true);
        }
        //Player Recieves Break Damage Anim
        enemyDestroyParticleSystem.Emit(60);
    }

    public void EmitFire()
    {
        if (!splitterFireSystem.isPlaying)
        {
            splitterFireSystem.Play();
        }
        else if(splitterFireSystem.isPlaying)
        {
            splitterFireSystem.Stop();
        }
    }

}
