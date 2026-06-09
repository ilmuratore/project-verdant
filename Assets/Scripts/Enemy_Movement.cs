using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    public float speed; 
    public Transform player;
    private Rigidbody2D rb;
    private Animator enemyAnim;
    private SpriteRenderer sr;
    private CircleCollider2D detectionCollider;
    private bool isChasing;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyAnim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        detectionCollider = GetComponent<CircleCollider2D>();

    }

    void FixedUpdate()
    {
        if (isChasing == true)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;
            enemyAnim.SetBool("IsChasing", true);
            if (direction.x > 0.1f) 
            {
                sr.flipX = false;
                detectionCollider.offset = new Vector2(Mathf.Abs(detectionCollider.offset.x), detectionCollider.offset.y);
            }
            else if (direction.x < -0.1f)
            {
                sr.flipX = true;
                detectionCollider.offset = new Vector2(-Mathf.Abs(detectionCollider.offset.x), detectionCollider.offset.y);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            if(player == null)
            {
                player = collision.transform;
            }
            isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "Player")
        {
            rb.linearVelocity = Vector2.zero;
            isChasing = false;
            enemyAnim.SetBool("IsChasing", false);
            
        }
    }
}
