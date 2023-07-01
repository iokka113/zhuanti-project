# Shattered Spacetime

[CHINESE VERSION](/README.md)

## Overview

![CoverArt.png](/Docs/CoverArt.png "CoverArt.png")

> **Development of this project has been discontinued**

"Shattered Spacetime" is a 2D Rougelike role-playing game with pixel art style.  
Player will need to explore in the dungeon to find the exit.  
The monsters and traps in the dungeon will attack you.  
Keep your mind and escape the dungeon.

## Documents

* [Development Proposal](/Docs/Proposal.pdf)
* [Project Report](/Docs/ProjectReport.pdf)

## Awards & Nominations

* Vision Get Wild Award 2022, PC Game Group (Nominated)

## Download

Click [HERE](https://github.com/iokka113/zhuanti-project/releases/latest) to download latest game version for Windows x86-64

## Development Experience

This work is a 2D stand-alone RPG game. Developed with C# .NET and Unity 2D engine.

The game procedure is driven by event-triggered (function pointer).

The biggest design focus is the monster's AI (as a finite state machine):  
Design the state of each monster as a diffenrent object, and use the factory method to generate it.  
By changing state in the state machine, simulate the monster's AI automatically.

In order to use resources efficiently, used the Object Pool Pattern:  
Every object that can be recycled by the object pool must be abstracted with a common interface, and managed uniformly by the static method of the object pool.

Each manager in the game (Player's Properties, Level Manager, UI Manager, etc.) is implemented from singleton base:  
By inheriting from the same generic base class, ensured that each manager will only have a single instance in the process to avoid program conflicts.

Make full use of object-oriented features (encapsulation, inheritance, polymorphism) to systematically develop a game.

## Licensing & Credits

All program code (i.e. C#) is copyrighted by [Iokka Lim](https://github.com/iokka113) and licensed under the [MIT License](/LICENSE).

All non-code assets (e.g. art, sounds, fonts, DLLs, markup language files) are copyrighted by their original authors, whether or not they are built into binaries. DO NOT reverse engineer, decompile or disassemble, and DO NOT copy, modify, release, sublicense, or otherwise violate the copyrights of the original authors without permission.

### Used Third-party Assets

* Character Sprites
  * Name: 2D Retro Heroes - SPUM Premium Addon Pack
  * Author: [soonsoon](https://assetstore.unity.com/publishers/4419)
* Mob Sprites
  * Name: Pixel Mobs
  * Author: [Henry Software](https://assetstore.unity.com/publishers/9216)
* Item Sprites
  * Name: 2D Pixel Item Asset Pack
  * Author: [Startled Pixels](https://assetstore.unity.com/publishers/31653)
* Background Sprites
  * Name: 2D Pixel Top-Down Dungeon Tileset
  * Author: [Matthias Stuetzer](https://assetstore.unity.com/publishers/23590)
* VFX Sprites
  * Author: [ぴぽや](http://blog.pipoya.net/)
* Sounds
  * Author: [小森　平](https://taira-komori.jpn.org/freesoundtw.html)
* Fonts
  * Name: 源界明朝 & 装甲明朝
  * Author: [FLOP DESIGN](https://flopdesign.booth.pm/)

Thanks to all the artists and creators mentioned above.
