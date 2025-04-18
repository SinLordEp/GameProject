using UnityEngine;
using System.Collections;
public class EnemyControl : MonoBehaviour
{
    public int health = 100;
    public bool patrol = false;
    public int chaseTime = 600;
    public float moveSpeed = 5f;
    public float jumpForce = 6f;
    public float knockbackForce = 10f;
    public float knockbackTime = 1;
    public float chaseRange = 10f;
    public float attackRange = 1.5f;
    public GameObject attackHitbox;
    public Animator animator;
    public LayerMask groundLayer;
    public LayerMask playerLayer;
    public LayerMask wallLayer;
    public AudioSource audioSource;
    public AudioClip hitByStone;
    public AudioClip hitByFire;
    public AudioClip hitByLight;
    private bool isKnockback = false;
    private Rigidbody2D rb;
    private bool facingRight = false;
    private enum State {Idle,Moving,Charging};
    private State currentState = State.Idle;
    private bool isDead = false;
    private bool isAttacking = false; 
    private bool isWaiting = false;
    private int chaseCountdown;
    private GameObject player;
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
        if(!isAttacking && isLayerDetected(playerLayer, attackRange))
        {
            Attack();
        }
        if(isLayerDetected(playerLayer, chaseRange))
        {
            chaseCountdown = chaseTime;
            player = GameObject.FindGameObjectWithTag("Player");
        }else{
            chaseCountdown -= 1;
        }
        if(chaseCountdown <= 0){
            ChangeState(State.Idle);
            return;
        }
        if(!isAttacking)
        {
            ChasePlayer();
        }

    }

    void ChasePlayer()
    {
        float distance = Mathf.Sign(player.transform.position.x - transform.position.x);
        if ((distance > 0 && !facingRight) || (distance < 0 && facingRight))
        {
            Flip();
        }
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        if(isLayerDetected(wallLayer, 1f))
        {
            ChangeState(State.Idle);
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        if(isLayerDetected(groundLayer, 0.6f))
        {
            Jump();
        }else{
            HorizontalMoving(direction);
        }
    
    }

    void Patrol()
    {
        if(!isAttacking && isLayerDetected(playerLayer, attackRange))
        {
            Attack();
        }
        else if(isLayerDetected(wallLayer, 0.6f))
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            Flip();
            ChangeState(State.Idle);
            isWaiting = true;
            Invoke("StopWaiting",2f);
        }
        else if(isLayerDetected(groundLayer, 0.6f))
        {
            Jump();
        }
        else if(!isAttacking && !isWaiting)
        {
            Vector2 direction = facingRight ? Vector2.right : Vector2.left;
            HorizontalMoving(direction);
        }
    }
    void StopWaiting()
    {
        isWaiting = false;
    }
    bool isLayerDetected(LayerMask layer, float detectRange)
    {
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        Vector2 bottom = transform.position;
        float height = GetComponent<Collider2D>().bounds.size.y;
        Vector2 middle = bottom + new Vector2(0, height * 0.5f);
        Vector2 top = bottom + new Vector2(0, height);
        bottom += new Vector2(0, 0.2f);
        RaycastHit2D hitTop = Physics2D.Raycast(top, direction, detectRange, layer);
        RaycastHit2D hitMiddle = Physics2D.Raycast(middle, direction, detectRange, layer);
        RaycastHit2D hitBottom = Physics2D.Raycast(bottom, direction, detectRange, layer);
        return hitTop.collider != null || hitMiddle.collider != null || hitBottom.collider != null;
    }

    void Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");
        Invoke("CreateAttackHitbox",0.5f);
        Invoke("StopAttacking", 1f);
    }

    void StopAttacking()
    {
        isAttacking = false;
        ChangeState(State.Idle);
    }

    void CreateAttackHitbox()
    {
        if(isKnockback) return;
        Vector2 direction = facingRight ? Vector2.right : Vector2.left;
        rb.linearVelocity = new Vector2( direction.x * moveSpeed*2f, rb.linearVelocity.y);
        GameObject hitbox = Instantiate(attackHitbox, transform.position, Quaternion.identity, transform);
        hitbox.transform.localPosition = new Vector3(-0.4f, 1, 0);
        StartCoroutine(DestroyHitbox(hitbox));
    }
    IEnumerator DestroyHitbox( GameObject hitbox)
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(hitbox);
    }
    private void ChangeState(State newState)
    {
        if (currentState != newState)
        {
            currentState = newState;
            animator.SetInteger("State", (int)currentState);
        }
    }
    void HorizontalMoving(Vector2 direction)
    {
        rb.linearVelocity = new Vector2( direction.x * moveSpeed, rb.linearVelocity.y);
        ChangeState(State.Moving);
    }
    void Jump()
    {
        if(isGrounded())
        {
            animator.SetTrigger("Jump");
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
        if(!isKnockback && !isDead && collision.CompareTag("StoneSkill")){
            audioSource.PlayOneShot(hitByStone);
            isKnockback = true;
            ReceiveDamage(40);
            Vector2 knockbackDirection = Vector2.up; 
            StartCoroutine(Knockback(knockbackDirection, 1f));
        }else if(!isKnockback && !isDead && collision.CompareTag("FireSkill"))
        {
            audioSource.PlayOneShot(hitByFire);
            isKnockback = true;
            ReceiveDamage(50);
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockbackDirection, 1f));
        }else if(!isKnockback && !isDead && collision.CompareTag("LightSkill"))
        {
            audioSource.PlayOneShot(hitByLight);
            isKnockback = true;
            ReceiveDamage(10);
            Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
            StartCoroutine(Knockback(knockbackDirection, 0.1f));
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
        yield return new WaitForSeconds(knockbackTime*knockbackRate);
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
        isDead = true;
        if(gameObject.tag == "HeavyEnemy")
        {
            GameManager.Instance.RegainHP(40);
        }
        if(gameObject.tag == "LightEnemy"){
            GameManager.Instance.RegainHP(20);
        }
        gameObject.tag = "Dead";
        gameObject.layer = 7;  
        animator.SetTrigger("Dead");
        Destroy(gameObject, 3f);
    }
}
