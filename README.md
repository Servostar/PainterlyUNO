# PainterlyUNO
A small C# application for creating and editing images, which then can be uploaded to an microcontroller which manages an matrix of LEDs.

> This project is currently in alpha status

Features
-
* Editing new GIF images
  * Simple painting tool
    * supports brush radius control
    * brush color
  * Fill and Clear tool
  * Timeline for editing individual frames  
  * 2D filter (GLSL like)
* Arbitrary image scale up to 20x20 (In virtual mode up to 512x512, but in here no data is send to the microcontroller)
* Image uploading to the microcontroller (Right code required on controller)

Microcontroller
-
Preferrably an Arduino UNO, since this is the prime target device.
Working production code for Arduino UNO can be found in ./arduino
