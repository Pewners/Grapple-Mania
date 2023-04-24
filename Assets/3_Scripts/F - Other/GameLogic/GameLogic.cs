using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using RcLab;

public class GameLogic : MonoBehaviour
{
    public static GameLogic instance;

    [Header("Scene Management")]
    public List<string> levelNames;

    [Header("Point System")]
    public int currentPoints;
    public GameObject runFinishedScreen;

    private bool runFinished;

    private int enemiesDefeated;

    [Header("PlayerStats")]
    private Stats stats;
    public int gold;
    public TextMeshProUGUI text_gold;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        SceneManager.activeSceneChanged += OnSceneWasLoaded;

        stats = PlayerReferences.instance.stats;
    }

    private void Update()
    {
    }

    public void StartRun()
    {
        gold = 0;
        currentPoints = 0;

        text_gold.SetText(gold.ToString());

        NextLevel();
    }

    public void NextLevel()
    {
        string levelName = GetNextLevelName();
        SceneManager.LoadScene(levelName);

        Invoke(nameof(Delay), 0.5f);
    }

    private void Delay()
    {
        Spawner.instance.SpawnRoomRoutine();

        LevelStartRoutine();
    }

    string lastSceneName;
    private void OnSceneWasLoaded(Scene from, Scene to)
    {
        // fix scene loading twice
        if (to.name == lastSceneName) return;

        print("scene loaded: " + to.name);

        if (to.name == "MainScreen") return;

        Transform spawnPoint = GameObject.Find("SpawnPoint").transform;
        if(FindObjectOfType<PlayerMovement_MLab>() != null)
        {
            Transform player = FindObjectOfType<PlayerMovement_MLab>().transform;
            player.position = spawnPoint.position;
        }

        lastSceneName = to.name;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LevelStartRoutine()
    {
        
    }

    public void EndRun()
    {
        runFinished = true;

        //runFinishedScreen.SetActive(true);
        //runFinishedScreen.GetComponent<RunFinishedScreen>().Setup(currentPoints);
    }

    public void ResetPlayer()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnEnemyDefeated()
    {
        AddPoints(50);
        AddGold(35);

        enemiesDefeated++;

        if (enemiesDefeated >= 5)
        {
            if (portalToActivate != null)
            {
                portalToActivate.SetActive(true);

                // audio
                //AudioManager.PlayEffect(GameAssets.EffectSound.DoorOpen);
            }
        }
    }

    #region Point & Gold System

    public void AddPoints(int amount)
    {
        currentPoints += amount;
    }

    public void AddGold(int amount)
    {
        gold += 25;
        text_gold.SetText(gold.ToString());
    }

    public bool TrySubtractGold(int amount)
    {
        if(gold >= amount)
        {
            gold -= amount;
            text_gold.SetText(gold.ToString());
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    #region Level Selection

    private int lastLevel = -1;
    public string GetNextLevelName()
    {
        int randomLevel = Random.Range(0, levelNames.Count);

        while (randomLevel == lastLevel)
            randomLevel = Random.Range(0, levelNames.Count);

        lastLevel = randomLevel;
       
        return levelNames[randomLevel];
    }

    #endregion

    #region Ui



    #endregion

    #region Setters

    private GameObject portalToActivate;
    public void SetPortalOnEnemiesDefeated(GameObject portal)
    {
        portal.SetActive(false);
        portalToActivate = portal;
    }

    #endregion
}
