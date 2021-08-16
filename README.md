# PlanetarySystem [[Youtube Presentation](https://www.youtube.com/watch?v=nmisIYMvPkU)]

<h3> Application description </h3>

Unity3D single-player desktop game.
The theme of the game is cosmic exploration. The player is joining an unreal world where he becomes a Planetary System creator. Settled up with an inventory of planet items, the player is building up planetary systems by mimicking each planet's 3D trajectory by a continuous mouse input. Depending on the current planet type (for example, Jupiter), it has a specific speed, size, and it is seemingly a real kind of that planet.

The player's final task is to manage the space of each planetary system so that the planets don't collide while rotating. If two planets collide, both will be disintegrated into smaller pieces that can potentially collide and destroy other planets as well. A broken planet fragment is not living for long until they burn entirely and they become ashes.

The game's progress depends of the current game level and the planets in the inventory. A minimum number of planets is required for each system to be completed. The broken planets obviously lose the count. If the player ends up with 0 planets in their invetory, the game is lost.

This is a short demo of planet trajectory and collisions simulation:

![Demo](https://github.com/BogdanPolitic/Demos/blob/main/Planetary-System-demo.gif?raw=true)

<p align="center">
  Player's inventory looks like this:
  
  <img src="https://github.com/BogdanPolitic/Demos/blob/main/PlanetInventory.jpg" />
</p>

<h3> Implementation details </h3>

The implementation mainly focused on:
- the planet's orbiting trajectory
- two planets collisions
- a well organised and good looking inventory, based on the level logic
- a 3D menu build

The trajectory of each planet is calculated by filtering the user's mouse input. Since the input is likely to be spiky and discontinuous, the best solution approximates the input to a smooth, uniform, convex and closed curve (similar to a Bezier curve).

The collision between two planets is predetermined by faking the colliders of each planet, placing them ahead, and detect collision one framecount before the actual collision happens. When the ghost colliders trigger - it means the planets will collider next frame - the planets already start the disintegration process. On the next frame, Unity's physical colliders apply directly to the planet's fragments. There are two phases of disintegration: the fragment splitting and the fragment's burning to become cosmic dust.

<p align="center">
  Example of a collision detection between a planet and a planet fragment. The dash lines are the fake colliders (CHECKER)
  <img src="https://github.com/BogdanPolitic/Demos/blob/main/ciocnire_planeta_fragment.png" />
</p>
