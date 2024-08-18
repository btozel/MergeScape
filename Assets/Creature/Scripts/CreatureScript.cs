using UnityEngine;

public class CreatureScript : MonoBehaviour
{

    private Vector3 direction; 

    private float speed = 1f; 

    private float duration = 2f; 

    private float elapsedTime = 0f;

    public bool IsMainCreature { get; set; } = false;


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
            Destroy(other.gameObject);
        }
    }


    public void SetColliderTriggerOn(){
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

}
