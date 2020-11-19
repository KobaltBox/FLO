using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyParticleScript : MonoBehaviour
{

    private ParticleSystem enemyDestroyParticleSystem;

    void Start()
    {


        ParticleSystem[] PS = GetComponentsInChildren<ParticleSystem>();

        enemyDestroyParticleSystem = Array.Find<ParticleSystem>(PS, element => element.gameObject.name.Equals("DestroyPS"));
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

}
