using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureScript : MonoBehaviour
{

    private Vector3 direction; // Unit vector in the direction you want to move
    private float speed = 1f; // Speed of the movement
    private float duration = 2f; // Duration of the movement in seconds

    private float elapsedTime = 0f;


    void Awake(){
        
    }

    // Start is called before the first frame update
    void Start()
    {
        direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (elapsedTime < duration)
        {
            transform.position += direction * speed * Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }
        else
        {
            direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
            elapsedTime = 0f;
        }
    }
}
