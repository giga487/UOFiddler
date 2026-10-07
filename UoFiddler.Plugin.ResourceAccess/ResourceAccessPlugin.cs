using UoFiddler.Controls.Plugin;
using UoFiddler.Controls.Plugin.Interfaces;

namespace UoFiddler.Plugin.ResourceAccess;

public sealed class ResourceAccessPlugin : PluginBase
{
    private ResourceAccessControl? _control;
    private TabPage? _page;

    public override IPluginHost Host { get; set; } = null!;
    public override string Name => "Resource Access";
    public override string Description => "Query the active profile's resources by ID or byte range through a local read-only connection.";
    public override string Author => "SeaHats";
    public override string Version => "1.0.0";

    public override void Initialize()
    {
        _control = new ResourceAccessControl();
    }

    public override void ModifyTabPages(TabControl tabControl)
    {
        _page = new TabPage("Resource Access") { Tag = tabControl.TabCount + 1 };
        _page.Controls.Add(_control ??= new ResourceAccessControl());
        tabControl.TabPages.Add(_page);
    }

    public override void ModifyPluginToolStrip(ToolStripDropDownButton toolStrip)
    {
        var item = new ToolStripMenuItem("Resource Access");
        item.Click += (_, _) =>
        {
            if (_page?.Parent is TabControl tabs)
            {
                tabs.SelectedTab = _page;
            }
        };
        toolStrip.DropDownItems.Add(item);
    }

    public override void Unload()
    {
        _control?.Dispose();
        _control = null;
        _page?.Dispose();
        _page = null;
    }
}
