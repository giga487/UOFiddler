// /***************************************************************************
//  *
//  * $Author: Turley
//  * 
//  * "THE BEER-WARE LICENSE"
//  * As long as you retain this notice you can do whatever you want with 
//  * this stuff. If we meet some day, and you think this stuff is worth it,
//  * you can buy me a beer in return.
//  *
//  ***************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.RightsManagement;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using SeaHatsCustom;
using Ultima;
using UoFiddler.Controls.Classes;
using UoFiddler.Controls.Plugin;
using UoFiddler.Controls.Plugin.Interfaces;

namespace UoFiddler.Plugin.SeaHats
{
    public class SeaHatsPluginBase : PluginBase
    {
        SeaHatsPluginForm? _form { get; set; } = null;
        public override IPluginHost Host { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string Name => "SeaHats Custom Plugin";

        public override string Description => "Plugin for everything we need";

        public override string Author => "Giga487";

        public override string Version => "0.0.1";

        public override void Initialize()
        {
            _form = new SeaHatsPluginForm();

        }

        public override void ModifyTabPages(System.Windows.Forms.TabControl tabControl)
        {
            TabPage page = new TabPage
            {
                Tag = tabControl.TabCount + 1, // at end used for undock/dock feature to define the order
                Text = "Multi Editor"
            };

            _form = new SeaHatsPluginForm()
            {
                Dock = DockStyle.Fill
            };
            page.Controls.Add(_form);

            tabControl.TabPages.Add(page);
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }
    }
}
