# Hesitative Frog 2D Platformer

2D platformer game for Windows OS

The game have been created for educational and entertainment purposes to get some practice.

Learned and mastered:
* Defferent possibilities of the "Unity" environment and how to use it to create 2D games
* Basic C# methods and principles
* Creation of mathematical regularities for moving objects using C# language
* Debugging, testing, building project
* OOP principles

<img src="https://i.postimg.cc/wjNV3V4P/preview-img.jpg" width="800">

#### The following steps have been done to complete the game creation:
* to add a start and end game screen to our game by creating scenes with a canvas and buttons.
* to create a Tile Palette in Unity and start drawing our Tilemap.
* to continue writing the player movement code using the Unity Input Manager together with a Rigidbody2D and velocity.
* to set up a running animation for our player using a boolean Animator parameter. 
* to add more animations to our animation state machine and toggle between them using an enum.
* to take care that our player can only jump while he is standing on the ground.
* to add collectible items to our game and display a counter in the UI.
* to let the player die when it collides with a trap and restart the level using the SceneManager.
* to create a moving platform that follows an array of waypoints and can transport our player. 
* to combine our waypoint follower script with a new rotation script to create a new moving saw trap.
* to add sounds and music to our game using the Audio Source component.
* to add more levels to our game by creating additional scenes.
* to add a start and end game screen to our game by creating scenes with a canvas and buttons.

## Features

* Smooth and precise movement
* Allows for jumps
* Easily add animations
* One way platforms
* Moving platforms
* Collect objects
* Static and moving traps 
* Finish checkpoint system

## Planned Features

* Ladders/Ropes
* Crumbling platforms
* Checkpoint system
* Environment hazards
* Pressure plates/levers/other triggers
* Ledge grabs
* Running (dash)
* Ducking/Crawling
* Basic combat templates

# Rules and game overview:

## Goal 

Achieve the finish flag of the level and collect as many kiwis (points) as possible.

## Game mechanics

The "hesitated frog" game screen contains the player object (frog) and by now static and moving traps (you will die immidiately if you touch them). 
The difficulty increases slightly with each next level.
<img src="https://i.postimg.cc/BbSd5vwr/Screenshot-6.png" width="600">

## The start of the game  

To start the game you have to click the "start" button. 

<img src="https://i.postimg.cc/XqxXxpNd/Screenshot-8.png" width="600">

## The end game conditions

By now, the player has only one life. It means if the player collides with a trap, the current level will be restarted.
To end the game you have to complete all levels. 
NOTE: you have to click the "quit" button to exit the game.

<img src="https://i.postimg.cc/KzG2fQ1p/Screenshot-7.png" width="600">


# Project structure

The project structure includes the files required to compile and run the game. 
Thus, "Scripts" folder contains all С# classes for the implementation of verious events in the game.

### Camera controller

The method is implemented in the class of "CameraController" script. The update function which is responsible for positioning of a camera is shown below:

```
public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform player;
    private void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
    }
}
```
NOTE: [SerializeField] is used to allow us change the chosen parameter directly in the Unity UI.

### Player movement

