/**
 * Main GameManager
 * 
 * G. Micah Garrison
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;


public class App : MonoBehaviour
{
    public GameObject inventoryCanvas;
    public GameObject closeInvButton;
    public GameObject invTip;

    private GameObject player;
    private static bool inventoryOpen;

    public void Awake()
    {
        // Keep __app GameObject throughout all scenes
        DontDestroyOnLoad(gameObject);
        // Load the scene immediately after the _preload scene
        SceneManager.LoadScene(1);

        inventoryOpen = false;
        inventoryCanvas.SetActive(false);
    }

    void Update()
    {
        // Pull up invetory if 'I' key pressed
        if (Input.GetKeyDown(KeyCode.I) && SceneManager.GetActiveScene().name != "SplashScene")
        {
            InventoryOpen = !InventoryOpen;
            if (invTip.activeSelf)
            {
                StartCoroutine(FadeTextToZeroAlpha(3f, invTip));
                StartCoroutine(FadeTextToZeroAlpha(3f, invTip.transform.GetChild(0).gameObject));
            }
        }
    }

    public bool InventoryOpen
    {
        get
        {
            return inventoryOpen;
        }
        set
        {
            if (value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                StartCoroutine(FadeTextToOneAlpha(0.5f, inventoryCanvas));
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                StartCoroutine(FadeTextToZeroAlpha(0.5f, inventoryCanvas));
            }

            inventoryOpen = value;

            if (player = GameObject.Find("FPSController"))
            {
                player.GetComponent<CharacterController>().enabled = !value;
                player.GetComponent<FirstPersonController>().enabled = !value;
            }
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, GameObject item)
    {
        Image i;
        Text j;
        CanvasGroup k;
        if (i = item.GetComponent<Image>())
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
            while (i.color.a > 0.0f)
            {
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
                yield return null;
            }
        }
        else if (j = item.GetComponent<Text>())
        {
            j.color = new Color(j.color.r, j.color.g, j.color.b, 1);
            while (j.color.a > 0.0f)
            {
                j.color = new Color(j.color.r, j.color.g, j.color.b, j.color.a - (Time.deltaTime / t));
                yield return null;
            }
        }
        else if (k = item.GetComponent<CanvasGroup>())
        {
            k.alpha = 1;

            while (k.alpha > 0.0f)
            {
                k.alpha = k.alpha - (Time.deltaTime / t);
                yield return null;
            }
        }
        item.SetActive(false);
    }

    public IEnumerator FadeTextToOneAlpha(float t, GameObject item)
    {
        CanvasGroup k;
        if (k = item.GetComponent<CanvasGroup>())
        {
            k.alpha = 0;

            item.SetActive(true);
            while (k.alpha < 1.0f)
            {
                k.alpha = k.alpha + (Time.deltaTime / t);
                yield return null;
            }
        }
    }
}
