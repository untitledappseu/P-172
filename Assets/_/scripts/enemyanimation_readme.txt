# Enemy Animation System Setup Guide

This guide explains how to set up the enemy animation system with different enemy types.

## Overview

The enemy controller supports three different enemy types:
- Dinosaur (Type 0)
- Ama (Type 1)
- Lumen (Type 2)

Each type has its own walking animation.

## Step 1: Create Animation Clips

Create the following animation clips:
- Dinosaur_Walk.anim
- Ama_Walk.anim
- Lumen_Walk.anim

## Step 2: Set Up the Animator Controller

1. Create a new Animator Controller (EnemyAnimator)
2. Add only one parameter:
   - EnemyType (Integer) - Determines which enemy type is active (0 = Dinosaur, 1 = Ama, 2 = Lumen)

3. Create your animation states:
   - Dinosaur_Walk
   - Ama_Walk
   - Lumen_Walk

4. Set transitions directly from Entry to each animation with conditions:
   - To Dinosaur_Walk: EnemyType = 0
   - To Ama_Walk: EnemyType = 1
   - To Lumen_Walk: EnemyType = 2

## Step 3: Apply to Enemy Game Objects

1. Add the EnemyController script to your enemy GameObject
2. Set the Enemy Type in the inspector (Dinosaur, Ama, or Lumen)
3. Attach the Animator component and assign the EnemyAnimator controller

## Changing Enemy Types in Code

You can change the enemy type at runtime using:

```csharp
EnemyController enemyController = GetComponent<EnemyController>();
enemyController.SetEnemyType(EnemyController.EnemyType.Lumen); // Change to Lumen
```

## Animation Parameter

The EnemyType parameter is automatically set by the EnemyController:
- EnemyType: Integer value representing the enemy type (0 = Dinosaur, 1 = Ama, 2 = Lumen)