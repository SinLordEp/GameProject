using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float knockbackForce = 10f;
    private bool isGrounded = true;
    private Rigidbody2D rb;
    private bool facingRight = true;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private float checkRadius = 0.2f;
    private enum State {Idle,Moving,Charging,Attacking,Hurt};
    private State currentState = State.Idle;
    private bool isCharging = false;

    public int lives;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {   
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        HandleInput();
        HorizontalMoving();
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
        if(collision.gameObject.CompareTag("Enemy")){
            ReceiveDamage(20);
            KnockBack(collision);
        }
            
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

    void KnockBack(Collision2D collision)
    {
        Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
        rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    }

    void Die()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOver");
    }
} 