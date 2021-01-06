using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("Generators")]
    public VilageGenerator VG;
    public QuestGenerator QG;
    [Header("")]
    public GameObject Player;
    public bool RandomSeed;
    public int seed;
    [Header("Animations")]
    public bool PlayCityBuildingAnimation;
    public Transform CinematicCamera;
    public Image fade;
    public LocalNavMeshBuilder meshBuilder;
    public Spawner[] Boids;
    public MiniMap miniMap;

    public enum StartOrder
    {
        Quest,
        Village
    }

    public StartOrder order;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        if (RandomSeed)
            seed = RandomNumber.Range(-1000, 1000);

        RandomNumber.Initialize(seed);
    }

    private void Start()
    {
        order = (StartOrder)LevelLoader.instance.RequestOrder();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(CreateWorld());
    }

    IEnumerator CreateWorld()
    {
        TerrainGenerator.instance.ResetTerrain();

        if (order == StartOrder.Village)
        {
            VG.Generate();

            yield return new WaitForSeconds(1f);
            QG.data = VG.CreateData();
            //Quest stuff here
            QG.Generate();
            QG.ConfigureQuest();

        }
        else
        {
            QG.Generate();
            VG.data = QG.CreateData();
            //quest stuff here
            VG.Generate();
            yield return new WaitForSeconds(1f);

            QG.g.abstactedData = VG.CreateData(); //this line should probably be replaced by some other way to save? the spawned locations and pass them back rather then juct making sure they are spawned?
            yield return new WaitForEndOfFrame();
            //pass back to the quest generator
            QG.ConfigureQuest();

        }

        yield return new WaitForSeconds(1f);
        //show level sceen
        fade.CrossFadeAlpha(0f, 1.5f, false);
        yield return new WaitForSeconds(1.5f);

        VG.RunAnimations();

        yield return new WaitForSeconds(12.5f);

        //fade to black
        fade.CrossFadeAlpha(1f, 1.5f, false);
        yield return new WaitForSeconds(1.5f);

        LoadPlayerIntoWorld();

        meshBuilder.UpdateNavMesh(true);


        foreach (Spawner s in Boids)
            s.Run();

        FindObjectOfType<BoidManager>().StartBoids();

        yield return new WaitForSeconds(.5f);
        VG.StartNPCs();
        //fade in
        yield return new WaitForSeconds(.5f);
        fade.CrossFadeAlpha(0f, 1.5f, false);

        QG.DisplayQuest();
        miniMap.CanStart();
    }

    Vector3 spawnPoint;
    Quaternion SpawnRotation;

    void LoadPlayerIntoWorld()
    {
        spawnPoint = new Vector3(255F, 5F, 255F);
        SpawnRotation = Quaternion.identity;
        if (GameObject.Find("SpawnPoint1"))
        {
            spawnPoint = GameObject.Find("SpawnPoint1").transform.position;
            SpawnRotation = GameObject.Find("SpawnPoint1").transform.rotation;
        }
        else if (GameObject.Find("SpawnPoint2"))
        {
            spawnPoint = GameObject.Find("SpawnPoint2").transform.position;
            SpawnRotation = GameObject.Find("SpawnPoint2").transform.rotation;
        }
        else if (GameObject.Find("SpawnPoint3"))
        {
            spawnPoint = GameObject.Find("SpawnPoint3").transform.position;
            SpawnRotation = GameObject.Find("SpawnPoint3").transform.rotation;
        }

        GameObject g = Instantiate(Player, spawnPoint, SpawnRotation);
        g.name = "Player";
        CinematicCamera.gameObject.SetActive(false);

        miniMap.Initialize(g);
    }

    bool runningEnd = false;

    private void Update()
    {
        if (QG.g.finished && !runningEnd)
        {
            runningEnd = true;
            //starts the end screen
            StartCoroutine(StartEndProccess());
        }
    }

    IEnumerator StartEndProccess()
    {
        for (int i = 0; i < 5; i++)
        {
            QG.PlayPartical(new Color(.83f, .21f, .34f, 1));
            yield return new WaitForSeconds(1f);
        }

        fade.CrossFadeAlpha(1, 2f, false);

        for (int i = 0; i < 2; i++)
        {
            QG.PlayPartical(new Color(.83f, .21f, .34f, 1));
            yield return new WaitForSeconds(1f);
        }

        //GameObject Survay = new GameObject();
        //RunSurvay r = Survay.AddComponent<RunSurvay>();
        //r.SetChoise(GameManager.instance.order);

        //kill the player off?
        SceneManager.LoadScene(0);
    }

    public void PlayerDead()
    {
        StartCoroutine(PlayerIsDead());
    }

    IEnumerator PlayerIsDead()
    {
        //respawns the player
        fade.CrossFadeAlpha(1, .2f, false);
        yield return new WaitForSeconds(1f);
        DestroyImmediate(GameObject.Find("Player"));
        GameObject g = Instantiate(Player, spawnPoint, SpawnRotation);
        g.name = "Player";
        yield return new WaitForSeconds(1f);
        fade.CrossFadeAlpha(0, 1f, false);
    }
}
