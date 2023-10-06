using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using JetBrains.Annotations;
using ShowGear.Windows;
using CSFramework = FFXIVClientStructs.FFXIV.Client.System.Framework.Framework;

namespace ShowGear;

[PublicAPI]
public sealed class Plugin : IDalamudPlugin
{
    private DalamudPluginInterface PluginInterface { get; init; }
    private IFramework Framework { get; init; }

    public Configuration Configuration { get; init; }
    public readonly WindowSystem WindowSystem = new("ShowGear");

    private readonly byte prevTryonValue;
    private readonly byte prevColorantValue;
    private readonly byte prevMiragePrismMiragePlateValue;
    private static int TryonOffset => 0x2f1; // 753
    private static int ColorantOffset => 0x3d9; // 985
    private static int MiragePrismMiragePlateOffset => 0x344; // 836

    public unsafe Plugin(
        [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
        [RequiredVersion("1.0")] IFramework framework)
    {
        PluginInterface = pluginInterface;
        Framework = framework;

        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
        Configuration.Initialize(PluginInterface);
        ConfigWindow = new ConfigWindow(this);
        WindowSystem.AddWindow(ConfigWindow);
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

    public ConfigWindow ConfigWindow { get; init; }

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
        ConfigWindow.IsOpen = true;
    }

    private unsafe void OnFrameworkUpdate(IFramework framework)
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
