## Description

The application is designed to find a hand in a thermal image, and compare the coordinates of characteristic points on the hand, in the thermal image and digital image. The application shows characteristic points (exactly 20 points) and presents information about characteristic points from thermal and digital images using the OpenPose algorithm.

The program looks as follows, we have the option of choosing two photos for analysis, and fields where the coordinates of the points, the resolution of the given photo, and vectors of points are given. The size of the dots on the image depends on its resolution. Digital photos taken with a digital camera have a lower resolution, so two tables with coordinates are provided for these photos: one for the original resolution, the other for the resolution converted to the resolution of the first photo.

## Technologies

C#, OpenPose (v1.7.0), EmguCV (v4.5.1.4349)

All NuGet packages
![image](https://user-images.githubusercontent.com/41111309/229279115-4ca6ebbc-3ae0-4fc1-b85b-769017166917.png)


## Installation

1. Clone the repository
2. Download the OpenPose hand detection model:
    - Download file ```openpose-1.7.0-binaries-win64-cpu-python3.7-flir-3d.zip```
  from https://github.com/CMU-Perceptual-Computing-Lab/openpose/releases/tag/v1.7.0
    - Extract the downloaded archive
    - Open the file ```models/getBaseModels.bat```
    - After the wget program is finished, copy the file ```models/hand/pose_iter_102000.caffemodel``` to the folder ```Wykrywanie dłoni``` located in the repository from the first point
3. Open ```Wykrywanie dłoni/bin/Debug/Wykrywanie dłoni.exe``` or open project ```Wykrywanie dłoni.sln``` in Visual Studio and build app

## Screenshots

![image](https://user-images.githubusercontent.com/41111309/229280058-49e1f925-e389-4d02-9233-69fca6f10684.png)
![image](https://user-images.githubusercontent.com/41111309/229280129-b53bafc2-034b-4710-bd7b-c1347c0ff81a.png)
![image](https://user-images.githubusercontent.com/41111309/229280189-a0af8245-6cc1-4cad-ad1b-ffa5f3fb6d77.png)
