# MyBachelor
A Unity project that can control a UR5e physical or simulated robot via the HoloLens 2 and communication through ROS

**Note:** both the HoloLens and the PC running Unity needs to be set to developer mode for the communication between the elements to work.

# Setup Unity

Download the git repository and open as a Unity project. It is recomended to use Unity 2020.3 LTS, this is what was used in our demonstration. (Any 2020.3.xx LTS release should work)

When the project is up and running in Unity, you need to enable all recomended settings for the HoloLens 2 to work.

1. In Unity go to Mixed Reality -> Project -> Apply recommended project/scene settings for HoloLens 2. (Click both options)  
2. Go to File -> Build Settings (CTRL+SHIFT+B) and make sure Universal Windows platform is selected  
3. Then go to Edit -> Project Settings.  
4. Locate XR-Plug-in Management, and tick initialize XR on startup, openXR and Microsoft HoloLens feature group.  
5. Then go to OpenXR (below XR-Plug-in Management) and tick Microsoft HoloLens, Hand-Tracking, Mixed Reality Fratures and Motion Controller Model.  
6. You are good to go, and can now close the project settings and build settings windows. 

To run the unity side, go to the Robotics Hub tab at the top, click on ROS settings, then make sure these settings correspond with the server endpoint you are setting up in ROS.

The last step is to go to the Mixed Reality tab, click remoting -> holographic remoting for play mode. Make sure the IP and Port correspond with the one given in the HoloLens.

# HoloLens setup

Connect to the same network as the PC that runs Unity, download the holographic remoting app on the HoloLens 2 app store, run it and read the IP address. (This is the one used for the last step in the Unity section above)

**Note:** The first time you open the holographic remoting app you should probably go to the listed IP address in a browser and follow the instructions there. This isn't mandatory as far as I can tell, but is good practice regardless

# Setup ROS
ROS uses Ubuntu Linux as the platform of choice. Depending on the Ubuntu release you need different versions of ROS. This guide assumes you have both Ubuntu and ROS installed, but if not you need to do so before proceeding. You can use either a Virtual Machine, WSL (Windows Subsystem for Linux) or another PC for this.

For help installing ROS, follow this tutorial:  
`ROS Noetic (for Ubuntu LTS 20-04)`: https://wiki.ros.org/noetic/Installation/Ubuntu  
`ROS Melodic (for Ubuntu LTS 18-04)`: https://wiki.ros.org/melodic/Installation/Ubuntu

Melodic was used when creating this project. Noetic should also work, it is the version I am using, but may need certain alterations (I will detail those that I know when they are relevant).

First create a catkin workspace if you haven't done so yet. Follow the tutorials here if needed: https://wiki.ros.org/ROS/Tutorials  


Then you will need these four packages in your source (src) folder. (Just follow the commands in the install instructions below):
- fmauch_universal_robot      -> https://github.com/ros-industrial/universal_robot/tree/melodic-devel
- ROS-TCP-Endpoint            -> https://github.com/Unity-Technologies/ROS-TCP-Endpoint
- Unity-Robotics-Hub          -> https://github.com/Unity-Technologies/Unity-Robotics-Hub
- Universal_Robots_ROS_Driver -> https://github.com/UniversalRobots/Universal_Robots_ROS_Driver

## Install instructions for ROS packages
This assumes you have a catkin workspace (catkin_ws) setup in the default location

`cd ~/catkin_ws`

`git clone https://github.com/ros-industrial/universal_robot.git src/fmauch_universal_robot`

`git clone https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git src/ROS-TCP-Endpoint`  

If like me you are using ros-noetic instead of melodic like the original project did, you might have to edit the `package.xml` and `default_server_endpoint.py` to force python 3. Instructions for that can be found at the bottom.

`git clone https://github.com/Unity-Technologies/Unity-Robotics-Hub.git src/Unity-Robotics-Hub`  

Here, depending on your needs you should either delete the `tutorials` folder entirely (or move it out of the catkin_ws if you want to have a backup), or delete all `ros2` subfolders as well as the `tutorials/pick_and_place/PickAndPlaceProject` folder, as these are/contain duplicate packages, and therefore won't let you compile.

`catkin_make` (not needed, but could be nice to check if everything compiles correctly so far)

