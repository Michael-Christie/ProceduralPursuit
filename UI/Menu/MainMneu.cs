using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMneu : MonoBehaviour
{
    public Texture2D mousePointer;
    public Image Fade;
    public RectTransform rotateMenu;
    public bool flip = false;
    public GameObject BGM;

    private void Awake()
    {
        //Cursor.visible = false;
        Cursor.SetCursor(mousePointer, new Vector2(.5f, .5f), CursorMode.Auto);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameObject m = GameObject.Find("BackGroundMusic");

        if (m != BGM)
            DestroyImmediate(BGM);
    }

    private void Start()
    {
        Fade.CrossFadeAlpha(0f, 1f, false);
    }


    public void LoadGame()
    {

        LevelLoader.instance.SetPlayerId(Random.Range(-1000,1000));
        LevelLoader.instance.roundNumber = 0;
        LevelLoader.instance.doneQuest = -1;
        StartCoroutine(GameFadeOut());
    }

    public void AboutSwitch()
    {
        flip = !flip;
    }

    float valueRotation = 0;

    private void Update()
    {
        if (flip && valueRotation > -90f)
        {
            valueRotation -= 180 * Time.deltaTime;
            rotateMenu.rotation = Quaternion.Euler(0, valueRotation, 0);
        }

        if (!flip & valueRotation < 0f)
        {
            valueRotation += 180 * Time.deltaTime;
            rotateMenu.rotation = Quaternion.Euler(0, valueRotation, 0);
        }
    }

    IEnumerator GameFadeOut()
    {
        Fade.CrossFadeAlpha(1f, 2.5f, false);
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(1);
    }

    IEnumerator QuitFadeOut()
    {
        Fade.CrossFadeAlpha(1f, 2.5f, false);
        yield return new WaitForSeconds(2.5f);
        Application.Quit();
    }

    public void QuitFromMenu()
    {
        StartCoroutine(QuitFadeOut());
    }

}
