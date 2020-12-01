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

    private Renderer bgrender;

    private Rigidbody2D playerrb;

    void Start()
    {
        playerrb = player.GetComponent<Rigidbody2D>();
        bgrender = bg.GetComponent<Renderer>();
        bgrender.material.SetTextureScale("_BumpMap", new Vector2(6f, 7f));
    }

    // LateUpdate is called once per frame
    void Update()
    {
        Vector2 UVscrollvector = new Vector2(player.transform.position.x * xScrollSpeed, Time.time * yScrollSpeed);
        bgrender.material.SetTextureOffset("_MainTex", UVscrollvector);
        //Also have to move the normal map
        bgrender.material.SetTextureOffset("_NormalMap", UVscrollvector);

    }
}
