using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyParticleScript : MonoBehaviour
{
    public ParticleSystem ps;
    public int quantity;

    public void Emit()
    {
        ps.Emit(quantity);
    }

}
