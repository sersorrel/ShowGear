using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ShowGear.Windows;

public class ConfigWindow : Window, IDisposable
{
    private readonly Configuration configuration;

    public ConfigWindow(Plugin plugin) : base(
        "ShowGear",
        ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoCollapse)
    {
        this.Size = new Vector2(500, 0);
        this.SizeCondition = ImGuiCond.Always;

        this.configuration = plugin.Configuration;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.TextWrapped(
            "The Item Dyeing and Glamour Plate Selection windows don’t remember the previous state of the Show Gear button, which makes them "
            + "somewhat more difficult for this plugin to deal with. When this setting is enabled, Show Gear will be "
            + "enabled every time you open Item Dyeing or Plate Selection, but if you disable Show Gear and want to re-enable it without "
            + "closing and reopening the window, you’ll have to click “Manually adjust visor” to update the model.");
        ImGui.TextWrapped(
            "If you’d rather this plugin not interfere with those windows at all, disable this option.");
        // can't ref a property, so use a local copy
        var manageItemDyeing = this.configuration.ManageItemDyeing;
        if (ImGui.Checkbox("Manage Item Dyeing/Glamour Plate Selection", ref manageItemDyeing))
        {
            this.configuration.ManageItemDyeing = manageItemDyeing;
            // save immediately on change
            this.configuration.Save();
        }
    }
}
