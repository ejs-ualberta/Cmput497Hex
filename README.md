# Hex

This repository contains an implementation of the game *Hex* in Unity by David Pankratz for Cmput 497.

# Installing

## OSX
 - Press `Download Unity Hub` on the download [page][Unity hub download]
 - Run the installer `UnityHubSetup.dmg`
 - Press `Agree`.
 - Drag the Unity Hub icon to the applications folder
 - Run Unity Hub from Launchpad
 - In Unity Hub, press the `installs` button on the left side and then press `Add` on the right side.
 - Choose Version `Unity 2019.2.9f1` or similar and press `next`
 - (Optional) If you're planning on modifying code then it is recommended to keep selected Visual Studio for Mac
 - (Optional) Depending on your target platforms you can select build tools to create standalone builds.
 - Once you have chosen your desired add-ons press `Next`.
 - As the Unity Editor is installing you can clone this repository.
 - Once the editor is installed, press the `projects` button on the left side of Unity Hub.
 - Press the `add` button and navigate to the folder containing the repo and press `open`.
 - Open the project called `Cmput497Hex` and then press confirm to upgrade the project.
 - Once the project is opened, click on the folder `Scenes` and drag the scene called `Hex` to the `Hierarchy` tab.

## Windows
 - Note that windows does not currently support the Benzene integration due to difficulty building on Windows.

## Ubuntu
 - Installation is analogous to **OSX** except that Ubuntu does not have Visual Studio support. 
 - Instead Visual Studios Code can be used which also has some integration with Unity. 

## General
 - Once you have chosen your desired add-ons press `Next`.
 - As the Unity Editor is installing you can clone this repository.
 - Once the editor is installed, press the `projects` button on the left side of Unity Hub.
 - Press the `add` button and navigate to the folder containing the repo and press `open`.
 - Open the project called `Cmput497Hex` and then press confirm to upgrade the project.
 - It is possible that the main scene will not be included by default when opening the project for the first time. To remedy this navigate to `Assets/Scenes` in the **Project** window and drag the `Domineering` scene to the **Hierarchy** window. 
 - At this point the project should be setup and can be ran by pressing the triangular run button at the top of the editor. 

# Building
 One nice feature of using Unity is that this game can be ported to a variety of platforms and behave similarly across all of them. To create a build for a given platform you can follow these instructions:
 - Open the  *File -> Build Settings...* and then click on the desired platform. 
 - Unity will likely prompt you the first time to install dependancies for building for that platform so accept and allow it to install.
 - Afterwards you should be able press the *Switch Platform* and then *Build and Run* to build. 

[Unity hub download]: <https://unity3d.com/get-unity/download>
