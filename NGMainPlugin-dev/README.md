# Info  
 This is the main plugin for the SCP:SL server Nexus Gaming \
 Discord: [Nexus Gaming](https://discord.gg/xHw7uD6Fr8)

 __***This is the Dev-Branch and version***__
 ## Configs

 Config settings for the individual items will ***NOT*** be found in the default plugin config file. Instead they will be located in ~/.config/EXILED/Configs/CustomItems on Linux or %AppData%\EXILED\Configs\CustomItems on Winblows.
The default config file will be named "global.yml" however, the file used can be changed for each SCP server via that server's normal plugin config file, if you wish to run multiple servers with different custom item config settings.

### Permissions
All EXILED permissions have the "ng" prefix.

e.g.: ng.Admin

### Item list
ItemName | ItemID | Description
:---: | :---: | :------
Grenade Launcher | 0 | A grenade launcher. This weapon shoots grenades that explode on impact with anything, instead of bullets.
Door Remote *(Doesn't work yet)* | 2 | This is a remote control for doors, you can lock and unlock them.
Impact Grenades | 3 | A grenade, which instantly explodes uppon impact.
SCP-1499 | 8 | The gas mask that teleports you to another dimension, when you put it on.
Sniper | 10 | A sniper rifle. Also self-explanatory.
Tranquilizer Gun | 11 | This gun is also modified to fire self-injecting projectile darts. When fired at a hostile target, it will tranquilize them, rendering them unconscious for several seconds.
C4 | 15 | A frag-grenade with a much longer than normal fuse, that will stick to the first solid surface it comes in contact with. It can be detonated using a console command. ".detonate"



### Commands
Command | Arguments | Permissions | Description
:---: | :---: | :---: | :------
ci give | (item name/id) [player] | citems.give | Gives the specified item to the indicated player. If no player is specified it gives it to the person running the command. IN-GAME RA COMMAND ONLY.
ci spawn | (item name/id) (location) | citems.spawn | Spawns the specified item at the specified location. This location can either be one of the valid Spawn Location's below, a player's name (it spawns at their feet), or in-game coordinates.
ci info | (item name/id) | n/a | Prints a more detailed list of info about a specific item, including name, id, description and spawn locations + chances.
ci list | n/a | n/a | Lists the names and ID's of all installed and enabled custom items on the server.
.detonate | n/a | n/a | Detonates any C4-Charges you have placed, if you are within range of them.

### Valid Spawn Location names
The following list of locations are the only ones that are able to be used in the SpawnLocation configs for each item:
(Their names must be typed EXACTLY as they are listed, otherwise you will probably break your item config file)
```
Inside012
Inside012Bottom
Inside012Locker
Inside049Armory
Inside079Secondary
Inside096
Inside173Armory
Inside173Bottom
Inside173Connector
InsideEscapePrimary
InsideEscapeSecondary
InsideIntercom
InsideLczArmory
InsideLczCafe
InsideNukeArmory
InsideSurfaceNuke
Inside079First
Inside173Gate
Inside914
InsideGateA
InsideGateB
InsideGr18
InsideHczArmory
InsideHid
InsideHidLeft
InsideHidRight
InsideLczWc
InsideServersBottom
```