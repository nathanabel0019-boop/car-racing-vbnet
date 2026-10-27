# Mobil Racing (VB.NET)

A simple car racing game built with VB.NET and Windows Forms, created as a project for a Visual Programming course. Dodge the oncoming cars, grab power-ups, and chase the high score, alone or against a friend on the same keyboard.

## Features
- Main menu with 1 Player and Multiplayer modes
- Forward, backward, left, and right controls (speed rises when moving forward and drops when moving backward)
- Local 2-player mode on one keyboard, with a score battle
- Power-ups:
  - Shield: immune to collisions for a few seconds
  - x2 Points: doubles the points you earn for a few seconds
- Odometer (distance traveled)
- High score and longest distance, saved automatically
- 6 achievements with an on-screen notification when unlocked
- Pause, Resume, Replay, and return to the main menu
- Continuously scrolling road and roadside scenery

## Controls
| Player | Controls |
|---|---|
| Player 1 | Arrow keys (Up / Down / Left / Right) |
| Player 2 | W / S / A / D |

## How to Run
1. Open the `.sln` file in Visual Studio.
2. Press F5.

Requires Windows and the .NET SDK or runtime that matches the project's target framework.

## Saved Data
The high score, longest distance, and achievements are saved to a file named `MobilRacing_data.txt` in the `%AppData%` folder. Delete that file to reset your progress.

## Tech Stack
- Language: VB.NET
- Framework: Windows Forms (.NET)
- IDE: Visual Studio
- Concepts used: arrays, loops, timers, event handlers, file storage

## Screenshot
<img width="1280" height="720" alt="yutub" src="https://github.com/user-attachments/assets/bda64202-8505-46b7-9b7f-078d4017b761" />


## Author
Nathasya Yohana Rosabel
