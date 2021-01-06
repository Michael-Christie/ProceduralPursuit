
using UnityEngine;
using UnityEngine.UI;

public class FPS : MonoBehaviour
{
    public Text FPSCounter;

    void Update()
    {
        FPSCounter.text = (1 / Time.deltaTime).ToString();
    }
}
