using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureCreationScript : MonoBehaviour
{

    public GameObject[] creatures;

    public GameObject arena;

    public GameObject score;

    private TMP_Text scoreText;

    private readonly int numberOfCreatures = 40;

    public Dictionary<string , int> creaturesTable = new Dictionary<string, int>();


    // Start is called before the first frame update
    void Start()
    {
        scoreText = score.GetComponent<TMP_Text>();
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
        GameObject randomCreaturePrefabRef = creatures[Random.Range(0, creatures.Length)];
        GameObject creature = Instantiate(randomCreaturePrefabRef, randomLocationInArena, randomCreaturePrefabRef.transform.rotation);
        string nameOfCreature = creature.name;
        if(creaturesTable.ContainsKey(nameOfCreature)){
            int current = creaturesTable[nameOfCreature];
            creaturesTable[nameOfCreature] = current+1;
        }else{
            creaturesTable.Add(nameOfCreature, 1);
        }
        return creature;
    }


    private Vector3 GetRandomLocationInArena()
    {
        var renderer = arena.GetComponent<MeshRenderer>();
        return new(Random.Range((-renderer.bounds.size.x + 1) / 2f, (renderer.bounds.size.x - 1) / 2f), 0.1f, Random.Range((-renderer.bounds.size.z + 1) / 2f, (renderer.bounds.size.z - 1) / 2f));
    }


    public void SetScore(){
        scoreText.text = string.Format("{0:0000000}", 1425);  
    }

}