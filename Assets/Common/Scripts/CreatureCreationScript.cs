using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;


public class CreatureCreationScript : MonoBehaviour
{

    public GameObject[] creatures;

    public GameObject arena;

    public GameObject score;

    private TMP_Text scoreText;

    public TMP_Text livesText;

    public GameObject gameOverText;

    private readonly int numberOfCreatures = 40;

    public Dictionary<string, int> creaturesTable = new();

    public static Action<GameObject> MainCreatureCollided;
    public static Action<int> MainCreatureLiveCountUpdateEvent;

    public static Action GameOverEvent;

    private GameObject mainCreature;


    // Start is called before the first frame update
    void Start()
    {
        scoreText = score.GetComponent<TMP_Text>();
        MainCreatureCollided += HandleMainCreatureCollision;
        MainCreatureLiveCountUpdateEvent += MainCreatureLiveCountUpdate;
        GameOverEvent += GameOver;
        CreateCreatures();
        CreateMainCreature();
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


    public void SetScore()
    {
        scoreText.text = string.Format("{0:0000000}", 1425);
    }


    private void HandleMainCreatureCollision(GameObject collidedCreature)
    {
        RemoveCreatureFromGame(collidedCreature);
        ChangeToAnotherCreature();
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
        gameOverText.SetActive(true);
    }


}