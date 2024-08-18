using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class CreatureCreationScript : MonoBehaviour
{

    public GameObject creature;
    public GameObject arena;

    private int numberOfCreatures = 40;

    // Start is called before the first frame update
    void Start()
    {
        var renderer = arena.GetComponent<MeshRenderer>();

        for(int i = 0; i < numberOfCreatures; i++){
            Vector3 randomLocationInArena = new Vector3(Random.Range(-renderer.bounds.size.x, renderer.bounds.size.x), 0.1f, Random.Range(-renderer.bounds.size.z, renderer.bounds.size.z));
            Instantiate(creature, randomLocationInArena, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
