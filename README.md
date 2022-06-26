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

Then you will need these for packages in your source file:
- fmauch_universal_robot      -> https://github.com/ros-industrial/universal_robot/tree/melodic-devel
- ROS-TCP-Endpoint            -> https://github.com/Unity-Technologies/ROS-TCP-Endpoint
- Unity-Robotics-Hub          -> https://github.com/Unity-Technologies/Unity-Robotics-Hub
- Universal_Robots_ROS_Driver -> https://github.com/UniversalRobots/Universal_Robots_ROS_Driver

Then to set up for the controller of the real robot in the ros side, do as shown in this video: https://www.youtube.com/watch?v=j6bBxfD_bYs

To launch the ros side you will need some modified launchfiles and scripts. 
These are:
- ur5e_moveit_planning_execution.launch 
- moveit_rviz.launch
- mover.py
- ...
Replace the existing files with similar names in your file system with the given launchfiles and scripts.
Also make sure the ip of the robot and the tcp server is correct in the launchfiles. 


If something is missing, dont hasitate to contact me at havar@dankel.com
