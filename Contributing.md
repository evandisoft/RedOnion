# Hi

We welcome people of all skill levels to participate in the project. However please talk to us first if you want to make a contribution. Either make make an issue [here on github](https://github.com/evandisoft/RedOnion/issues), talk about it in [this thread](https://forum.kerbalspaceprogram.com/index.php?/topic/189983-110x-redonion-unrestricted-in-game-scripting-v052/page/2/), and/or request to join the discord.


Contributing is not limited to coding/fixing bugs. There are a lot of other ways to contribute:
- Feedback: Specific or general feedback about the project. Ideas for modifying or adding features to the mod. 
- Testers: People willing to test the project and provide feedback on bugs. One relatively big testing gap we have is that currently neither of the main developers have a Mac computer to test/develop on.
- Documentation writing/review: Is the documentation confusing? Should any sections be added/fleshed out more? Are any sections outdated, or otherwise irrelevant?
- Videos: If you like the mod and want to make a video about it, that would be helpful as well! And if it is a helpful video, we'll add a link to it in the pertinent documentation section, and/or the videos section.
- Other miscellaneous help: If you want to help in ways that are not listed here, we're open to suggestions!

## Coding Contributions

We are developing this on linux and windows. Here is some information about getting started.

Make a fork of our repository, [RedOnion](https://github.com/evandisoft/RedOnion), on github. Then clone it to your development machine.

Make a new branch with your username in it, and use that to contribute and make pull requests on github.

## Linux:
For Linux you have to use the repositories [Here](https://www.mono-project.com/download/stable/) to get the best version of the mono tools, including monodevelop (which is a very good ide on linux for c# development). The default version of mono tools on your linux distribution may not work at all with this project.

Note that due to KSP using a version of Unity that was not released for Linux, debugging plugin mods does not seem to work. However, the nature of this project means that we may be able to eventually provide better debugging tools inside for our scripting languages, or even the regular C# development part.

## Windows:
On Windows, Visual Studio 2017 Community is the easiest way to contribute.

## Common 
In order to work with the KSP library dll's, we have a symbolic link called "ksp" we place in the repository directory.
The link should link to your KSP game install.

On Windows, you can make symbolic links using [mklink](https://www.howtogeek.com/howto/16226/complete-guide-to-symbolic-links-symlinks-on-windows-or-linux/). Use the version that has the "/D" option.

Once you have that set up, you can build the project. The output will go into GameData/RedOnion/Plugins. In order for your KSP install to see these resulting dll's, you can create a link in the GameData folder of your ksp install called "RedOnion" that links to GameData/RedOnion in your project folder.

Firda has created a cmd script (create-links.cmd) for windows that will create these links assuming that you have set your ksp install directory in the environment variable "ksp". If you have not, it assumes ksp is installed in the default location it would be installed under steam.
("C:\Program Files (x86)\Steam\steamapps\common\Kehttps://forum.kerbalspaceprogram.com/index.php?/topic/183050-redonion-033-unrestricted-in-game-scripting-has-repl-editor-and-intellisense-lua-and-a-custom-jsruby-like-language-implemented-tested-on-ksp-17/&tab=comments#comment-3566618