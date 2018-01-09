![#000000](https://github.com/wren11/Lorule-Dark-Ages-Server/blob/master/GitStuff/pictures/darkages.gif?raw=true)

# Lorule-Dark-Ages-Server
## Discord- https://discord.gg/PwbFH3T

out-of-the-box Sand box Server For Dark Ages Client 7.18

![Alt text](https://github.com/wren11/Lorule-Dark-Ages-Server/blob/master/GitStuff/pictures/lorule1.png?raw=true "Server")

![Alt text](https://github.com/wren11/Lorule-Dark-Ages-Server/blob/master/GitStuff/pictures/lorule2.png?raw=true "In Game")



## About This Project:
A Darkages Server Project - For client **7.18** Only. (Customized Client will be available soon! (Major Changes!))


## Project Design Overview.
- Json based storage and cache
- SingleInstance Object Manager Service
- Configurable From UI
- Scriptable Storage System/Content
- Component driven design pattern.
- Component Proxy Service to support server transitions and multiple servers.


## Server Development Status:
- Server is currently about ~~92%~~ 94% percent completed until it is content ready.
- Expect Glithes! Help by testing and reporting them!

## Working Features
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Monsters, Items, Mundanes
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Spells/Skills
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Equipment
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Shops, Dialogs, Pursuits
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Maps, Zones
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Death Handling, you can die, revive. ect
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Elements, Fire, Water, Earth, Wind, Dark, Light
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Regen
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Player PVP, Arena is working with a NPC for reviving.
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Assails, Ambush, Crasher, Kelb, Wff, pramh - all working
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Spell Bars, Buffs, Debuffs
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Quests, Examples can be found in gos/benson templates.
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Loot Systems, Experience System
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Object Spawning
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) World Maps, World Portals, Warps
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Grouping
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Community States, Awake, daydreaming ect.
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Elementals
- [x] ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Quests



## What is missing?
- [ ] ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) Meta Database
- [ ] ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) Exchanging
- [ ] ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) In-game Boards
- [ ] ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) Whispering, Ignore



## Progress Map 09/01/2018
- Exchanging

## Building & Compiling
- This was developed using Visual Studio 2017 Community (https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15#)
    - Using Microsoft's Framework **4.6.1**
    - By default, It will compile to ..\Staging\bin\Debug\

So the Staging Folder is the place to seek when making changes to templates, other settings ect.
as everything will by default point to that directory.


## Using this Server:

- Download and Compile the Lorule Source code, It should compile out-of-the-box
- Download and Install Dark Ages Client 7.18
        - https://drive.google.com/file/d/1EbIf7AzQLJaUrR9Kd3wmZDQWM7qT0-hR/view?usp=sharing    
- Download our modified Darkages.exe executable and replace the previous one in the 7.18 Installation Directory.
        - https://drive.google.com/file/d/18fCo2kyL1pF6QPJ9TgO1PIGmfKn5Fm_D/view?usp=sharing
    
That is all you need to run the server on your local system.
To host your server online, you must use the Client Launcher for your users to connect to your IP Address.
Your IPAddress also needs to reflect the **server.tbl** file located in your Server's running directory.

Lorule Client Launcher Download:    
        - https://drive.google.com/file/d/16Uff3R23Qdg3qRiaMbCG2q1szgfCj4LG/view?usp=sharing 
        - (optional) - Copy the Darkages_Client folder and the config to your documents directory.
    
            -- When adding a server to the launcher config file, the settings you will be looking for client 7.18 are:
                    -- PatchTable  : 212257
                    -- HookTable   : 213435
            -- The rest of the information can be set yourself, IP, port, Ect.

One more thing, run launcher as administrator if it fails, darkages will also need to run as **administrator**.


## Formulas

Formulas are in the works, but contributes are welcome! Please see the formulas directory as part of this project.

Example of Supported Excel Template:
![Alt text](https://github.com/wren11/Lorule-Dark-Ages-Server/blob/master/GitStuff/formulas/items.png?raw=true "Items Template")


## Projects


# Client Launcher (DarkagesLauncher.csproj)
    - Used to connect you and your users to a lorule server, by default the client is configured to run on localhost.
    - Requires Administrator Privlages, 
    
# Darkages (Darkages.csproj)
    - This is the Main Server Interface, It references the main server library and act's as a front-end to (Darkages.Server)
    
# Darkages.Server (Darkages.Server.csproj)
    - This is the Main Server Library, This contains a ServerConext that acts as a wrapper to Server Instance.
   
   For instance if one wanted to create a service proxy, an example of creating a new service instance if
   desired can be done using this pattern:
   
```cs   
        public class ObjectClient : NetworkClient
        {
            public Proxy ProxyServer = new Proxy();
            public class Proxy : ServerContext
            {

            }
        }

        public class ObjectServer : NetworkServer<ObjectClient>
        {
            public ObjectServer() : base(1000)
            {

            }

            public override void Abort()
            {

            }

            public override void ClientDataReceived(ObjectClient client, NetworkPacket packet)
            {
                var objs = client.GetObjects<Monster>(i => true);

            }

            public override void ClientDisconnected(ObjectClient client)
            {
            }

            public override void ClientConnected(ObjectClient client)
            {
                Console.WriteLine("Object Server Connected.");
            }
        }  
```cs   


This would allow you to interface to the object service from an independant server.
and be able to handle packets send and received from a running lorule server.
        
An example use for this would be a messaging proxy, for relaying communication between aislings across different servers.
Accessing objects Across Multple Servers, ect
        
        
        
        


