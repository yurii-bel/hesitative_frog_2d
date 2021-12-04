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


The method is implemented in the "CameraController" script. The update function which is responsible for positioning of a camera is shown below:

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
The "increment" variable is used as an argument to the function. When calling the function to increase the current number of points, we indicate the exact value, for example, 10, then 10 will be added to the current value. 

### Implementation of the spaceship control system
#### Touch Control
Here we have the class "TouchMovementControl", which implements the logic of touch control. With the touch control, you can control the object of the player spaceship, pointing your finger across the device screen its motion vector. In the method "Awake()" we we set the direction value to zero. Awake() is called to initialize variables or states before the application start.
```
private void Awake()
    {
        direction = Vector2.zero;
        touched = false;
    }
```
Then methods for handling events are implemented: 

OnPointerDown() is called when user taps on the screen and takes an PointerEventData object as an argument.
```
public void OnPointerDown(PointerEventData eventData)
    {
        if (!touched)
        {
            origin = eventData.position;
            touched = true;
            pointerID = eventData.pointerId;  // get status and click number
        }
    }
```
OnDrag() is called to handle finger movement across the screen and takes an PointerEventData object as an argument.
```
    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.pointerId == pointerID)  // check if it is the same touch
        {
            Vector2 currentPosition = eventData.position;
            Vector2 directionResult = currentPosition - origin;
            direction = directionResult.normalized;
        }
    }
```
OnPointerUp() is called when user's touch was released.
```
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == pointerID)
        {
            direction = Vector2.zero;  // stop moving
        }
    }
```
And finally, Method GetDirection() returns a position of the moved object. Here we use MoveTowards method, that moves a point current towards target. By updating an object’s position each frame using the position calculated by this function, you can move it towards the target smoothly.
```
    public Vector2 GetDirection()
    {
        smoothDirection = Vector2.MoveTowards(smoothDirection, direction, smoothness);
        return smoothDirection;
    }
```
We call this function in "PlayerController" class:
```
    Vector2 direction = touchControl.GetDirection();

    Vector3 movement = new Vector3(direction.x, 0.0f, direction.y);
    rigidbody.velocity = movement * speed;
```

#### Accelerometer Control
First of all, you need to calibrate the accelerometer:
```
    public void CalibrateAccelerometer()
    {
        Vector3 accelerationSnapshot = Input.acceleration;
        Quaternion rotateQuaternion = Quaternion.FromToRotation(new Vector3(0.0f, 0.0f, -1.0f), accelerationSnapshot);
        calibrationQuaternion = Quaternion.Inverse(rotateQuaternion);
    }
```
Then in the method "FixedUpdate" we get the data for Vector3, taken from the accelerometer and using the velocity component and the speed variable, we set the spaceship in movement:
```
    Vector3 accelerationRaw = Input.acceleration;
    Vector3 acceleration = FixedAcceleraton(accelerationRaw);

    Vector3 movement = new Vector3(acceleration.x, 0.0f, acceleration.y);
    rigidbody.velocity = movement * speed;
```
