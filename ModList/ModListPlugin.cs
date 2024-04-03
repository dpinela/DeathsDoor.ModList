using Bep = BepInEx;
using MUI = MagicUI;
using CG = System.Collections.Generic;

namespace DDoor.ModList;

[Bep.BepInPlugin("deathsdoor.modlist", "ModList", "1.0.0.0")]
[Bep.BepInDependency("deathsdoor.magicui", "1.8")]
internal class ModListPlugin : Bep.BaseUnityPlugin
{
    private MUI.Elements.StackLayout? modList;
    private CG.List<ModListEntry> entries = new();

    public void Start()
    {
        var root = new MUI.Core.LayoutRoot(true, "Mod List Display");
        modList = new(root, "Mod List");
        modList.Orientation = MUI.Core.Orientation.Vertical;
        modList.HorizontalAlignment = MUI.Core.HorizontalAlignment.Left;
        modList.VerticalAlignment = MUI.Core.VerticalAlignment.Top;
        foreach (var (name, mod) in Bep.Bootstrap.Chainloader.PluginInfos)
        {
            entries.Add(new(mod, root));
        }
        entries.Sort((a, b) => a.Name.CompareTo(b.Name));
        foreach (var entry in entries)
        {
            modList.Children.Add(entry.Entry);
        }
    }

    public void FixedUpdate()
    {
        if (modList == null)
        {
            return;
        }
        var visible = GameSceneManager.GetCurrentSceneName() == "TitleScreen";
        if (visible)
        {
            modList.Visibility = MUI.Core.Visibility.Visible;
        }
        else
        {
            modList.Visibility = MUI.Core.Visibility.Collapsed;
        }
    }
}

internal class ModListEntry
{
    private const int fontSize = 22;

    private MUI.Elements.TextObject listEntry;
    private Bep.PluginInfo mod;

    public string Name => mod.Metadata.Name;
    public MUI.Elements.TextObject Entry => listEntry;

    public ModListEntry(Bep.PluginInfo mod, MUI.Core.LayoutRoot root)
    {
        this.mod = mod;
        listEntry = new(root, "Mod List Entry " + mod.Metadata.GUID);
        listEntry.Text = $"{mod.Metadata.Name}: {mod.Metadata.Version}";
        listEntry.FontSize = fontSize;
    }
}
