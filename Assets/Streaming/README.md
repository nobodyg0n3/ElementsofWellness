# OptiTrack Streaming (51379 Spatial Computing Design)

This code is for streaming OptiTrack motion capture data to Spatial Computing Design class student's laptop for easier project implemtation on their own laptop.

After setting this up, the class room computer with OptiTrack becomes a server, and all students can simultaneously access to the OptiTrack motion capture data. (We don't need to compete to occupy the computer :D)

## Setup computer with OptiTrack

1. Connect to WiFi (CMU Secure).

2. Check ip address of the classroom computer. This will be used in your own laptop later.
	- Click "Search" next to Windows icon on the bottom left.
	- Search "Command Prompt"
	- Type `ipconfig` then enter.
	- Find `Wireless LAN adapter Wi-Fi` then check the number next to `IPv4 Address`. Currently OptiTrack computer's ip address is `172.26.98.224`. Keep this for later use.

3. Setup simple Unity project with OptiTrack Unity Plugin.
	- This is the same procedure as what we already learn from the class.
	- Create new Unity Project.
	- Install package `OptiTrack_Unity_Plugin_1.4.0.unitypackage`. You can download it online or you can find it folder under `Documents/Spatial Computing Design/Student Proejcts - F2023/OptiTrackStreamTutorial`
	- Drag `Assets/OptiTrack/Prefabs/Client - OptiTrack.prefab` to `Hierarchy`.
	- Create `3D Object` by right click on the `Hierarchy`.
	- Name the object as you want, for example `Blue Egg`.
	- Drag `Assets/OptiTrack/Scripts/OptiTrackRigidBody.cs` to the object you just created.
	- Click the object and change the `Rigid Body Id` to the number you see in the Motive software.

4. Set up server!
	- Right click on the `Hierarchy` tab and click `Create Empty`. Name it whatever, for example `server`.
	- Drag `OptiTrackStreamServer.cs` to the `server` object. You can find this script in this GitHub or folder under `Documents/Spatial Computing Design/Student Proejcts - F2023/OptiTrackStreamTutorial`

5. Link your objects.
	- Click and open `Inspector` of objects that you want to track.
	- You will see `Tag` in the `Inspector` right under the object name. Change it to **`OptiTrack`** (Note lower and under cases. Should be exactly the same). If there is no such tag, click `Add Tag..` and create one.
	- End. Add this tag whichever you want to track.


## Setup your own laptop

1. Connect to WiFi (CMU Secure).

2. Open `OptiTrackStreamClient.cs` file and change `private string server_ipaddress = CHANGE HERE;` to the OptiTrack computer's ip address you checked earlier. You can find this script in this GitHub or folder under `Documents/Spatial Computing Design/Student Proejcts - F2023/OptiTrackStreamTutorial`

3. Open Unity project that you will build on.

4. Set up client!
	- Right click on the `Hierarchy` tab and click `Create Empty`. Name it whatever, for example `client`.
	- Drag `OptiTrackStreamClient.cs` to the `client` object. 

5. Link your objects.
	- Create object and name it as you set in the OptiTrack computer, for example `Blue Egg`. The object with the same name in the OptiTrack computer is linked to your object with the same name.
	- Click and open `Inspector` of the object.
	- Change `Tag` to **`OptiTrack`**.

## How to use

1. Click 'Play' button of the Unity project in the computer with OptiTrack (Server computer). You do this first before you click 'Play' on your own Unity project in your computer.
2. Click 'Play' button of the Unity project on your own laptop. Then you will see the objects are moving!

## Several notes
- Server computer can have many objects in its Unity project. In your own Unity project on your laptop, you don't need to create all objects in the server computer. For example, server computer can have both 'object 1 student A' and 'object 1 student B'. If you are student B, you don't need to create 'object 1 student A'.