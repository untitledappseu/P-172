# 2D Side Scroller Controller

This is a simple 2D side scroller controller for Unity that allows the player to run left and right, jump, and shoot with a gun that follows the mouse cursor.

## Setup Instructions

1. Create a new GameObject in your scene or use an existing one
2. Add a Rigidbody2D component to your GameObject
   - Set Gravity Scale to 3
   - Set Collision Detection to Continuous
   - Check "Freeze Rotation Z" in Constraints
3. Add a Collider2D component (BoxCollider2D or CapsuleCollider2D recommended)
4. Add the PlayerController script to your GameObject
5. Set up the Ground Check:
   - Create a child GameObject at the bottom of your player (or let the script create one automatically)
   - Assign it to the "Ground Check" field in the inspector
   - Set the Ground Check Radius (default: 0.2)
6. Create a layer for the ground and assign it to your ground objects
7. Assign this layer to the "Ground Layer" field in the inspector

## Gun Setup

1. Create a child GameObject for the gun (e.g., "Gun")
2. Add a SpriteRenderer component with your gun sprite
3. Add the GunController script to the gun GameObject
4. Create a bullet prefab:
   - Create a new GameObject
   - Add a SpriteRenderer with your bullet sprite
   - Add a Rigidbody2D (set to Kinematic if using triggers)
   - Add a Collider2D (set as trigger if needed)
   - Add the Bullet script
   - Set the Collision Layers to determine what the bullet can hit
   - Save as a prefab
5. Assign the bullet prefab to the Gun Controller's "Bullet Prefab" field

## Controller Settings

### Movement Settings

- **Move Speed**: How fast the player moves horizontally (default: 5)
- **Jump Force**: How high the player jumps (default: 10)
- **Fall Multiplier**: Makes falling faster than jumping for better feel (default: 2.5)
- **Low Jump Multiplier**: Makes short jumps feel better (default: 2)

### Ground Check

- **Ground Check**: Transform used to check if player is grounded
- **Ground Check Radius**: Radius of the circle used for ground detection (default: 0.2)
- **Ground Layer**: Layer mask for ground detection

### Gun Settings

- **Fire Point**: Transform where bullets spawn (created automatically if not assigned)
- **Bullet Prefab**: Prefab of the bullet to shoot
- **Bullet Speed**: How fast bullets travel (default: 20)
- **Fire Rate**: Time between shots in seconds (default: 0.2)
- **Bullet Lifetime**: How long bullets exist before being destroyed (default: 2)
- **Flip With Player**: Whether the gun should flip when the player turns (default: true)
- **Gun Offset**: Position offset from the player center (default: 0.5)

## Animation Parameters

The controller sets the following animation parameters if an Animator is attached:

- **IsRunning**: True when the player is moving horizontally
- **IsJumping**: True when the player is moving upward in the air
- **IsFalling**: True when the player is falling

## Controls

- **Horizontal Movement**: Use the "Horizontal" input axis (A/D or Left/Right arrow keys)
- **Jump**: Use the "Jump" button (Space bar by default)
- **Aim**: Move the mouse cursor to aim the gun
- **Shoot**: Left mouse button
