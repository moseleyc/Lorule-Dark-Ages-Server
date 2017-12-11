# Lorule-Dark-Ages-Server
out-of-the-box Sand box Server For Dark Ages Client 7.18

![Alt text](https://github.com/wren11/Lorule-Dark-Ages-Server/blob/master/GitStuff/pictures/lorule.png?raw=true "Server")

![Alt text](https://github.com/wren11/Lorule-Dark-Ages-Server/blob/master/GitStuff/pictures/lorule2.png?raw=true "In Game")



## About This Project:
A Darkages Server Project - For client 7.18 Only. (Customized Client will be available soon! (Major Changes!))


## Project Design Overview.
- Json based storage and cache
- SingleInstance Object Manager Service and Object Management with cache support.
- Configurable From UI
- Scriptable Storage System/Content
- Component driven design pattern.

## Server Development Status:
- Server is currently about 90 percent completed until it is content ready.
- Expect Glithes! Help by testing and reporting them!

## Working Features
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Monsters, Items, Npcs
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Spells/Skills
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Equipment
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Shops, Dialogs
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Maps, Zones
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Death Handling, you can die, revive. ect
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Elements, Fire, Water, Earth, Wind, Dark, Light
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Regen
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Player PVP, Arena is working with a NPC for reviving.
- ![#eeeccc](https://placehold.it/15/ecceee/000000?text=+) Assails, Ambush, Crasher, Kelb, Wff, pramh - all working



## What is missing?
- Alot, but to name a few:
- ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) Meta Database is not implemented yet.
- ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) Scriptables need to be refractored to be more maintainable.
- ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) Grouping, Exchanging, User List Order - Yet to be completed!
- ![#f03c15](https://placehold.it/15/f03c15/000000?text=+) 0x30 Dialogs are abit buggy, but shops, options, skills and spell learning dialogs are working correctly.

## Using this Server:

1) Download and Compile the Lorule Source code, It should compile out-of-the-box
2) Download and Install Dark Ages Client 7.18
    -> https://drive.google.com/file/d/1EbIf7AzQLJaUrR9Kd3wmZDQWM7qT0-hR/view?usp=sharing    
3) Download our modified Darkages.exe executable and replace the previous one in the 7.18 Installation Directory.
    -> https://drive.google.com/file/d/18fCo2kyL1pF6QPJ9TgO1PIGmfKn5Fm_D/view?usp=sharing
    
That is all you need to run the server on your local system.
To host your server online, you must use the Client Launcher for your users to connect to your IP Address.
Your IPAddress also needs to reflect the server.tbl file located in your Server's running directory.

Lorule Client Launcher Download:
    -> https://drive.google.com/file/d/16Uff3R23Qdg3qRiaMbCG2q1szgfCj4LG/view?usp=sharing
    -> Copy the Darkages_Client folder and the config to your documents directory.
    
    Wehn adding a server to the launcher config file, the settings you will be looking for clint 7.18 are:
    PatchTable: 212257
    HookTable: 213435
    The rest of the information you can set yourself, IP, port, Ect.


