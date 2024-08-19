using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class CreatureScript : MonoBehaviour
{

    private Vector3 direction;

    private Vector3 originalSize = new(1f, 0.1f, 1f);

    private Vector3 growthAmount = new(0.1f, 0f, 0.1f);

    private Vector3 shrinkAmount = new(-0.2f, 0f, -0.2f);

    private float speed = 1f;

    private float duration = 2f;

    private float elapsedTime = 0f;

    public bool IsMainCreature { get; set; } = false;

    private int currentGrowthStep = 0;
    private int currentShrinkStep = 0;
    private int maxShrinkStep = 3;


    void Start()
    {
        if (!IsMainCreature)
        {
            duration = Random.Range(1f, 4f);
            speed = Random.Range(0.5f, 1.5f);
            direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f));
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
                CreatureCreationScript.MainCreatureCollided.Invoke(other.gameObject);
                GetBigger();
            }
            else
            {
                GetSmaller();
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
            if (currentShrinkStep >= maxShrinkStep)
            {
                //TODO: Show end results. 
                Debug.Log("Game Over");
                CreatureCreationScript.GameOverEvent.Invoke();
            }
            else if (transform.localScale == originalSize)
            {
                // Must mean the main creature collided with another at original size
                // Thus just increment currentShrinkStep; closer to death.
                currentShrinkStep++;
            }
            else
            {
                // The main creature must be bigger than original and still has lives. 
                // Just shrink.
                currentShrinkStep++;
                Vector3 scaleBy = currentShrinkStep * shrinkAmount;
                Vector3 scaleTo = originalSize + scaleBy;
                transform.localScale = scaleTo;
            }

        }
    }


}
