using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{

    public Door targetDoor;
    public Transform exitPoint;
    public Animator anim;

    private bool playerInRange = false;
    private PlayerController player;

    private float openDuration = 0.5f;

    public AudioSource audioSource;
    public AudioClip sfxopen;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange)
        {
            if(Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                audioSource.PlayOneShot(sfxopen);
                StartCoroutine(Teleport());
            }
        }
    }
    private IEnumerator Teleport()
    {
        player.StopMoving();

        player.FaceToDoor();

        anim.SetTrigger("Open");

        yield return new WaitForSeconds(openDuration);

        player.transform.position = targetDoor.exitPoint.position;

        targetDoor.anim.SetTrigger("Open");

        yield return new WaitForSeconds(0.15f);

        player.ReturnToIdle();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
            player = other.GetComponent<PlayerController>();
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
