using Dalamud.Configuration;
using Dalamud.Plugin;
using System;

namespace ShowGear;

[Serializable]
public class Configuration : IPluginConfiguration
{
    public int Version { get; set; } = 0;

    public bool ManageItemDyeing { get; set; } = true;

    // the below exist just to make saving less cumbersome
    // ReSharper disable once InconsistentNaming
    [NonSerialized]
    private DalamudPluginInterface? PluginInterface;

    public void Initialize(DalamudPluginInterface pluginInterface)
    {
        this.PluginInterface = pluginInterface;
    }

    public void Save()
    {
        this.PluginInterface!.SavePluginConfig(this);
    }
}
