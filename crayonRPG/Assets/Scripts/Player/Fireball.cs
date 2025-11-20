using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 2f;
    private int direction = 1;

    private Animator anim;
    private bool exploded = false;
    private Vector3 startPos;

    public AudioSource audioSource;
    public AudioClip sfxfly;
    public AudioClip sfxexplo;

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
       
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;

        if(sfxfly!=null)
        {
            audioSource.PlayOneShot(sfxfly);
        }
        
        Destroy(gameObject, lifeTime+0.5f);
    }
    public void SetDirection(int dir)
    { 
        direction = dir;

        if (dir == -1)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }
    private void Explode()
    {
        exploded = true;

        speed = 0f;

        if (sfxfly != null)
        {
            audioSource.PlayOneShot(sfxexplo);
        }

        anim.SetTrigger("Impact");
       

        Destroy(gameObject, 0.35f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (exploded) return;

        if(other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(2);
        }

        if(other.CompareTag("Boss"))
        {
            other.GetComponent<TVBoss>().TakeDamage(2);
        }

       

        Explode();


    }

    // Update is called once per frame
    void Update()
    {
        if(!exploded)
        {
            transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
        }
        if(!exploded&&Vector3.Distance(startPos,transform.position)>3f)
        {
            Explode();
        }
    }
   

   

}
