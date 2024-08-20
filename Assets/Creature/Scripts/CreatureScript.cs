using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;
using TMPro;
using System.Numerics;

public class CreatureScript : MonoBehaviour
{

    private Vector3 direction;

    private Vector3 originalSize = new(0.5f, 0.1f, 0.5f);

    private Vector3 growthAmount = new(0.1f, 0f, 0.1f);

    private Vector3 arenaBound;

    private float speed = 1f;

    private float duration = 2f;

    private float elapsedTime = 0f;

    public bool IsMainCreature { get; set; } = false;

    public int currentGrowthStep { get; set; } = 0;

    public int numberOfLives { get; set; } = 3;

    private float immunityTimeSpan = 2f;
    private bool isInImmunity = true;

    public GameObject birthAnimation;


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
            birthAnimation.SetActive(true);
            speed = 2f;
        }
    }


    void Update()
    {
        if (numberOfLives == 0)
        {
            if (IsMainCreature)
            {
                transform.Rotate(Vector3.up * Time.deltaTime * 400f);
            }
            return;
        }

        if (!IsMainCreature)
        {
            if (transform.position.x <= -9.5)
            {
                ReflectMovementVector(Vector3.right);
            }
            else if (transform.position.x >= 9.5)
            {
                ReflectMovementVector(Vector3.left);
            }
            else if (transform.position.z <= -4.5)
            {
                ReflectMovementVector(Vector3.forward);
            }
            else if (transform.position.z >= 4.5)
            {
                ReflectMovementVector(Vector3.back);
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
            if (isInImmunity)
            {
                if (immunityTimeSpan <= 0f)
                {
                    immunityTimeSpan = 3f;
                    isInImmunity = false;
                }
                else
                {
                    immunityTimeSpan -= Time.deltaTime;
                }
            }

            Vector3 pos = transform.position;

            Vector3 bounds = GetComponent<Renderer>().bounds.size;

            if (Input.GetKey(KeyCode.UpArrow) && pos.z < arenaBound.z / 2f - bounds.z / 2f)
            {
                pos.z += speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.DownArrow) && pos.z > -arenaBound.z / 2f + bounds.z / 2f)
            {
                pos.z -= speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.RightArrow) && pos.x < arenaBound.x / 2f - bounds.z / 2f)
            {
                pos.x += speed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.LeftArrow) && pos.x > -arenaBound.x / 2f + bounds.z / 2f)
            {
                pos.x -= speed * Time.deltaTime;
            }

            transform.position = pos;
        }
    }

    public void SetArenaBounds(Vector3 arenaVector3)
    {
        arenaBound = arenaVector3;
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
                if (!isInImmunity)
                {
                    CreatureCreationScript.MainCreatureCollidedWithAlien.Invoke();
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


    private void ReflectMovementVector(Vector3 reflectNormal)
    {
        direction = Vector3.Reflect(direction, reflectNormal);
        elapsedTime = 0f;
        Move();
    }


    private void Redirect()
    {
        direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
        elapsedTime = 0f;
    }


    private void Move()
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
            currentGrowthStep = (int)Math.Round(currentGrowthStep * 0.66f);

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
                if (twoThirds.x < originalSize.x)
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
