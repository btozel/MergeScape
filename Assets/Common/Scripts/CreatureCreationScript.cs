using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

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


    // Start is called before the first frame update
    void Start()
    {
        scoreText = score.GetComponent<TMP_Text>();
        MainCreatureCollided += HandleMainCreatureCollision;
        MainCreatureLiveCountUpdateEvent += MainCreatureLiveCountUpdate;
        GameOverEvent += GameOver;
        CreateMainCreature();
        CreateCreatures();
    }


    private void CreateMainCreature()
    {
        GameObject mainCreature = CreateCreature();
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
            CreateCreature();
        }
    }


    private GameObject CreateCreature()
    {
        Vector3 randomLocationInArena = GetRandomLocationInArena();
        GameObject randomCreaturePrefabRef = creatures[UnityEngine.Random.Range(0, creatures.Length)];
        GameObject creature = Instantiate(randomCreaturePrefabRef, randomLocationInArena, randomCreaturePrefabRef.transform.rotation);
        string nameOfCreature = creature.name;
        if (creaturesTable.ContainsKey(nameOfCreature))
        {
            int current = creaturesTable[nameOfCreature];
            creaturesTable[nameOfCreature] = current + 1;
        }
        else
        {
            creaturesTable.Add(nameOfCreature, 1);
        }
        return creature;
    }


    private GameObject NextCreature(){
        GameObject nextOne = null;
        int total = creaturesTable.Count;
        List<String> keyList = new List<string>(creaturesTable.Keys);
        int randomCreatureIndex = UnityEngine.Random.Range(0,total - 1);
        String nameOfTheCreature = keyList[randomCreatureIndex];
        for(int i = 0; i < numberOfCreatures; i++){
            if(creatures[i].name == nameOfTheCreature){
                nextOne = creatures[i];
            }
        }
        return nextOne;
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
    }


    private void GameOver()
    {
        gameOverText.SetActive(true);
    }


}