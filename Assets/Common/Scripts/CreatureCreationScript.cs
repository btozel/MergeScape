using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CreatureCreationScript : MonoBehaviour
{

    public GameObject[] creatures;

    public GameObject arena;

    public GameObject score;

    private TMP_Text scoreText;

    public TMP_Text livesText;

    public GameObject finalScoreGO;

    public TMP_Text finalScoreText;

    public GameObject gameOverText;

    private readonly int numberOfCreatures = 40;

    private readonly int pointsPerMerge = 50;

    public Dictionary<string, int> creaturesTable = new();

    public static Action<GameObject> MainCreatureCollided;

    public static Action MainCreatureCollidedWithAlien;

    public static Action<int> MainCreatureLiveCountUpdateEvent;

    public static Action<Boolean> FlashLiveCountUpdateEvent;

    public static Action GameOverEvent;

    private GameObject mainCreature;

    private int timeSpentPlaying = 0;

    private bool isGameOver = false;

    public AudioClip growAudio;
    public AudioClip shrinkAudio;


    // Start is called before the first frame update
    void Start()
    {
        scoreText = score.GetComponent<TMP_Text>();
        MainCreatureCollided += HandleMainCreatureCollision;
        MainCreatureLiveCountUpdateEvent += MainCreatureLiveCountUpdate;
        MainCreatureCollidedWithAlien += HandleMainCreatureCollidedWithAlien;
        FlashLiveCountUpdateEvent += FlashLive;
        GameOverEvent += GameOver;
        CreateCreatures();
        CreateMainCreature();
        AddToScore(0);
    }


    void Update()
    {
        int secondsThusFar = (int)Math.Round(Time.timeSinceLevelLoad);
        if(timeSpentPlaying < secondsThusFar){
            timeSpentPlaying = secondsThusFar;
            AddToScore(-1);
        }
    }


    private void CreateMainCreature()
    {
        mainCreature = CreateCreature(GetPrefabReferenceFromExistingCreatures(), true);
        CreatureScript creatureScript = mainCreature.GetComponent<CreatureScript>();
        creatureScript.IsMainCreature = true;
        creatureScript.SetColliderTriggerOn();
        mainCreature.tag = "MainCreature";
        mainCreature.transform.position = new Vector3(0f, 0.1f, 0f);
    }


    private void CreateCreatures()
    {
        for (int i = 0; i < numberOfCreatures; i++)
        {
            CreateCreature(GetRandomCreaturePrefabRef(), false);
        }
    }


    private GameObject GetRandomCreaturePrefabRef()
    {
        return creatures[UnityEngine.Random.Range(0, creatures.Length)];
    }


    private GameObject CreateCreature(GameObject prefabRef, bool isMainCreature)
    {
        Vector3 randomLocationInArena = GetRandomLocationInArena();
        GameObject creature = Instantiate(prefabRef, randomLocationInArena, prefabRef.transform.rotation);
        string nameOfCreature = creature.name;
        creature.GetComponent<CreatureScript>().SetArenaBounds(arena.GetComponent<Renderer>().bounds.size);

        if (!isMainCreature)
        {
            if (creaturesTable.ContainsKey(nameOfCreature))
            {
                int current = creaturesTable[nameOfCreature];
                creaturesTable[nameOfCreature] = current + 1;
            }
            else
            {
                creaturesTable.Add(nameOfCreature, 1);
            }
        }

        return creature;
    }


    private GameObject GetPrefabReferenceFromExistingCreatures()
    {
        GameObject prefabRef = null;
        int total = creaturesTable.Keys.Count;
        List<string> keyList = new List<string>(creaturesTable.Keys);
        int randomCreatureIndex = UnityEngine.Random.Range(0, total);
        string nameOfTheCreature = keyList[randomCreatureIndex].Replace("(Clone)", string.Empty);
        for (int i = 0; i < creatures.Length; i++)
        {
            if (creatures[i].name.Equals(nameOfTheCreature))
            {
                prefabRef = creatures[i];
            }
        }
        return prefabRef;
    }


    private Vector3 GetRandomLocationInArena()
    {
        var renderer = arena.GetComponent<MeshRenderer>();
        return new(UnityEngine.Random.Range((-renderer.bounds.size.x + 1) / 2f, (renderer.bounds.size.x - 1) / 2f), 0.1f, UnityEngine.Random.Range((-renderer.bounds.size.z + 1) / 2f, (renderer.bounds.size.z - 1) / 2f));
    }


    private void HandleMainCreatureCollision(GameObject collidedCreature)
    {
        RemoveCreatureFromGame(collidedCreature);
        ChangeToAnotherCreature();
        GetComponent<AudioSource>().clip = growAudio;
        GetComponent<AudioSource>().Play();
        AddToScore(pointsPerMerge);
    }


    private void ChangeToAnotherCreature()
    {
        Vector3 prevMainCreaturePos = new Vector3(mainCreature.transform.position.x, mainCreature.transform.position.y, mainCreature.transform.position.z);
        Vector3 prevMainCreatureScale = new Vector3(mainCreature.transform.localScale.x, mainCreature.transform.localScale.y, mainCreature.transform.localScale.z);
        int remainingLives = mainCreature.GetComponent<CreatureScript>().numberOfLives;
        int prevGrowthStep = mainCreature.GetComponent<CreatureScript>().currentGrowthStep;

        Destroy(mainCreature);

        CreateMainCreature();

        mainCreature.transform.position = prevMainCreaturePos;
        mainCreature.transform.localScale = prevMainCreatureScale;
        mainCreature.GetComponent<CreatureScript>().currentGrowthStep = prevGrowthStep;
        mainCreature.GetComponent<CreatureScript>().numberOfLives = remainingLives;
    }


    private void MainCreatureLiveCountUpdate(int remainingLives)
    {
        livesText.text = $"Lives: {remainingLives}";
    }

    private void FlashLive(Boolean immunity){
        if(immunity){
            livesText.GetComponent<MeshRenderer>().enabled = false;
        }else{
            livesText.GetComponent<MeshRenderer>().enabled = true;
        }
    }


    private void HandleMainCreatureCollidedWithAlien()
    {
        GetComponent<AudioSource>().clip = shrinkAudio;
        GetComponent<AudioSource>().Play();
    }


    private void RemoveCreatureFromGame(GameObject creature)
    {
        Destroy(creature);
        if (creaturesTable.ContainsKey(creature.name))
        {
            creaturesTable[creature.name] = creaturesTable[creature.name] - 1;
        }

        if (creaturesTable[creature.name] == 0)
        {
            creaturesTable.Remove(creature.name);
        }
    }


    private void GameOver()
    {
        isGameOver = true;
        gameOverText.SetActive(true);
        finalScoreText.text = $"Score {scoreText.text}";
        finalScoreGO.SetActive(true);
    }


    private void AddToScore(int amount)
    {
        if(!isGameOver){
            int currentScore = int.Parse(scoreText.text);
            int scoreToSet = Math.Max(currentScore + amount, 0);
            scoreText.text = string.Format("{0:000}", scoreToSet);
        }
    }

}