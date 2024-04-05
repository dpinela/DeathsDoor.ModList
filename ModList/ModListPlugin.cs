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
            foreach (var entry in entries)
            {
                entry.Update();
            }
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
    private string baseText;
    private ModStatus lastStatus = ModStatus.Unknown;

    public string Name => mod.Metadata.Name;
    public MUI.Elements.TextObject Entry => listEntry;

    public ModListEntry(Bep.PluginInfo mod, MUI.Core.LayoutRoot root)
    {
        this.mod = mod;
        listEntry = new(root, "Mod List Entry " + mod.Metadata.GUID);
        baseText = $"{mod.Metadata.Name}: {mod.Metadata.Version}";
        listEntry.Text = baseText;
        listEntry.FontSize = fontSize;
    }

    private ModStatus GetStatus()
    {
        if (mod.Instance == null)
        {
            return ModStatus.Unknown;
        }
        var statusProp = mod.Instance.GetType().GetProperty("InitStatus", typeof(int));
        if (statusProp == null)
        {
            return ModStatus.Unknown;
        }
        return (ModStatus)(int)statusProp.GetValue(mod.Instance);
    }

    public void Update()
    {
        if (lastStatus != ModStatus.Unknown)
        {
            return;
        }
        lastStatus = GetStatus();
        listEntry.Text = lastStatus switch
        {
            ModStatus.Unknown => baseText,
            ModStatus.InitFailed => baseText + " - INIT FAILED",
            ModStatus.InitOK => baseText,
            _ => baseText + $"{baseText} - STATUS CODE {(int)lastStatus}"
        };
    }
}

internal enum ModStatus
{
    Unknown,
    InitOK,
    InitFailed
}
