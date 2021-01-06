using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Pause : MonoBehaviour
{
    public Image fade;
    public GameObject canvas;

    void ShowMenu()
    {
        canvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    void HideMenu()
    {
        canvas.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    bool inMenu = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            inMenu = !inMenu;

            if (inMenu)
                ShowMenu();
            else
                HideMenu();
        }
    }

    public void Back()
    {
        inMenu = false;
        HideMenu();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {

        //destroy any dont destroy on load?
        DontDestroyObject[] obj = FindObjectsOfType<DontDestroyObject>();

        foreach (DontDestroyObject o in obj)
            DestroyImmediate(o.gameObject);

        Time.timeScale = 1f;
        StartCoroutine(FadeOut());


    }

    IEnumerator FadeOut()
    {
        fade.CrossFadeAlpha(1f, .5f, false);
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(0);
    }
}
