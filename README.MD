# AR - Instantiation, rotation and destruction of game objects.

## Overview
This is an example for Unity (2019.2.12f1) on how to instantiate, rotate and destroy game objects in augmented reality using the AR Foundation API.

## Demo execution
![](AR_1.gif)

## Features
- User can switch between two types of prefabs to instantiate: Picture frames or a TV.
- These objects can be instantiated by performing a single short touch on an AR plane.
- Objects can be destroyed if they are tapped over them.
- In order to rotate a object, just long press over a object and it will being rotating clockwise.

### Considerations:
This has been tested using an Android Virtual Device (AVD) since the AR Foundation API requires the AR Core service running in a supported target device (which I don't have at this time).
