using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    // [REVISION] Fields should probably be sorted, but I'm not sure what the best way is to do it and I don't want to break stuff messing around
    public float speed;
    public float jumpForce;
    private float moveInput;
    private bool facingRight = true;
    public bool isGrounded;
    public Transform bounceCheck;
    public Transform groundCheck;
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
    public bool warping = false;
    public float cooldown = 50f;
    private bool hitCactus = false;
    public float bounceRadius = 5f;
    private float collisionCD = 50f;
    private float hitstun = 0f;
    private bool hitGoal = false;
    private bool dead = false;
    private float deathTimer = 70f;

    public GameObject Sapling;
    private int seeds = 0;
    public bool onDirt = false;

    void Start()
    {
        // Initialize Rigidbody and Animator
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        hitstun--;
        collisionCD--;

        CheckCurrentCollisions();
        CheckForGoal(hitGoal);
        // CheckForMovementLock() contains MoveHorizontally() and TimeTravelStartup()
        CheckForMovementLock();
        TimeTravel();
        Flip();
        CactusCollision();
        Death();
        ChangeAnimationState();
    }

    // [REVISION][PlantTree()] I think there should be a method in Start to check for dirt/seeds in the scene before continuously checking in Update
    void Update()
    {
        if (isGrounded == true)
        {
            PlantTree();
            extraJumps = extraJumpsValue;
            CheckForTimeTravelCommand();
        }
        Jump();
    }

    // If the player collides with a cactus, applies a large force propelling them away
    private void CactusCollision()
    {
        if (hitCactus && collisionCD < 0f)
        {
            collisionCD = 30f;
            hitstun = 30f;
            Debug.Log("Hit a cactus");
        }
        if (hitstun > 0f)
        {
            rb.AddForce(new Vector2(rb.velocity.x * -2, 1), ForceMode2D.Impulse);
            hitstun--;
        }
    }

    // Change the player's animation based on their movement
    private void ChangeAnimationState()
    {
        if (isGrounded && rb.velocity.x != 0)
        {
            animator.SetBool("IsRunning", true);
            Debug.Log("Running");
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }
        if (!isGrounded && rb.velocity.y > 0)
        {
            Debug.Log("GoingUP");
            animator.SetBool("GoingUp", true);
            animator.SetBool("IsRunning", false);
        }
        else
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

    // Checks what the player is interacting with on a given frame
    // [REVISION] I think custom tags can be used for collision checking instead
    private void CheckCurrentCollisions()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        onDirt = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsDirt);
        hitCactus = Physics2D.OverlapCircle(bounceCheck.position, bounceRadius, whatIsCactus);
        hitGoal = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGoal);
    }

    // Changes the scene forward one if the player hits a goal object
    private void CheckForGoal(bool var)
    {
        if (var)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // Allows moving only if the player is not time travelling
    private void CheckForMovementLock()
    {
        if (!warping && !dead)
        {
            MoveHorizontally();
        }
        else
        {
            TimeTravelStartup();
        }
    }

    // Checks for when the player attempts to time travel
    private void CheckForTimeTravelCommand()
    {
        if (Input.GetKeyDown(KeyCode.J) && inFuture == true)
        {
            warping = true;
        }
        else if (Input.GetKeyDown(KeyCode.J) && inFuture == false && warping == false)
        {
            warping = true;
        }
    }

    // Resets the scene if the player is dead
    private void Death()
    {
        if (dead)
        {
            deathTimer--;
        }
        if (deathTimer <= 0f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    // Changes the player's direction based on keyboard input and current direction
    private void Flip()
    {
        if ((facingRight == false && moveInput > 0) || (facingRight == true && moveInput < 0))
        {
            facingRight = !facingRight;
            Vector3 Scaler = transform.localScale;
            Scaler.x *= -1;
            transform.localScale = Scaler;
        }
    }

    // Makes the player jump while grounded or if they are not grounded and have extra jumps
    private void Jump()
    {
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

    // Moves the player in their current direction
    private void MoveHorizontally()
    {
        moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }

    // Makes the player dead if the player comes into contact with a kill block
    private void OnTriggerEnter2D(Collider2D col)
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

    // If the player is on dirt and presses a button while they have at least one seed, a sapling is made in the past and a tree is made in the future
    private void PlantTree()
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
    }

    // If the cooldown has run out, starts a new cooldown and sends the player to a past zone or a future zone
    private void TimeTravel()
    {
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
    }

    //Locks the players movement and velocity and moves them slightly upwards when they time travel
    private void TimeTravelStartup()
    {
        rb.velocity = new Vector2(0, 1);
        cooldown--;
    }
}