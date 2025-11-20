using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;


public class DoorNextScene : MonoBehaviour
{

    public string targetScene;
    public Animator anim;

    private PlayerController player;
    private bool playerInRange = false;

    public AudioSource audioSource;
    public AudioClip sfxOpen;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange && Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            audioSource.PlayOneShot(sfxOpen);
            StartCoroutine(GoToScene());
        }
    }
    IEnumerator GoToScene()
    {
        player.StopMoving();
        player.FaceToDoor();

        anim.SetTrigger("Open");
        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(targetScene);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            player = other.GetComponent<PlayerController>();
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
