using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int maxHP = 2;
    private int currentHP;

    private Animator anim;
    private Rigidbody2D rb;

    public float moveSpeed = 1.2f;
    public float walkTime = 2f;
    public float stopTime = 1f;

    private bool isDead = false;
    private float timer = 0f;
    private int direction = -1;
    private enum State { Walk, Stop }
    private State state = State.Walk;

    public AudioSource audioSource;
    public AudioClip sfxStep;


    private float stepTimer = 0f;
    public float stepInterval = 0.2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        timer += Time.deltaTime;

        if (state == State.Walk)
        {
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
            anim.SetFloat("Speed", moveSpeed);

            if (timer >= walkTime)
            {
                timer = 0f;
                state = State.Stop;
                rb.linearVelocity = Vector2.zero;
                anim.SetFloat("Speed", 0f);
            }
        }
        else if (state == State.Stop)
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetFloat("Speed", 0f);

            if (timer >= stopTime)
            {
                timer = 0f;
                state = State.Walk;

                direction *= -1;

                GetComponent<SpriteRenderer>().flipX = direction > 0;
            }
        }



        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        if(isMoving&&!isDead)
        {
            stepTimer += Time.deltaTime;

            if(stepTimer>=stepInterval)
            {
                stepTimer = 0;
                audioSource.PlayOneShot(sfxStep);
            }
        }
        else
        {
            stepTimer = 0;
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHP -= dmg;
        anim.SetTrigger("Hurt");


        if (currentHP <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        anim.SetTrigger("Die");
        anim.SetBool("IsDead", true);

        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;

        Destroy(gameObject, 0.6f);
    }

}
