# motion-to-photon-measurement
This repository contains the Unity project needed to perform measurements of VR controller motion-to-photon latency. 
It was created and tested using Unity 2019.4.14f1 LTS, SteamVR (1.14.16, plugin version 2.6.1), and UXF.

**WARNING:** The combined measurement mode strobes the HMD screen between different colours continuously. 
Please be careful using this tool if you are sensitivie to strobing lights, e.g. with epileptic seizures.

## Operation
Inside the Unity project are two scenes, which operate different latency measurements (see below for an explanation). 
The project should be built to an executable using one of these two scenes.
Then with the VR equipment active andf SteamVR open, the executable should be opened.
It will prompt the user to select a settings file (see below for which should be used with which scene), and create a participant list (in this case a list of headsets tested), and to create a new "Participant ID", which should be the name of the headset.
The start button can then be clicked. 

The method following this will depend on the measurement mode used, and will explained below. 
Regardless of the mode, if measurements are being made of the motion-to-photon latency it is critical that a high-speed camera records the operation and that the HMD screen is fully visible (to record the screen colour) and that the controller position can be identified clearly in the video (e.g. by using an LED attached to the controller). 
Video clarity is improved by performing the testing in a dark room so that the contrast of the background and the HMD screen and LED are maximised. 

## Measurement modes
There are two included modes to run the latency measurements under (located in separate Unity scenes for convenience). 

### Combined measurement mode (default mode)
The first strobes the HMD screen between colours in a known pattern. 
This means that with a video recording the HMD screen and controller movement, and the files output from this program containing the HMD screen colour and virtual controller position, the screen colour can be used as a method to align the real and virtual controller motion, allowing assessment of latency throughout a movement. 
The "combined_experiment.json" setting file should be used for this mode. 

After pressing to start the testing, the user should move the VR controller to one extreme position and press the trigger button on the controller to take measurements of this extreme position. 
The screen will flash twice to indicate recording start and end respectively. 
The user should then move the controller to the other extreme position and repeat this procedure.
After this, the HMD screen will be strobed and the controller should be moved between the extreme positions with a sudden start to the movement and a smooth translation thereafter. 
This should be repeated as many times as desired.
The trigger button can then be pressed to start a new repetition of this procedure. 

### Individual measurement mode
The second mode allows measurements to be made at specific points in the movement, either at the start of the movement (discrete) or at set points inside a movement (continuous). 
The combined measurement mode encompasses both of these modes by allowing latency to be assessed at any point during a movement but these may be useful to easily compare between existing methods of motion-to-photon latency measurement.

#### Discrete
This will flash the HMD screen a colour when a sudden movement is detected. 
The "discrete_experiment.json" settings file should be used for this mode. 
The mode uses an outlier detection method based on how many median absolute deviations the current sample is away from a window of previous samples, and there are settings in the settings file to control both the length of this window and how many MADs away the sample should be to be classed as an outlier and hence as movement onset.

After pressing to start the testing, the user needs to apply sudden movements to the controller and the HMD screen will flash when a sudden movement is detected.
To move to a new repetition of the procedure, the trigger button should be pressed. 

#### Continuous

This will flash the HMD screen a colour when the controller crosses set threshold positions. 
The "continuous_experiment.json" settings file should be used for this mode. 

After pressing to start the testing, the user should move the VR controller to one extreme position and press the trigger button on the controller to take measurements of this extreme position. 
The screen will flash twice to indicate recording start and end respectively. 
The user should then move the controller to the other extreme position and repeat this procedure.
After this, the controller should be smoothly moved between the extreme positions with smooth translations.
Each time the controller crosses set threshold positions (e.g. the mid-point of the movement between the two extreme positions) the screen will flash. 
This should be repeated as many times as desired.
The trigger button can then be pressed to start a new repetition of this procedure. 
