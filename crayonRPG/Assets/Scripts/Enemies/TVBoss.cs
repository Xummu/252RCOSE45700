using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class TVBoss : MonoBehaviour
{
    public int maxHP = 10;
    private int currentHP;

    private Animator anim;
    private bool isDead = false;

    public AudioSource audioSource;
    public AudioClip sfxboom;

    public string targetScene;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        
    }
    private void Die()
    {
        isDead = true;
        anim.SetTrigger("Boom");
        audioSource.PlayOneShot(sfxboom);
        Destroy(gameObject, 4f);
        StartCoroutine(GoToScene());
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHP -= dmg;

        if(currentHP<=0)
        {
            Die();
        }
        
    }

    IEnumerator GoToScene()
    {
      

        yield return new WaitForSeconds(0.5f);

        SceneManager.LoadScene(targetScene);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
