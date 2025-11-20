using UnityEngine;

public class WallMoveUp : MonoBehaviour
{
    public float moveDistance = 2f;
    public float moveSpeed = 2f;
    public Transform player;
    private bool isMoving = false;
    private bool playerInRange = false;

    private Vector3 startPos;
    private Vector3 targetPos;


    public AudioSource audioSource;
    public AudioClip sfxMoveup;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        targetPos = startPos + new Vector3(0, moveDistance, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isMoving&&playerInRange)
        {
            var pc = player.GetComponent<PlayerController>();

            if(pc.hasKey)
            {
                isMoving = true;
                audioSource.PlayOneShot(sfxMoveup);
            }
        }

        if(isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed *Time.deltaTime
                );

            if(Vector3.Distance(transform.position,targetPos)<0.01f)
            {
                isMoving=false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            player = other.transform;
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }


}
