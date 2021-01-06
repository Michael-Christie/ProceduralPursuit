using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndScreenHandler : MonoBehaviour
{
    private void Start()
    {
        FindObjectOfType<RunSurvay>().Survay();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Done()
    {
        LevelLoader.instance.roundNumber++;
        LevelLoader.instance.canSwitch = true;

        if (LevelLoader.instance.roundNumber <= 1)
            SceneManager.LoadScene(1);
        else
            SceneManager.LoadScene(0);
    }
}
