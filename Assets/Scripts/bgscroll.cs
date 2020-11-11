using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgscroll : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject bg;
    public GameObject player;
    public float xScrollSpeed;
    public float yScrollSpeed;

    private Rigidbody2D playerrb;

    void Start()
    {
        playerrb = player.GetComponent<Rigidbody2D>();
    }

    // LateUpdate is called once per frame
    void LateUpdate()
    {
        Vector2 UVscrollvector = new Vector2(player.transform.position.x * xScrollSpeed, Time.time * yScrollSpeed);
        bg.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", UVscrollvector);

    }
}
