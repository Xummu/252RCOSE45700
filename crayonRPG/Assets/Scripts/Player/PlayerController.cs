using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; //이동속도
    public float jumpForce = 12f;  //점프력

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isGrounded = false; //바닥에 있는지
    public Transform groundCheck; //발 접지 검증
    public float groundCheckRadius = 0.1f; //검증범위
    public LayerMask groundLayer;


    //fireball
    public GameObject fireballPrefab;
    public Transform firePoint;
    public Vector2 firePointOffset = new Vector2(0.3f, 0f);
    public float skillCooldown = 1.5f;
    private float lastSkillTime = -999f;

    //damage
    public Transform attackPoint;
    public float attackRange = 0.4f;
    public LayerMask enemyLayer;


    private float inputX; //   기록입력


    private bool isInLadderZone = false;
    private bool isClimbing = false;
    public float climbSpeed = 3f;
    private float ladderCenterX;


    public bool hasKey = false;


    public bool isInvincible = false;
    public float invincibleDuration = 0.5f;


    //Audio
 
   
    public AudioSource audioSource;
    public AudioClip sfxwalk;
    public AudioClip sfxJump;
    public AudioClip sfxAttack;
    public AudioClip sfxHurt;
    public AudioClip sfxGetItem;

    private float stepTimer = 0f;
    public float stepInterval = 0.2f;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {


        //climbing
        float v = Keyboard.current.upArrowKey.isPressed ? 1 :
          Keyboard.current.downArrowKey.isPressed ? -1 : 0;

        if (isInLadderZone && !isClimbing)
        {
            if (v > 0)
            {
                isClimbing = true;
                rb.gravityScale = 0;

                transform.position = new Vector3(
                    ladderCenterX,
                    transform.position.y,
                    transform.position.z
                    );
            }
        }

        if (isClimbing)
        {
            rb.linearVelocity = new Vector2(0, v * climbSpeed);

            anim.SetBool("Climbing", true);

            if (v != 0)
            {
                anim.speed = 1f;
            }
            else
            {
                anim.speed = 0f;
            }
            return;
        }

        var state = anim.GetCurrentAnimatorStateInfo(0);
        
        //공격상태 이동금지
        if (state.IsName("Attack"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            inputX = 0;
            return;//이동금지
        }
        
        //스킬상태 이동금지
        if(state.IsName("Skill"))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            inputX = 0;
            return;//이동금지

        }
        //방향키 
        float x = 0f;
        if (Keyboard.current.leftArrowKey.isPressed)
            x = -1f;
            
        else if (Keyboard.current.rightArrowKey.isPressed)
            x = 1f;


        inputX = x;

        //반전 방향
        if (inputX != 0)
            sr.flipX = inputX < 0;

        // animation parameter ;speed
        anim.SetFloat("Speed", Mathf.Abs(inputX));


        //Jump
        if(Keyboard.current.cKey.wasPressedThisFrame&&isGrounded)
        {
            anim.SetTrigger("Jump");
            audioSource.PlayOneShot(sfxJump);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            
        }

        //공격
        if(Keyboard.current.xKey.wasPressedThisFrame)
        {
            anim.SetTrigger("Attack");
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            inputX = 0;
            audioSource.PlayOneShot(sfxAttack);

}
        
        //스킬
        if(Keyboard.current.spaceKey.wasPressedThisFrame&&CanCastSkill())
        {
            
            anim.SetTrigger("Skill");

            lastSkillTime = Time.time;

            Invoke(nameof(CastFireball), 0.15f);
        }

        //fireball point
        firePoint.localPosition = new Vector3(
            sr.flipX ? -firePointOffset.x : firePointOffset.x,
            firePointOffset.y,
            0

         );

        //Attack point
        var pos = attackPoint.localPosition;
        
        pos.x = Mathf.Abs(pos.x)*(sr.flipX ? -1 : 1);
        attackPoint.localPosition = pos;

        bool moving = Mathf.Abs(inputX) > 0.1f && isGrounded;

        if (moving)
        {
            stepTimer += Time.deltaTime;

            if(stepTimer>=stepInterval)
            {
                stepTimer = 0f;
                audioSource.PlayOneShot(sfxwalk);
            }
        }
        else
        {
            stepTimer = 0f;
        }
          


    }

    void FixedUpdate()
    {
        if (isClimbing) return;
        //이동
        rb.linearVelocity = new Vector2(inputX * moveSpeed,rb.linearVelocity.y);

        //(Raycast or OverLapCircle)
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius,groundLayer);

        anim.SetBool("IsGrounded", isGrounded);

       


    }

    private bool CanCastSkill()
    {
        bool notMoving = Mathf.Abs(inputX) < 0.01f;
        bool isReady = (Time.time - lastSkillTime) >= skillCooldown;

        return isGrounded &&
            notMoving &&
            isReady&&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Jump") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("Skill");
    }
    public void TakeDamage(int dmg)
    {
        if (isInvincible) return;
        audioSource.PlayOneShot(sfxHurt);
        StartCoroutine(InvincibleFlash());
    }

    private IEnumerator InvincibleFlash()
    {
        isInvincible = true;

        float elapsed = 0f;
        float flashSpeed = 0.1f;

        while(elapsed<invincibleDuration)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(flashSpeed);

            sr.enabled = true;
            yield return new WaitForSeconds(flashSpeed);

            elapsed += flashSpeed * 2;
        }

        sr.enabled = true;
        isInvincible = false;

    }

    void DoAttack()
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            attackRange,
            enemyLayer
            );

        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Enemy>().TakeDamage(1);
        }
    }

    private void CastFireball()
    {
        int dir = sr.flipX ? -1 : 1;
        GameObject fb = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        fb.GetComponent<Fireball>().SetDirection(dir);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Ladder"))
        {
            isInLadderZone = true;

            Transform center = other.transform.Find("centerPoint");
            if (center != null)
            {
                ladderCenterX = center.position.x;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Ladder"))
        {
            isInLadderZone = false;
            isClimbing = false;

            rb.gravityScale = 1;
            anim.SetBool("Climbing", false);
            anim.speed = 1f;
        }
    }

    public void StopMoving()
    {
        rb.linearVelocity = Vector2.zero;
        inputX = 0;
        anim.SetFloat("Speed", 0);
    }
    
    public void FaceToDoor()
    {
        anim.SetTrigger("FaceDoor");
    }
    public void ReturnToIdle()
    {
        anim.SetTrigger("Idle");
    }

    public void AddKey()
    {
        hasKey = true;
        audioSource.PlayOneShot(sfxGetItem);
        Debug.Log("获得钥匙！");
    }

}
