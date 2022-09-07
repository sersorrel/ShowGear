# ShowGear

Shows your entire outfit by default in the Fitting Room and Item Dyeing.

## Usage

You should install this plugin through Dalamud’s plugin installer, like any other plugin. If it’s not listed, it’s probably out of date and won’t work.

## Development

### Building

1. Open up `ShowGear.sln` in your C# editor of choice (likely [Visual Studio 2022](https://visualstudio.microsoft.com) or [JetBrains Rider](https://www.jetbrains.com/rider/)).
2. Build the solution. By default, this will build a `Debug` build, but you can switch to `Release` in your IDE.
3. The resulting plugin can be found at `ShowGear/bin/x64/Debug/ShowGear.dll` (or `Release` if appropriate.)

### Activating in-game

1. Launch the game and use `/xlsettings` in chat or `xlsettings` in the Dalamud Console to open up the Dalamud settings.
    * In here, go to `Experimental`, and add the full path to `ShowGear.dll` to the list of Dev Plugin Locations.
2. Next, use `/xlplugins` (chat) or `xlplugins` (console) to open up the Plugin Installer.
    * In here, go to `Dev Tools > Installed Dev Plugins`, and `ShowGear` should be visible. Enable it.

Note that you only need to add it to the Dev Plugin Locations once (Step 1); it is preserved afterwards. You can disable, enable, or load the plugin on startup through the Plugin Installer.
