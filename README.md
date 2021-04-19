# Super Grid Racer : Rebel Matrix  

Group project for Comp 476 (Advanced Game Development).   

## Getting Started

### Install Packages

In Unity, navigate to `Window > Package Manager`. In the Package Manager window, click the `advanced` dropdown and make sure `show preview packages` is enabled (In Unity 2020, they moved this option to `Edit > Project Settings > Package Manager`). Then, in the *search box*, search for and install the following packages:

1. **Universal RP**
    - After installing the *Universal RP* package, navigate to `Edit > Project Settings...`. In the *Project Settings* window click on `Graphics` in the left hand menu and change the `Scriptable Render Pipeline Settings` property to the one of the three located at `Assets/URP/UniversalRP-{QUALITY}`. Of course, you could just keep the default URP settings asset if you want.

2. **Shader Graph**

### Add Scenes to Build Settings

In Unity, navigate to `File > Build Settings...`. With the Build Settings window now open, navigate to the `Assets/_Scenes/` directory in the project explorer view and drag and drop all scene files into the *Scenes in Build* area. Be sure to put the scene named `Preload` at the top (index 0).  

### Choose Preload Scene

A preload scene is a scene that gets loaded first when your game starts and it usually contains code to initalize any configuration, systems, singletons, etc.  

There should be a preload scene already included and located at `Assets/_Scenes/Preload.unity`. In order to work in the unity editor and ensure that the preload scene gets loaded automatically before any scene that you run in the editor, you need to specify which scene is the preload scene. Navigate to `Tenacious > AutoPreloadScene > Load Preload Scene On Play` and select `Assets/_Scenes/Preload.unity` or your own custom preload scene. Make sure that this scene is also at the top of your Build Settings (index 0) so that it gets loaded first in production.  

### Edit Player Settings

In Unity, navigate to `Edit > Project Settings...`. In the *Project Settings* window, click on `Player` in the left hand menu. Change the `Company Name`, `Product Name`, and any other properties of your choosing.  

Open the included Splash scene located at `Assets/_Scenes/Splash.unity` and press the play button. At this point you are free to edit and add scenes and start making a game!


### Icon Sources
- missile: https://game-icons.net/1x1/lorc/rocket.html (Lorc)
- oil spill: https://game-icons.net/1x1/delapouite/leak.html (Delapouite)
- random: https://game-icons.net/1x1/lorc/uncertainty.html (Lorc)
- shield: https://game-icons.net/1x1/lorc/bordered-shield.html (Lorc)
- speed: https://game-icons.net/1x1/delapouite/speedometer.html (Delapouite)
