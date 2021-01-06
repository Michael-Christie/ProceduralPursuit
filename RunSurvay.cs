using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunSurvay : MonoBehaviour
{
    public GameManager.StartOrder survayChoice;

    private void Awake() { DontDestroyOnLoad(this); }

    public void SetChoise(GameManager.StartOrder choice) { survayChoice = choice; }

    public void Survay()
    {
        FindObjectOfType<GameManager>();

        if(survayChoice == GameManager.StartOrder.Quest)
        {
            //runs the quest survay
            Application.OpenURL("https://forms.gle/4aYjnSrVzBnukeee7");
        }
        else
        {
            //run the village survay
            Application.OpenURL("https://forms.gle/Pv9Sgesdgh1mewZV9");
        }
    }
}
