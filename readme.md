# Pixelflut Games

Play [Pong](https://en.wikipedia.org/wiki/Pong) on a [Pixelflut](https://labitat.dk/wiki/Pixelflut) using a controller, works on both Linux and Windows.

Requirements:

- A Pixelflut server
- A Game Controller

## Get started

1. [Install .NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) ([or later](https://dotnet.microsoft.com/en-us/download/dotnet))
2. `cd src/PixelFlut`
3. Edit `appsettings.json` to point to the Pixelflut server's IP address and port
4. `dotnet run`
5. Make sure the Game Controller is connected

You can now control the 2 players with the Game Controller.

### Nitendo like game controller

| Input Id | Input  | Action |
| - | - | - |
| 65585 | Up | Moves left player up |
| 65585 | Down | Moves left player down |
| 589825 | X | Moves right player up |
| 589827 | B | Moves left player down |

### Playstation like game controller

| Input Id | Input  | Action |
| - | - | - |
| 65585 | Left stick | Moves left player up |
| 65585 | Left stick | Moves left player down |
| 589828 | Triangle | Moves right player up |
| 589826 | Cross | Moves left player down |

## Configuration

The configurations are stored in `src/PixelFlut/appsettings.json`.

## Links
- Official wiki: https://labitat.dk/wiki/Pixelflut 
- DO NOT TRUST THE PROTOCOL DOCUMENTATION: https://github.com/JanKlopper/pixelvloed/blob/master/protocol.md
- Only trust the server code: https://github.com/JanKlopper/pixelvloed/blob/master/C/Server/main.c 
- The server: https://github.com/JanKlopper/pixelvloed
- A example client: https://github.com/Hafpaf/pixelVloedClient 


