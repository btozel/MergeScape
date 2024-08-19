using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;
using TMPro;

public class CreatureScript : MonoBehaviour
{

    private Vector3 direction;

    private Vector3 originalSize = new(0.5f, 0.1f, 0.5f);

    private Vector3 growthAmount = new(0.1f, 0f, 0.1f);

    private float speed = 1f;

    private float duration = 2f;

    private float elapsedTime = 0f;

    public bool IsMainCreature { get; set; } = false;

    public int currentGrowthStep { get; set; } = 0;

    public int numberOfLives { get; set; } = 3;

    private float immunityTimeSpan = 2f;
    private bool isInImmunity = true;


    void Start()
    {
        if (!IsMainCreature)
        {
            duration = Random.Range(1f, 4f);
            speed = Random.Range(0.5f, 1.5f);
            direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));

            CreatureCreationScript.MainCreatureLiveCountUpdateEvent.Invoke(numberOfLives);
        }
        else
        {
            speed = 1.5f;
        }
    }


    void Update()
    {
        if (!IsMainCreature)
        {
            if ((transform.position.x <= -9.5 || transform.position.x >= 9.5) || (transform.position.z <= -4.5 || transform.position.z >= 4.5))
            {
                Redirect();
                Move();
            }
            else if (elapsedTime < duration)
            {
                Move();
            }
            else
            {
                Redirect();
            }            
        }
        else
        {
            if(isInImmunity)
            {
                Debug.Log("In immunity");
                if(immunityTimeSpan <= 0f)
                {
                    immunityTimeSpan = 3f;
                    isInImmunity = false;
                    Debug.Log("Not In immunity");
                }else
                {
                    immunityTimeSpan -= Time.deltaTime;
                }
            }

            Vector3 pos = transform.position;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                pos.z += speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                pos.z -= speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                pos.x += speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pos.x -= speed * Time.deltaTime;
            }

            transform.position = pos;
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (CompareTag("MainCreature"))
        {
            if (name.Equals(other.name))
            {
                GetBigger();
                CreatureCreationScript.MainCreatureCollided.Invoke(other.gameObject);
            }
            else
            {
                if(!isInImmunity)
                {
                    GetSmaller();
                    isInImmunity = true;            
                }
            }
        }
    }


    public void SetColliderTriggerOn()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }


    void Redirect()
    {
        direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        elapsedTime = 0f;
    }


    void Move()
    {
        transform.position += direction * speed * Time.deltaTime;
        elapsedTime += Time.deltaTime;
    }


    private void GetBigger()
    {
        if (IsMainCreature)
        {
            currentGrowthStep++;
            Vector3 scaleBy = currentGrowthStep * growthAmount;
            Vector3 scaleTo = originalSize + scaleBy;
            transform.localScale = scaleTo;
        }
    }


    private void GetSmaller()
    {
        if (IsMainCreature)
        {
           numberOfLives--;

            if (numberOfLives == 0)
            {
                //TODO: Show end results. 
                Debug.Log("Game Over");
                CreatureCreationScript.GameOverEvent.Invoke();
            }
            else
            {
                // The main creature must be bigger than original and still has lives. 
                // Just shrink.
                Vector3 scaleBy;
                Vector3 twoThirds = new(transform.localScale.x * .66f, transform.localScale.y, transform.localScale.z * 0.66f);
                if(twoThirds.x < originalSize.x)
                {
                    scaleBy = originalSize;
                }
                else
                {
                    scaleBy = twoThirds;
                }

                transform.localScale = scaleBy;
            }

            CreatureCreationScript.MainCreatureLiveCountUpdateEvent.Invoke(numberOfLives);

        }
    }


}
