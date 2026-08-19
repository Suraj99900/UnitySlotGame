# Unity Slot Machine Game

## Game Overview

A 3-reel slot machine game developed in Unity as part of the Unity Slot Game Assignment.

The player starts with 100 credits and each spin costs 10 credits.

## Features

- 3-reel slot machine
- Randomized symbol outcomes
- Smooth reel spinning animation
- Different reel stopping times
- Credit system
- Multiple winning combinations
- Payout system
- Jackpot system
- Sound effects
- Win/lose feedback
- WebGL build

## Winning Combinations

| Combination | Payout |
|---|---:|
| 7 - 7 - 7 | +100 |
| Cherry - Cherry - Cherry | +25 |
| Bell - Bell - Bell | +50 |
| BAR - BAR - BAR | +75 |

## How to Play

1. Start the game.
2. Click the SPIN button.
3. 10 credits are deducted.
4. Wait for all three reels to stop.
5. Matching symbols provide a payout.
6. 777 awards the jackpot.

## Technical Approach

The game uses Unity C# coroutines to control reel spinning and stopping.

Each reel generates a random symbol using Unity's random number generator.

The final symbol index of each reel is stored and used to determine the winning combination.

Different stopping durations are used for each reel to create a more realistic slot-machine experience.

## Bonus Features

- Jackpot for 777
- Jackpot sound effect
- Win sound effects
- Animated win message
- Credit system

## WebGL Build

The WebGL build is available in:

Build/WebGL/

## Project Structure

Assets/
- Scripts/
- Prefabs/
- Animations/
- UI/
- Sounds/
- Sprites/