using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class move : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    private float moveInput;
    private Rigidbody2D rb;
    private bool facingRight = true;
    public bool isGrounded;
    public Transform bounceCheck;
    public Transform groundCheck;
    public GameObject Sapling;
    public GameObject player;
    public float checkRadius;
    public LayerMask whatIsGround;
    public LayerMask whatIsCactus;
    public LayerMask whatIsGoal;
    public LayerMask whatIsDirt;
    public GameObject Tree;
    private int extraJumps;
    public int extraJumpsValue;
    public bool inFuture = true;
    private bool warping = false;
    public float cooldown = 50f;
    private bool hitCactus = false;
    public float bounceRadius = 5f;
    private float collisionCD = 50f;
    private float hitstun = 0f;
    private bool hitGoal = false;
    private bool dead = false;
    private float deathTimer = 70f;
    private int seeds = 0;
    public bool onDirt = false;
    private Animator animator;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        hitstun -= 1f;
        collisionCD -= 1f;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        onDirt = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsDirt);
        hitCactus = Physics2D.OverlapCircle(bounceCheck.position, bounceRadius, whatIsCactus);
        hitGoal = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGoal);
        // Changes the scene forward one if the player hits a goal object
        if (hitGoal)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        //Allows moving only if the player is not time travelling
        if (!warping && !dead)
        {
            moveInput = Input.GetAxis("Horizontal");
            rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        }
        else
        //Locks the players movement and velocity and moves them slightly upwards when they time travel
        {
            rb.velocity = new Vector2(0, 1);
            cooldown -= 1f;
        }
        // Time travel
        if (cooldown < 0)
        {
            cooldown = 50f;
            warping = false;
            if (inFuture == true)
            {
                transform.position = new Vector2(rb.position.x, rb.position.y - 50f);
                inFuture = false;
            }
            else
            {
                transform.position = new Vector2(rb.position.x, rb.position.y + 50f);
                inFuture = true;
            }
        }
        // Flips the player based on the direction they are facing
        if (facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if (facingRight == true && moveInput < 0)
        {
            Flip();
        }
        // If the player collides with a cactus, applies a large force propelling them away
        if (hitCactus && collisionCD < 0f)
        {
            collisionCD = 30f;
            hitstun = 30f;
            Debug.Log("Hit a cactus");
        }
        if (hitstun > 0f)
        {
            rb.AddForce(new Vector2(rb.velocity.x * -2, 1), ForceMode2D.Impulse);
            hitstun -= 1f;
        }
        // Resets the scene if the player is dead
        if (dead)
        {
            deathTimer -= 1f;
        }
        if (deathTimer <= 0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if(isGrounded && rb.velocity.x != 0)
        {
            animator.SetBool("IsRunning", true);
            Debug.Log("Running");
        } else
        {
            animator.SetBool("IsRunning", false);
        }
        if(!isGrounded && rb.velocity.y > 0)
        {
            Debug.Log("GoingUP");
            animator.SetBool("GoingUp", true);
            animator.SetBool("IsRunning", false);
        } else
        {
            animator.SetBool("GoingUp", false);
        }
        if (!isGrounded && rb.velocity.y < 0)
        {
            Debug.Log("Goingdown");
            animator.SetBool("GoingDown", true);
            animator.SetBool("IsRunning", false);
        }
        else
        {
            animator.SetBool("GoingDown", false);
        }
    }
    void Update()
    {

        if (isGrounded == true)
        {
            if (onDirt)
            {
                if (Input.GetKeyDown(KeyCode.K) && seeds > 0)
                {
                    Debug.Log("Planted Tree");
                    seeds--;
                    GameObject a = Instantiate(Sapling) as GameObject;
                    a.transform.position = new Vector2(rb.position.x, rb.position.y);
                    GameObject b = Instantiate(Tree) as GameObject;
                    b.transform.position = new Vector2(rb.position.x, rb.position.y + 52);
                }
            }
            extraJumps = extraJumpsValue;
            if (Input.GetKeyDown(KeyCode.J) && inFuture == true)
            {
                warping = true;
            }
            else if (Input.GetKeyDown(KeyCode.J) && inFuture == false && warping == false)
            {
                warping = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) && extraJumps > 0)
        {
            rb.velocity = Vector2.up * jumpForce;
            extraJumps--;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && extraJumps == 0 && isGrounded == true)
        {
            rb.velocity = Vector2.up * jumpForce;
        }
    }
    void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }
    // Makes the player dead if the player comes into contact with a kill block
    void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Collided");
        if (col.tag == "KillBlock")
        {
            Debug.Log("HitKillBlock");
            dead = true;
        }
        if (col.tag == "SeedBag")
        {
            seeds += 1;
            Debug.Log("GotASeed");
            Destroy(col.gameObject);
            Debug.Log(seeds);

        }
    }
}