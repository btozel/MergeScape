using TMPro;
using UnityEngine;

public class CreatureCreationScript : MonoBehaviour
{

    public GameObject[] creatures;

    public GameObject arena;

    public GameObject score;

    private TMP_Text scoreText;

    private int numberOfCreatures = 40;


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
        Destroy(creatureScript);
        mainCreature.AddComponent<MainCreatureLocomotion>();
        // mainCreature.transform.localScale += new Vector3(2f,2f,2f);
        mainCreature.transform.position = new Vector3(0f, 0f, 0f);
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
            return creature;
    }


    private Vector3 GetRandomLocationInArena()
    {
        var renderer = arena.GetComponent<MeshRenderer>();
        return new(Random.Range(-renderer.bounds.size.x, renderer.bounds.size.x), 0.1f, Random.Range(-renderer.bounds.size.z, renderer.bounds.size.z));
    }


    private int frameCount = 0;

    void Update()
    {
        frameCount++;
        if (frameCount % 90 == 0)
        {
            scoreText.text = Random.Range(0, 1000000).ToString();
        }
    }
}