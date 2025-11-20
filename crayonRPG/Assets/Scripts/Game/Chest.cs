using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour
{
    public GameObject closeChest;
    public GameObject openChest;
    public GameObject keyObject;

    private bool playerInRange = false;
    private bool opened = false;

    public AudioSource audioSource;
    public AudioClip sfxChestOpen;
    public AudioClip sfxKeySpawn;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!opened&&playerInRange)
        {
            if(Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                OpenChest();
            }
        }
    }
    void OpenChest()
    {
        opened = true;

        closeChest.SetActive(false);
        openChest.SetActive(true);

        audioSource.PlayOneShot(sfxChestOpen);

        keyObject.SetActive(true);
        audioSource.PlayOneShot(sfxKeySpawn);
        keyObject.GetComponent<KeyItem>().StartFloating();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
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
