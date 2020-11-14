using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessControl : MonoBehaviour
{
    // Start is called before the first frame update

    private PlayerController player;
    private Vignette vignette;
    private PostProcessVolume volume;

    void Start()
    {
        player = GameObject.Find("PlayerSprite").GetComponent<PlayerController>();

        volume = GetComponent<PostProcessVolume>();

        volume.profile.TryGetSettings(out vignette);
    }

    // Update is called once per frame
    void Update()
    {
        if (player.gameover)
        {
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0.3f, 1 * Time.deltaTime);
            vignette.smoothness.value = Mathf.Lerp(vignette.smoothness.value, 0.4f, 0.4f * Time.deltaTime);
        }
    }
}
