# Graphics Train Track Game 🚗🛤️

A C# Windows Forms interactive graphics game that allows the user to build and manipulate a railway track using different computer graphics techniques.

## Features

* Draw straight track segments using the DDA algorithm.
* Create circular track segments.
* Create custom Bézier curve track segments.
* Rotate and adjust the length of track segments.
* Modify curve control points.
* Undo the last added segment.
* Move the complete track using the arrow keys.
* Drive a car along the generated track.
* Score system during car movement.
* Start and exit game interface.
* Double buffering for smoother rendering.

## Computer Graphics Concepts

This project implements several computer graphics techniques:

* DDA Line Drawing Algorithm
* Circle Drawing
* Bézier Curves
* Geometric Transformations
* Rotation
* Translation
* Parametric Curves
* Double Buffering
* Animation using Windows Forms Timer

## Controls

| Key         | Action                           |
| ----------- | -------------------------------- |
| `1`         | Select DDA line segment          |
| `2`         | Select circular segment          |
| `3`         | Select Bézier curve segment      |
| `A`         | Add/confirm the current segment  |
| `R`         | Rotate segment                   |
| `Shift + R` | Rotate in the opposite direction |
| `L`         | Shorten line segment             |
| `Shift + L` | Increase line segment length     |
| `O`         | Decrease circle radius           |
| `Shift + O` | Increase circle radius           |
| `D`         | Undo the last segment            |
| `P`         | Start/stop car movement          |
| Arrow Keys  | Move the track                   |

## Technologies

* C#
* .NET
* Windows Forms
* System.Drawing
* Computer Graphics Algorithms

## Project Structure

* `Form1.cs` – Main game logic and graphics implementation.
* `imgg.png` – Game background image.
* `x.jpeg` – Car image used for animation.

## Demo 🎮

Watch the game demo here:

[Watch the Demo Video on Google Drive](https://drive.google.com/file/d/1GEVM3ER2WhlgkSN9K2xR5NoOx2vq6kyn/view?usp=sharing&utm_source=chatgpt.com)

The demo showcases the interactive track construction using DDA lines, circular paths, and Bézier curves, followed by the animated car movement along the generated track.

---

Developed as a Computer Graphics project using C# and Windows Forms.
