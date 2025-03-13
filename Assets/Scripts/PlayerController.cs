using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float knockbackForce = 10f;
    public float knockbackTime = 1;
    private bool isKnockback = false;
    private bool isGrounded = true;
    private Rigidbody2D rb;
    private bool facingRight = true;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private float checkRadius = 0.2f;
    private enum State {Idle,Moving,Charging,Hurt};
    private State currentState = State.Idle;
    private bool isCharging = false;

    public GameObject Skill;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {   
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        HandleInput();
        if(!isKnockback)
        {
            HorizontalMoving();
        }
         if (transform.position.y < -30f)
        {
            Die();
        } 
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isCharging = true;
            ChangeState(State.Charging);
        }else if (Input.GetKeyUp(KeyCode.Space))
        {
            animator.SetTrigger("Attack");
            CastSkill();
            isCharging = false;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }

    private void ChangeState(State newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            animator.SetInteger("State", (int)currentState);
        }
    }
    
    void HorizontalMoving()
    {
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        if (Mathf.Abs(move) > 0 && !isCharging)
        {
            ChangeState(State.Moving);
        }
        else if (!isCharging)
        {
            ChangeState(State.Idle);
        }
        if (move > 0 && !facingRight && !isCharging)
        {
            Flip();
        }
        else if (move < 0 && facingRight && !isCharging)
        {
            Flip();
        }

    }
    
    void Jump()
    {
        if (isGrounded)
        {   
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(!isKnockback && collision.gameObject.CompareTag("Enemy")){
            isKnockback = true;
            ReceiveDamage(20);
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockbackDirection));
        }
            
    }

    void CastSkill()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 spawnPosition = (Vector2)transform.position + new Vector2(direction * 0.6f, -0.9f);
        GameObject skillEffect = Instantiate(Skill, spawnPosition, Quaternion.identity);
        skillEffect.transform.localScale = new Vector3(direction, 1, 1);
    }
    void ReceiveDamage(int damage)
    {
        if(GameManager.Instance.ReceiveDamage(damage)){
                animator.SetTrigger("Hurt");
            }else{
                animator.SetTrigger("Dead");
                Die();
            }
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        rb.linearVelocity = direction * knockbackForce;
        yield return new WaitForSeconds(knockbackTime);
        isKnockback = false;
    }

    void Die()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOver");
    }
} 