This player movement code using the Unity Input Manager together with a Rigidbody2D and velocity; setting up a running animation for our player using a boolean Animator parameter; adding more animations to our animation state machine and toggle between them using an enum; taking care that our player can only jump while he is standing on the ground. Here, I left some comments to give you better idea about what is going on here.
```
public class PlayerMovement : MonoBehaviour
{
    // storing variables
    private Rigidbody2D rb; //rigid body variable
    private BoxCollider2D coll; 
    private SpriteRenderer sprite;
    private Animator anim;

    [SerializeField] private LayerMask jumpableGround;


    private float dirX = 0f;
    [SerializeField] private float moveSpeed = 7f; // [SerializeField] - to see this var in Unity
    [SerializeField] private float jumpForce = 14f;

    private enum MovementState { idle, running, jumping, falling } // create own data type. in unity converted to int values..

    [SerializeField] private AudioSource jumpSourceEffect;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal"); // press right +1 press left -1. we back to 0 immidiately
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y); //detect speed and direction a frame before. rb.velocity.y

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpSourceEffect.Play();
            rb.velocity = new Vector2(rb.velocity.x, jumpForce); // x, y 
        }

        UpdateAnimationState();
    }
    
    private void UpdateAnimationState()
    {
        MovementState state;

        if (dirX > 0f)
        {
            // anim.SetBool("running", true);
            state = MovementState.running;
            sprite.flipX = false;
        }
        else if (dirX < 0f)
        {
            // anim.SetBool("running", true);
            state = MovementState.running;
            sprite.flipX = true;
        }
        else 
        {
            // anim.SetBool("running", false);
            state = MovementState.idle;
        }

        // if check for jumping animation. we don't want to see running animation in the air.

        if (rb.velocity.y > .1f) // value is never exactly equals 0 even if we are staying... we user very small value.. 
        {
            state = MovementState.jumping;
        }
        // if check we might falling.
        else if (rb.velocity.y < -.1f) // downward force applied
        {
            state = MovementState.falling;
        }


        anim.SetInteger("state", (int)state); // turns enum to int representation
    }

    // method checks if we staying on the ground or not
    private bool IsGrounded()
    {
        // create a box around player that has same shape as box collider 
        // 0f- rotation (0 rotation)
        // Vector2.down, .1f moves the box a little bit down
        // checks if we overlaping jumpableGround true / false
        return Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0f, Vector2.down, .1f, jumpableGround);
    }
}
```

### Item Collector

Here, we add collectible items to our game and display a counter in the UI.
```
public class ItemCollector : MonoBehaviour
{
    private int kiwis = 0;

    [SerializeField] private Text kiwisText;
    [SerializeField] private AudioSource collectionSoundEffect;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Kiwi"))
        {
            collectionSoundEffect.Play();
            Destroy(collision.gameObject); // Destroy kiwi
            kiwis++;
            kiwisText.text = "Kiwis: " + kiwis;
        }
    }
}

```
### Player Life

In the following class "PlayerLife" we need to perform the player's death when it collides with a trap and restart the level using the SceneManager.
```
public class PlayerLife : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [SerializeField] private AudioSource deathSoundEffect;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
    }

    private void Die()
    {
        deathSoundEffect.Play();
        rb.bodyType = RigidbodyType2D.Static;
        anim.SetTrigger("death");
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}

```

### Sticky Platform

Here, we are checking when when we touch the trigger (platform collider 2D). We did it to avoid bugs and glitches, when player is stuck in a platform corners...

```
public class StickyPlatform : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collision.gameObject.transform.SetParent(null);
        }
    }

}
```

### Finish

Finally, we are checking if the player reaches the finish flag. Using the Collider2D collision here and taking care of the relevant sound effects with the help of <AudioSource> component.
```
public class Finish : MonoBehaviour
{
    private AudioSource finishSound;

    private bool levelCompleted = false;

    private void Start()
    {
        finishSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !levelCompleted)
        {
            finishSound.Play();
            levelCompleted = true;
            Invoke("CompleteLevel", 2f);
        }
    }

    private void CompleteLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
}
```

### Other scripts: 
    
    
#### StartMenu
    Allows to enter the game.
```
    public class StartMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
}
```
    
#### EndMenu 
    Allows to quit an application.
```
public class EndMenu : MonoBehaviour
{
    public void Quit()
    {
        Application.Quit();
    }
}
```
#### Rotate
    Defines how many times we will rotate 360deg per second. Special class that has been used for saw rotation effect.
```
    public class Rotate : MonoBehaviour
{
    [SerializeField] private float speed = 2f; 
    private void Update()
    {
        transform.Rotate(0, 0, 360 * speed * Time.deltaTime);
    }
}

```
    
#### WaypointFollower
    Special class that has been used for checking if cuurent waypoint and platform has a distance of .1f we know we touching it (used indexes of the waypoints). This script allows to move saw object from one waypoint to another with the specified speed (2 game units in my case).
```
    public class WaypointFollower : MonoBehaviour
{

    [SerializeField] private GameObject[] waypoints;
    private int currentWaypointIndex = 0; // 1st index for 1st waypoint

    [SerializeField] private float speed = 2f;
    private void Update()
    {
        if (Vector2.Distance(waypoints[currentWaypointIndex].transform.position, transform.position) < .1f) 
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;
            }
        }
        transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].transform.position, Time.deltaTime * speed); 
        
    }
}
```
