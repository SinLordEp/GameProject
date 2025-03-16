using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float knockbackForce = 10f;
    public float knockbackTime = 1;
    public float skillSpawnHeight;
    public float skillDelay;
    private bool isKnockback = false;
    private Rigidbody2D rb;
    private bool facingRight = true;
    public Animator animator;
    public LayerMask groundLayer;
    private enum State {Idle,Moving,Charging};
    private State currentState = State.Idle;
    private bool isCharging = false;
    private bool isAttacking = false;
    private bool isDead = false;
    public GameObject Skill;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {   
        if(!isKnockback && !isDead)
        {
            HandleInput();
            HorizontalMoving();
        }
        if (transform.position.y < -30f)
        {
            Die();
        } 
    }

    private void HandleInput()
    {
        if (!isCharging && !isAttacking && Input.GetKeyDown(KeyCode.Space))
        {
            ChangeState(State.Charging);
            isCharging = true;
        }else if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            isAttacking = true;
            isCharging = false;
            animator.SetTrigger("Attack");
            Invoke("CastSkill", skillDelay);
            Invoke("ResetAttackState", skillDelay+0.2f);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
    }

    void ResetAttackState()
    {
        isAttacking = false;
        ChangeState(State.Idle);
    }

    bool isTouchingWall()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        Vector2 bottom = transform.position;
        Vector2 middle = bottom + new Vector2(0, 0.5f);
        Vector2 top = bottom + new Vector2(0, 1f);
        RaycastHit2D hitTop = Physics2D.Raycast(top, direction, 0.6f, groundLayer);
        RaycastHit2D hitMiddle = Physics2D.Raycast(middle, direction, 0.6f, groundLayer);
        RaycastHit2D hitBottom = Physics2D.Raycast(bottom, direction, 0.6f, groundLayer);
        return hitTop.collider != null || hitMiddle.collider != null;

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
        if(isCharging || isAttacking) return;
        float move = Input.GetAxis("Horizontal");
        if (isTouchingWall() && move != 0)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);
        }
        if (Mathf.Abs(move) > 0)
        {
            ChangeState(State.Moving);
        }
        else if (!isCharging)
        {
            ChangeState(State.Idle);
        }
        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }

    }
    void Jump()
    {
        if(isGrounded())
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

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isKnockback && collision.CompareTag("Spikes")){
            ReceiveDamage(30);
            isKnockback = true;
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockbackDirection));
        }

        if(!isKnockback && collision.CompareTag("LightAttack")){
            ReceiveDamage(20);
            isKnockback = true;
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            InteruptByHurt();
            StartCoroutine(Knockback(knockbackDirection));  
        }
        
        if (!isKnockback && collision.CompareTag("HeavyAttack"))
        {
            ReceiveDamage(35);
            isKnockback = true;
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            InteruptByHurt();
            StartCoroutine(Knockback(knockbackDirection));  
        }
    }

    void InteruptByHurt()
    {
        isCharging = false;
        isAttacking =false;
        ChangeState(State.Idle);
    }
    void CastSkill()
    {
        float direction = Mathf.Sign(transform.localScale.x);
        Vector2 spawnPosition = (Vector2)transform.position + new Vector2(direction * 0.6f, skillSpawnHeight);
        GameObject skillEffect = Instantiate(Skill, spawnPosition, Quaternion.identity);
        skillEffect.transform.localScale = new Vector3(direction, 1, 1);
    }
    void ReceiveDamage(int damage)
    {
        if(GameManager.Instance.ReceiveDamage(damage)){
                animator.SetTrigger("Hurt");
            }else{
                gameObject.tag = "Dead";
                gameObject.layer = 7;
                isDead = true;
                animator.SetTrigger("Dead");              
                Invoke("Die", 3f);
            }
    }

    private IEnumerator Knockback(Vector2 direction)
    {
        rb.linearVelocity = direction * knockbackForce;
        yield return new WaitForSeconds(knockbackTime);
        isKnockback = false;
    }

    bool isGrounded()
    {
        Vector2 position = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 0.2f, groundLayer);
        return hit.collider != null;
    }
    void Die()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOver");
    }
} 