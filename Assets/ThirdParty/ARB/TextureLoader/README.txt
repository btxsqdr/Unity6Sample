TextureLoader is copyright (c) 2024 Andre Rene Biasi

Namespace:
using ARB.TextureLoader

Basic usage:
TextureLoader loader = TextureLoader.Load("http://example.com/texture.jpg").OnComplete((texture) => Debug.Log("Texture loaded"));
loader.Start();

Note that the URL for local textures must start with "file://".

Always call Dispose() or DisposeAll() methods when textures are no longer needed to avoid memory leaks.

Memory management:
• Textures are reused to ensure only a single instance is kept in memory when loading multiple instances with identical settings.
• Textures are released from memory once all associated loader instances are disposed.

Editor menu:
Window > TextureLoader

Configure the log level and default values in the Settings panel:
Window > TextureLoader > Settings

Reveal current cache folder:
Window > TextureLoader > Reveal Cache Folder

Clear current cache folder:
Window > TextureLoader > Clear Cache Folder

Website (documentation and more):
https://andres-organization-12.gitbook.io/home/unity-assets/textureloader

Removing the package:
1. Delete the folder Assets/ARB/TextureLoader.
2. Delete the file TextureLoaderSettings.asset inside Assets/Resources (if it exists).

Send bugs, feature requests and suggestions to:
andre.r.biasi@gmail.com