I had a few issues where `CMakeFiles.txt` and `project.xml` files for multiple packages didn't contain all the dependencies it needed, and also was looking in the wrong folder for packages. Hopefully you don't have these issues (they were likely partially user error), but one of my particular issues included that the `moveit_msgs_msg_paths.cmake` file located in `~/catkin_ws/devel/share/moveit_msgs/cmake/` didn't point to the correct folder at all. The path should be `/opt/ros/$ROS_DISTRO/share/moveit_msgs/msg` (after the `;`) (replace `$ROS_DISTRO` with your ros-distro name, `noetic`, `melodic`, `kinetic` etc).

finally

`git clone https://github.com/UniversalRobots/Universal_Robots_ROS_Driver.git src/Universal_Robots_ROS_Driver`

`catkin_make`

The Universal_Robots_ROS_Driver will likely fail to compile, stating something along the lines of `No member mass` and `No member center_of_gravity` if this happens locate `~/catkin_ws/src/fmauch_universal_robot/ur_msgs/srv/SetPayload.srv` and replace the contents with:

``` C++
float32 payload
float32 mass

geometry_msgs/Vector3 center_of_gravity
-----------------------

bool success
```


## Modify ur_5_e_moveit_config
Then to set up for the controller of the real robot in the ros side, do as shown in this video: https://www.youtube.com/watch?v=j6bBxfD_bYs


**IMPORTANT NOTE:**  
I had to make alterations to this next process quite heavily.  
I will make a list of things I did at some point, just note that for me at least, dropping the `ur5e_moveit_config` folder (from this repo) into the `fmauch_universal_robot` package and thereby replacing `ur5_e_moveit_config` (as I was instructed to do by the previous team) did not work (yes I know the instructions below don't say this, but I was told to do so in person regardless).  
In the end I had to manually modify the existing `ur5_e_moveit_config` with both the files found in this repo, and some manual rewrites.  
Make sure to rename the files you copy into `fmauch_universal_robot`, so they (and their contents) read `ur5_e_` instead of the current `ur5e_`  
I hope to be able to provide a better simpler solution soon, but for now be aware of the dangers of just overriding the files and folders.  
There will likely be missing dependencies and more.

**Original instructions:**  
To launch the ros side you will need some modified launchfiles and scripts.  
These are:
- ur5e_moveit_planning_execution.launch 
- moveit_rviz.launch
- mover.py
- ...
Replace the existing files with similar names in your file system with the given launchfiles and scripts.
Also make sure the ip of the robot and the tcp server is correct in the launchfiles. 


If something is missing, don't hesitate to contact me at havar@dankel.com


# Force python3
If by chance your project doesn't automatically select the propper python version, one thing to try is modifying the package.xml for the package, as well as specifying which python version to use in any script that is executable. The shebang (`#!/bin/bash` or `#!/usr/bin/env python` are examples of shebangs) in ROS should usually *(always)* be `#!/usr/bin/env python` according to the ROS wiki.

On the ROS-TCP-Endpoint package I had issues relating to the python version.
In my case I had to change the shebang in `~/catkin_ws/src/ROS-TCP-Endpoint/src/ros_tcp_endpoint/default_server_endpoint.py` to be `#!/usr/bin/env python3`, effectively forcing the interpreter to run python3 instead of python2. This is **NOT** the recommended way of doing it, as ROS should automatically use the correct python version if configured correctly, but might be usefull none the less.

You should probably also alter the package.xml to make sure ROS knows to use python3.
This is done by altering a few lines. Line 1 to 5 should look like:

```xml
<?xml version="1.0"?>
<?xml-model
  href="http://download.ros.org/schema/package_format3.xsd"
  schematypens="http://www.w3.org/2001/XMLSchema"?>`
<package format="3">
```

This should replace the similar fields on the existing section.

Similarly we should also make sure the file contains `condition="$ROS_PYTHON_VERSION == 3"` inside the relevant dependency tags

Example:
  `<exec_depend condition="$ROS_PYTHON_VERSION == 3">**PackageNameHere**</exec_depend>`

As opposed to:
  `<exec_depend>**PackageNameHere**</exec_depend>`

The ROS documentation regarding python versions, and changing it can be found here: https://wiki.ros.org/UsingPython3

***

Hope this guide is comprehensive enough.  
I have spent the better part of 3 days on getting the ROS side set up, so I decided it was way worth it to write a better guide than what I was given as a starting point.  
I have rewritten the parts that were decent, hopefully making them better, and added plenty more besides.  
Even then I know there will be issues that I have missed, but I hope there will be enough details to assist in troubleshooting, and make the process less painful.  
I left the original email where it was, so it should still be easy to contact the original creators (or me through them if needed) when this is finally merged back into the original repository.  

Cheers and good luck.  
595489
