using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    Camera PlayerCamera;
    public GameObject MiniMapCamera;
    public Image fade;

    bool switching = true;

    public void CanStart() => switching = false;

    public void Initialize(GameObject p)
    {
        PlayerCamera = p.transform.GetChild(0).GetComponent<Camera>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !switching)
        {
            StartCoroutine(SwitchCamera());
        }
    }

    IEnumerator SwitchCamera()
    {
        if(PlayerCamera == null)
        {
            PlayerCamera = GameObject.Find("Player").transform.GetChild(0).GetComponent<Camera>();
        }

        switching = true;
        fade.CrossFadeAlpha(1f, .25f, false);
        yield return new WaitForSeconds(.35f);

        MiniMapCamera.SetActive(!MiniMapCamera.activeInHierarchy);
        PlayerCamera.enabled = !PlayerCamera.enabled;

        if (MiniMapCamera.activeInHierarchy)
            DrawMiniMapGizmos.instance.Draw();
        else
            DrawMiniMapGizmos.instance.UnDraw();

        fade.CrossFadeAlpha(0f, .25f, false);
        yield return new WaitForSeconds(.35f);
        switching = false;
    }
}
