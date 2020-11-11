using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update

    public float shakeMagnitude;
    public float shakeDecay;

    private Vector3 initialPosition;
    public float shakeDuration;

    void Start()
    {
        initialPosition = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(shakeDuration > 0)
        {
            gameObject.transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeDuration -= Time.deltaTime * shakeDecay;
        }
        else
        {
            shakeDuration = 0f;
            gameObject.transform.localPosition = initialPosition;
        }
    }

    public void Shake(float duration)
    {
        shakeDuration = duration;
    }
}
