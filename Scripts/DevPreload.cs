/**
 * Script is used to auto preload the _preload scene
 * NOTE: this is not needed if already starting from the
 *       _preload scene. It is only used for development.
 */  
using UnityEngine;

public class DevPreload : MonoBehaviour
{
    void Awake()
    {
        GameObject check = GameObject.Find("__app");
        if (check == null)
        { UnityEngine.SceneManagement.SceneManager.LoadScene("_preload"); }
    }
}

