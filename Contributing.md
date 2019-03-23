# Hi

(This is a work in progress. I imagine these instructions are not amazing. Any feedback or questions are welcome as "issues" on the github.)

We welcome people of all skill levels to participate in the project.

We are developing this on linux and windows. Here is some information about getting started:

Make a fork of our repository, [RedOnion](https://github.com/evandisoft/RedOnion), on github. Then clone it to your development machine.

Make a new branch with your username in it, and use that to contribute and make pull requests on github.

## Linux:
For Linux you have to use the repositories [Here](https://www.mono-project.com/download/stable/) to get the best version of the mono tools, including monodevelop (which is a very good ide on linux for c# development). The default version of mono tools on your linux distribution may not work at all with this project.

Note that due to KSP using a version of Unity that was not released for Linux, debugging C# code does not seem to work. However, the nature of this project means that we may be able to provide better debugging tools inside for our scripting languages, or even the regular C# development part.

## Windows:
On Windows, Visual Studio 2017 Community is the easiest way to contribute.

## Common 
In order to work with the KSP library dll's, we have a symbolic link we place in the directory above the repository directory.
The link should link to your KSP game install.

On Windows, you can make symbolic links using [mklink](https://www.howtogeek.com/howto/16226/complete-guide-to-symbolic-links-symlinks-on-windows-or-linux/). Use the version that has the "/D" option.

Once you have that set up, you can build the project. The output will go into GameData/RedOnion/Plugins. In order for your KSP install to see these resulting dll's, you can create a link in the GameData folder of your ksp install called "RedOnion" that links to GameData/RedOnion in your project folder.

Firda has created a cmd script (create-links.cmd) for windows that will create these links assuming that you have set your ksp install directory in the environment variable "ksp". If you have not, it assumes ksp is installed in the default location it would be installed under steam.
("C:\Program Files (x86)\Steam\steamapps\common\Kerbal Space Program")

Once again, any questions, feedback, concerns for now can be placed into "issues" on the project github page.