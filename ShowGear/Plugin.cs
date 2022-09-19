using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using ShowGear.Windows;
using CSFramework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;
using DFramework = Dalamud.Game.Framework;

namespace ShowGear;

// ReSharper disable once ClassNeverInstantiated.Global
public sealed class Plugin : IDalamudPlugin
{
    public string Name => "Show Gear";

    private DalamudPluginInterface PluginInterface { get; init; }
    private DFramework Framework { get; init; }

    public Configuration Configuration { get; init; }
    public readonly WindowSystem WindowSystem = new("ShowGear");

    private readonly byte prevTryonValue;
    private readonly byte prevColorantValue;
    private readonly byte prevMiragePrismMiragePlateValue;
    private static int TryonOffset => 0x2c9;
    private static int ColorantOffset => 0x3a9;
    private static int MiragePrismMiragePlateOffset => 0x31c;

    public unsafe Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] DFramework framework)
    {
        PluginInterface = pluginInterface;
        Framework = framework;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);
        WindowSystem.AddWindow(new ConfigWindow(this));
        PluginInterface.UiBuilder.Draw += DrawUi;
        PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUi;

        var tryonAgentAddress = GetTryonAgentAddress();
        prevTryonValue = *(tryonAgentAddress + TryonOffset);
        *(tryonAgentAddress + TryonOffset) = 0;

        var colorantAgentAddress = GetColorantAgentAddress();
        prevColorantValue = *(colorantAgentAddress + ColorantOffset);
        var miragePrismMiragePlateAgentAddress = GetMiragePrismMiragePlateAgentAddress();
        prevMiragePrismMiragePlateValue = *(miragePrismMiragePlateAgentAddress + MiragePrismMiragePlateOffset);
        Framework.Update += OnFrameworkUpdate;
    }

    public unsafe void Dispose()
    {
        Framework.Update -= OnFrameworkUpdate;
        var miragePrismMiragePlateAgentAddress = GetMiragePrismMiragePlateAgentAddress();
        *(miragePrismMiragePlateAgentAddress + MiragePrismMiragePlateOffset) = prevMiragePrismMiragePlateValue;
        var colorantAgentAddress = GetColorantAgentAddress();
        *(colorantAgentAddress + ColorantOffset) = prevColorantValue;

        var tryonAgentAddress = GetTryonAgentAddress();
        *(tryonAgentAddress + TryonOffset) = prevTryonValue;

        PluginInterface.UiBuilder.OpenConfigUi -= DrawConfigUi;
        PluginInterface.UiBuilder.Draw -= DrawUi;
        WindowSystem.RemoveAllWindows();
    }

    private void DrawUi()
    {
        WindowSystem.Draw();
    }

    private void DrawConfigUi()
    {
        WindowSystem.GetWindow("ShowGear")!.IsOpen = true;
    }

    private unsafe void OnFrameworkUpdate(DFramework framework)
    {
        // the Item Dyeing popup doesn't persist the Show Gear toggle's state, so we write it every frame
        // nor does the Plate Selection popup, so we do that one too
        if (Configuration.ManageItemDyeing)
        {
            var colorantAgentAddress = GetColorantAgentAddress();
            *(colorantAgentAddress + ColorantOffset) = 0;
            var miragePrismMiragePlateAgentAddress = GetMiragePrismMiragePlateAgentAddress();
            *(miragePrismMiragePlateAgentAddress + MiragePrismMiragePlateOffset) = 0;
        }
    }

    private static unsafe byte* GetTryonAgentAddress()
    {
        return (byte*)CSFramework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Tryon);
    }

    private static unsafe byte* GetColorantAgentAddress()
    {
        return (byte*)CSFramework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.Colorant);
    }

    private static unsafe byte* GetMiragePrismMiragePlateAgentAddress()
    {
        return (byte*)CSFramework.Instance()->GetUiModule()->GetAgentModule()->GetAgentByInternalId(AgentId.MiragePrismMiragePlate);
    }
}
