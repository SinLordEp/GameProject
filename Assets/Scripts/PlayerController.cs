using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool facingRight = true;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;
    private float checkRadius = 0.1f;

    public int lives;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetBool("jumping",false);
    }

    void Update()
    {   
        groundAndJump();
        float move = Input.GetAxis("Horizontal");
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        if(move != 0){
            animator.SetBool("moving", true);
        } else{
            animator.SetBool("moving", false);
        }

        if (move > 0 && !facingRight)
        {
            Flip();
        }
        else if (move < 0 && facingRight)
        {
            Flip();
        }
         if (transform.position.y < -10f)
        {
            Die();
        } 
    }

    void groundAndJump(){
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded)
        {   
            animator.SetBool("jumping", true);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        if(isGrounded){
           animator.SetBool("jumping", false); 
        }
        Debug.DrawRay(groundCheck.position, Vector2.down * checkRadius, Color.red);
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
            if(lives > 0){
                animator.SetBool("hit",true);
                lives--;
            }else{
                Die();
            }
        }
            
    }

    void Die()
    {
        Debug.Log("Game Over");
        SceneManager.LoadScene("GameOver");
    }
} 