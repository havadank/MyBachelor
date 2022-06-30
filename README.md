# MyBachelor
a unity project that can control a ur5e physical or simulated robot via the hololens 2 and communication through ros

Note: bouth hololens and the pc running unity needs to be set to developper mode for the communication between the elements to work.
# Setup Unity:

Download the git and upload to your unity project. It is recomended to use Unity 2020.3 LTS, this is what is used in our demonstration.

When the project is up and running in your unity, you need to enable all recomended settings for the HL2 to work.
go to Mixed reality in the toolbar in the top of the project window -> Project -> apply bouth recomended scene/project settings for hololens 2.
Then go to build settings and make sure Universal Windows platform is selected
Then go to player settings>XR-plug-in management, and tick initialize on startup, openXR and Microsoft hololens feature group
Then go to OpenXR and again tick microsoft hololens, hand-tracking, Moxed Reality Fratures and Motion Controller Model
Then good to go. 

To run the unity side, go to the Robotics Hub tab at the top, click on ROS settings, then make sure these settings corresponsd with the server endpoint you are setting up in ROS.
Last step is to go to the mixed reality tab, click remoting>holographic remoting for play mode. Make sure the ip and port name corresponds with the one given in the hololens.

# Hololens setup

Connect to the same network as the pc that unity runs, download holographic remoting on Hololens 2, run it and connect to unity. 

# Setup Ros
melodic is used in this project.
First create a catkin ws. 

Then you will need these for packages in your source (src) folder:
- fmauch_universal_robot      -> https://github.com/ros-industrial/universal_robot/tree/melodic-devel
- ROS-TCP-Endpoint            -> https://github.com/Unity-Technologies/ROS-TCP-Endpoint
- Unity-Robotics-Hub          -> https://github.com/Unity-Technologies/Unity-Robotics-Hub
- Universal_Robots_ROS_Driver -> https://github.com/UniversalRobots/Universal_Robots_ROS_Driver

# Install commands and instructions (by 595489)
This assumes you have a catkin workspace setup in the default location

`cd ~/catkin_ws`

`git clone https://github.com/ros-industrial/universal_robot.git src/fmauch_universal_robot`

`git clone https://github.com/Unity-Technologies/ROS-TCP-Endpoint.git src/ROS-TCP-Endpoint`  

If like me (595489) you are using ros-noetic instead of melodic like the original project did, you might have to edit the package.xml and default_server_endpoint.py to force python 3. Instructions for that can be found at the bottom.

`git clone https://github.com/Unity-Technologies/Unity-Robotics-Hub.git src/Unity-Robotics-Hub`  

Here, depending on your needs you should either delete the tutorials folder entirely (or move it out of the catkin_ws), or delete all ros2 subfolders as well as the tutorials/pick_and_place/PickAndPlaceProject holders, as these are duplicate packages as far as ros can tell, and therefore won't let you compile.

`catkin_make` (to see if everything compiles propperly.)   

I had a few issues where CMakeFiles.txt and project.xml for multiple projects didn't contain all the dependencies it needed, and also was looking in the wrong folder for packages. In my particular case my moveit_msgs_msg_paths.cmake file located in *~/catkin_ws/devel/share/moveit_msgs/cmake/* didn't point to the correct folder at all. The path should be */opt/ros/noetic/share/moveit_msgs/msg* after the ; (replace noetic with your ros-distro name).

finally

`git clone https://github.com/UniversalRobots/Universal_Robots_ROS_Driver.git src/Universal_Robots_ROS_Driver`

`catkin_make`

The Universal_Robots_ROS_Driver will likely fail to compile, stating something along the lines of "No member *mass*" and "No member *center_of_gravity*" if this happens locate your "~/catkin_ws/src/fmauch_universal_robot/ur_msgs/srv/SetPayload.srv" and replace the contents with:

```float32 payload
float32 mass

geometry_msgs/Vector3 center_of_gravity
-----------------------

bool success
```


# Setup ROS
Then to set up for the controller of the real robot in the ros side, do as shown in this video: https://www.youtube.com/watch?v=j6bBxfD_bYs

To launch the ros side you will need some modified launchfiles and scripts. 
These are:
- ur5e_moveit_planning_execution.launch 
- moveit_rviz.launch
- mover.py
- ...
Replace the existing files with similar names in your file system with the given launchfiles and scripts.
Also make sure the ip of the robot and the tcp server is correct in the launchfiles. 


If something is missing, dont hesitate to contact me at havar@dankel.com


# Force python3
If by chance your project doesn't automatically select the propper python version, one thing to try is modifying the package.xml for the package, as well as specifying python version in any script that is executable. The shebang (**#!/bin/bash** or **#!/usr/bin/env** to give examples of shebangs) in ros should usually be **#!/usr/bin/env python**

On the ROS-TCP-Endpoint package I had issues relating to the python version.
In my case I had to change the shebang in *~/catkin_ws/src/ROS-TCP-Endpoint/src/ros_tcp_endpoint/default_server_endpoint.py* to **#!/usr/bin/env python3**, effectively forcing the interpreter to run python3 instead of python2. This is **NOT** the recommended way of doing it, as the ros configuration should automatically use the correct python version if configured propperly, but might be usefull none the less.

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

Similarly we should make sure the file also contains `condition="$ROS_PYTHON_VERSION == 3"` inside dependency tags

Example:
  `<exec_depend condition="$ROS_PYTHON_VERSION == 3">**PackageNameHere**</exec_depend>`

as opposed to:
  `<exec_depend>**PackageNameHere**</exec_depend>`

The ROS documentation regarding python versions, and changing it can be found here: https://wiki.ros.org/UsingPython3
