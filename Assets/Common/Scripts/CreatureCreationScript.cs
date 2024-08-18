using TMPro;
using UnityEngine;

public class CreatureCreationScript : MonoBehaviour
{

    public GameObject creature;
    public GameObject arena;
    public GameObject score;

    private TMP_Text scoreText;

    private int numberOfCreatures = 40;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = score.GetComponent<TMP_Text>();
        var renderer = arena.GetComponent<MeshRenderer>();

        for(int i = 0; i < numberOfCreatures; i++){
            Vector3 randomLocationInArena = new Vector3(Random.Range(-renderer.bounds.size.x, renderer.bounds.size.x), 0.1f, Random.Range(-renderer.bounds.size.z, renderer.bounds.size.z));
            Instantiate(creature, randomLocationInArena, Quaternion.identity);
        }
    }


    private int frameCount = 0;

    void Update()
    {
        frameCount++;
        if(frameCount % 90 == 0){
            scoreText.text = Random.Range(0, 1000000).ToString();
        }
    }
}
