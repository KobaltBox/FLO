using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SocialLinks : MonoBehaviour
{
    public void OpenWebLink(string link)
    {
        Application.OpenURL(link);
    }

}
