using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class LoadingVideoScript: MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        VideoPlayer vp = gameObject.GetComponent<VideoPlayer>();
        vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Loadingv2.mp4");
        RenderTexture.active = vp.targetTexture;
        GL.Clear(true, true, Color.black);
        RenderTexture.active = null;
        vp.frame = 0;
        vp.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
