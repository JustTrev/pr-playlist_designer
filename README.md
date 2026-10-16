# pr-playlist_designer

**About:**
A simple UI to manage your Halo 3 dedicated server playlists, a JSON editor for Project Reclaimer.

Hosting a dedicated server using a '_CustomPlaylist_.playlist.json' file requires 3 or more maps to be added. 

You can assign any dedicated server to a specific playlist you create and set the type of game mode you want, with any parameters you want with it. 

Simply use this tool to quickly create a playlist to your liking, and start hosting!

_You can edit any playlist.json file reclaimer uses with this app._   
<img width="1484" height="987" alt="image" src="https://github.com/user-attachments/assets/49ce19e6-c473-442e-9542-7aa9b6894b39" />
<img width="1083" height="736" alt="image" src="https://github.com/user-attachments/assets/1d87ee5f-676c-40da-bd97-25fdb1bb6f2f" />




**How-to-Use PR Playlist Designer:**

To use this applet, get started by clicking 'New' to construct your favorite playlist.

Set the necessary information you want, add a map, and set the gametype.

<img width="1059" height="642" alt="image" src="https://github.com/user-attachments/assets/f8b2c8a2-4754-4ff9-b693-0105c4803562" />


Once you have a game type selected you can click 'Save Game Type' to your playlist on the left.


Duplicate to create multiple versions of the same game type, or create new game types. Once you have 3 or more, you can save your playlist.

'Save Playlist' will write all of your settings as '[yourPlaylistName].playlist.json'.  These are NOT to be used with Game Type json files (ex. "faction-battlefront.json").



Use 'Load' to open a compatible '[PlaylistName].playlist.json' file. 

It's best to save custom playlists in either "c:\reclaimer\playlists" or in any appropriate location under "c:\reclaimer\" reflected in your dedicated server configuration file. 


Note:
You can read/write to any location as long as you properly point your dedicated server to the location of your saved playlist file. 
 'c:\reclaimer\playlists' <- is the default location when hosting a dedicated server. You can change this in the dedicated server settings if your file is located elsewhere.


<img width="790" height="730" alt="image" src="https://github.com/user-attachments/assets/f39ccf7f-3eaf-4460-98a1-81604b8f776c" />


**How-to-Use Game Type Editor:**

To use this applet, inside the Game Editor window, click 'Browse' for 'Game Type' and locate one of the built in templates in 'c:\reclaimer\content\Game Modes\Project Reclaimer\' to get started (There is no default path it will open to at this time). When loading a file you have two choices, either a .bin file or JSON file.

Bin files are associated with Halo 3's built in hopper system, where as the json file allow users to fully customize any map for Halo 3 as an alternative to these bin files (you are unable to edit bin files at this time).

Use 'Load' to open a compatible JSON files. 

The 'Save' button will write all of your settings as '[yourGameTypeName].json' to disk.  These are NOT to be used with PR Playlist Designer json files (ex. "[yourPlaylistName].playlist.json"). 

Please make sure you save these in a practical location for your dedicated server to work properly.


Note:
It's best to save your playlist in either "c:\reclaimer\content\Game Modes\" or "c:\reclaimer\content\Game Modes\Project Reclaimer\" for your dedicated server. 
 'c:\reclaimer\playlists' <- is the default location when hosting a dedicated server. You can change this in the dedicated server settings if your file is located elsewhere.






**To-Do:**

• Add support to decompile .bin files.

• Add server designer to build your server configuration file.

• Add H3 Bungie .bins from the x360 era.

• Add RCON Manager to view any and all servers.

• Add Server Deployment support.

• Add support to open and edit Reach Megalo files for backwards compatibility.
