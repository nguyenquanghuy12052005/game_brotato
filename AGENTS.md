# AGENTS.md

## Project

This is a small university Unity 2D game project inspired by Brotato.

The team has 3 student developers.

The priority is simplicity, readability, and completing the MVP rather than building advanced systems.

## Current milestone

We are currently working only on the first prototype foundation:

1. Unity project runs.
2. Gameplay scene runs.
3. Player appears.
4. Player moves using WASD.
5. Camera displays the gameplay correctly.

Do NOT implement weapons, enemies, waves, items, upgrades, UI systems, audio, animation, object pooling, or other gameplay systems unless explicitly requested.

## Technology

* Unity 2D
* C#
* Visual Studio Code
* Git
* Single-player game

Use the Unity version already configured by the project.

Do not upgrade Unity.

Do not install or update Unity packages unless explicitly requested.

## Coding style

Prefer simple code suitable for a fourth-year IT student learning Unity.

Rules:

* Prefer readable code over clever abstractions.
* Keep classes small and focused.
* Use meaningful names.
* Use private fields where possible.
* Use `[SerializeField]` for values that should be editable in the Unity Inspector.
* Avoid unnecessary design patterns.
* Avoid unnecessary interfaces, inheritance, dependency injection, service locators, or frameworks.
* Do not introduce third-party libraries.
* Do not over-engineer.
* Add short comments only when the reason for code is not obvious.

## Unity architecture

Organize scripts using:

Assets/
Scripts/
Core/
Player/
Weapon/
Enemy/
Wave/
Item/
UI/

For the current task, Player scripts belong in:

Assets/Scripts/Player/

Scenes belong in:

Assets/Scenes/

Prefabs belong in:

Assets/Prefabs/

## Player movement

For the first implementation:

* Player movement is top-down 2D.
* Controls are WASD.
* Diagonal movement must not be faster than horizontal/vertical movement.
* Player uses Rigidbody2D.
* Gravity must not affect the Player.
* Player rotation should remain locked.
* Movement speed should be adjustable from the Inspector.

Use the input system already available/configured in the project.

Do not install another input package just for Player movement.

## Unity scene safety

Do NOT manually rewrite large `.unity`, `.prefab`, `.meta`, or ProjectSettings files unless absolutely necessary.

Do not delete existing scenes or assets.

Prefer creating C# scripts and, when Editor automation is useful, create a small Unity Editor script instead of manually generating complex Unity YAML files.

Do not modify Packages/manifest.json unless explicitly required.

## Working rules

Before editing:

1. Inspect the existing project structure.
2. Inspect relevant files.
3. Explain briefly what will be changed.
4. Make the smallest change necessary.

After editing:

1. Check for obvious C# compile errors.
2. List files created or modified.
3. Explain how I should test the result inside Unity.
4. Do not claim Unity gameplay was verified unless it was actually executed successfully.

Do not continue to the next gameplay feature automatically.

Stop after completing the requested task.
