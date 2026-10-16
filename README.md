# pr-playlist_designer

**About:**
A simple UI to manage your Halo 3 dedicated server playlists, a JSON editor for Project Reclaimer.

Hosting a dedicated server using a '_CustomPlaylist_.playlist.json' file requires 3 or more maps to be added. 

You can assign any dedicated server to a specific playlist you create and set the type of game mode you want, with any parameters you want with it. 

Simply use this tool to quickly create a playlist to your liking, and start hosting!

_You can edit any playlist.json file reclaimer uses with this app._   
<img width="1484" height="987" alt="image" src="https://github.com/user-attachments/assets/49ce19e6-c473-442e-9542-7aa9b6894b39" />


 



**How-to-Use PR Playlist Designer:**


To use this applet, get started by clicking 'New' to construct your favorite playlist.


Set the necessary information you want, add a map (or type one), and select the gametype by clicking 'browse' or you can type one in yourself in the text field.

<img width="1059" height="642" alt="image" src="https://github.com/user-attachments/assets/f8b2c8a2-4754-4ff9-b693-0105c4803562" />

<img width="790" height="730" alt="image" src="https://github.com/user-attachments/assets/f39ccf7f-3eaf-4460-98a1-81604b8f776c" />

Once a map and gametype have been set, then click 'Save Game to Playlist' to add your map to the playlist on the left. Once you have a total of 3 or more maps added to the playlist, you can then click 'Save Playlist'.


<img width="1047" height="608" alt="image" src="https://github.com/user-attachments/assets/c08fee0f-e2d3-4c51-b35c-d06d7bd9c107" />



You can also duplicate these items to create multiple versions of the same game type, or create new multitudes of game variants.

'Save Playlist' will write all of your settings as '[yourPlaylistName].playlist.json'.  These are NOT to be used with Game Type json files (ex. "faction-battlefront.json").



To load a playlist, click 'Load' to open a compatible '[PlaylistName].playlist.json' file. 


<img width="986" height="657" alt="image" src="https://github.com/user-attachments/assets/61b72ea4-e7c2-44ef-9390-e735b8bdb742" />


It's best to save custom playlists in either "c:\reclaimer\playlists" or in any appropriate location under "c:\reclaimer\" (reflected in your dedicated server configuration file). 

Note:
 'c:\reclaimer\playlists' <- is the default location when hosting a dedicated server. You can change this in the dedicated server settings if your file is located elsewhere.




**How-to-Use Game Type Editor:**

To use this applet, inside the Game Editor window, click 'Browse' next to 'Game Type' and locate one of the built in JSON template files located in 'c:\reclaimer\content\Game Modes\Project Reclaimer\' to get started (There is no default path it will open to at this time).


<img width="790" height="730" alt="image" src="https://github.com/user-attachments/assets/f39ccf7f-3eaf-4460-98a1-81604b8f776c" />


_Bin files are associated with Halo 3's built in hopper system Project reclaimer will also use (you are unable to edit these bin files at this time)._

 
<img width="993" height="724" alt="image" src="https://github.com/user-attachments/assets/550cad26-966e-4d95-85d6-3b2fc91a373a" />


The 'Save As' button will write all of your settings as '[yourGameTypeName].json' to disk.  These are NOT to be used with PR Playlist Designer json files (ex. "[yourPlaylistName].playlist.json"). 

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
