using UnityEngine;
using System.Collections;
public class EnemyControl : MonoBehaviour
{
    public int health = 100;
    public bool patrol = false;
    public float chaseTime = 0;
    public float moveSpeed = -5f;
    public float jumpForce = 6f;
    public float knockbackForce = 10f;
    public float knockbackTime = 1;
    public Animator animator;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask wallLayer;
    private bool isKnockback = false;
    private Rigidbody2D rb;
    private bool facingRight = false;
    private enum State {Idle,Moving,Charging};
    private State currentState = State.Idle;
    private bool isDead = false;
    private bool playerSeen = false;
    private bool isAttacking = false; 
    private bool isWaiting = false;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {   
        if(isDead || isKnockback) return;
        if(patrol){
            Patrol();
        }else{
            Hunt(); 
        }
        if (transform.position.y < -30f)
        {
            Die();
        } 
    }

    void Hunt()
    {

    }

    void Patrol()
    {
        if(isPlayerInRange() && !isAttacking)
        {
            Attack();
        }
        else if(isTouchingWall())
        {
            Flip();
            ChangeState(State.Idle);
            isWaiting = true;
            Invoke("StopWaiting",2f);
        }
        else if(!isAttacking && !isWaiting)
        {
            HorizontalMoving();
        }
    }
    void StopWaiting()
    {
        isWaiting = false;
    }
    void StopAttacking()
    {
        isAttacking = false;
        ChangeState(State.Idle);
    }
    bool isTouchingWall()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        Vector2 bottom = transform.position;
        float height = GetComponent<Collider2D>().bounds.size.y;
        Vector2 middle = bottom + new Vector2(0, height * 0.5f);
        RaycastHit2D hitMiddle = Physics2D.Raycast(middle, direction, 1f, wallLayer);
        //Debug.DrawRay(middle, direction * 0.5f, Color.red);
        return hitMiddle.collider != null;
    }

    bool isPlayerInRange()
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        Vector2 bottom = transform.position;
        float height = GetComponent<Collider2D>().bounds.size.y;
        Vector2 middle = bottom + new Vector2(0, height * 0.5f);
        RaycastHit2D hitMiddle = Physics2D.Raycast(middle, direction, 2f, playerLayer);
        if (hitMiddle.collider != null && hitMiddle.collider.CompareTag("Player"))
        {
            return true;
        }else{
            return false;
        }
    }

    bool isPlayerSeen()
    {

    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        Invoke("StopAttacking",1f);
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
        rb.linearVelocity = new Vector2( moveSpeed, rb.linearVelocity.y);
        ChangeState(State.Moving);
        if(rb.linearVelocity.magnitude == 0) Jump();
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
        moveSpeed = moveSpeed * -1f;
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(!isKnockback && collision.CompareTag("StoneSkill")){
            isKnockback = true;
            ReceiveDamage(30);
            Vector2 knockbackDirection = Vector2.up; 
            StartCoroutine(Knockback(knockbackDirection, 1f));
        }else if(!isKnockback && collision.CompareTag("FireSkill"))
        {
            isKnockback = true;
            ReceiveDamage(40);
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockbackDirection, 1f));
        }else if(!isKnockback && collision.CompareTag("LightSkill"))
        {
            isKnockback = true;
            ReceiveDamage(15);
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockbackDirection, 0.5f));
        }
    }
    void ReceiveDamage(int damage)
    {
        health -= damage;
        if(health > 0)
        {
            animator.SetTrigger("Hurt");
        }
        else
        {
            Die();
        }
    }
    private IEnumerator Knockback(Vector2 direction, float knockbackRate)
    {
        rb.linearVelocity = direction * knockbackForce * knockbackRate;
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
        gameObject.tag = "Dead";
        isDead = true;
        animator.SetTrigger("Dead");
        Destroy(gameObject, 3f);
    }
}
