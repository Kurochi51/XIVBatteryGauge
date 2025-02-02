﻿using System;
using System.Linq;
using BatteryGauge.Base;
using BatteryGauge.UI;
using BatteryGauge.UI.Windows;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin;

namespace BatteryGauge;

public class BatteryGauge : IDalamudPlugin {
    internal static BatteryGauge Instance = null!;
    
    public string Name => "BatteryGauge";
    
    public DalamudPluginInterface PluginInterface { get; init; }
    public PluginConfig Configuration { get; init; }
    public WindowSystem WindowSystem { get; init; }

    private BatteryDtrBar BatteryDtrBar { get; init; }
    

    public BatteryGauge(DalamudPluginInterface pluginInterface) {
        pluginInterface.Create<Injections>();

        Instance = this;

        this.PluginInterface = pluginInterface;
        this.Configuration = this.PluginInterface.GetPluginConfig() as PluginConfig ?? new PluginConfig();
        this.Configuration.Initialize(this.PluginInterface);

        this.WindowSystem = new WindowSystem(this.Name);
        this.PluginInterface.UiBuilder.Draw += this.WindowSystem.Draw;
        this.PluginInterface.UiBuilder.OpenConfigUi += this.DrawConfigUI;
        
        this.BatteryDtrBar = new BatteryDtrBar();
    }

    public void Dispose() {
        this.BatteryDtrBar.Dispose();
        this.WindowSystem.RemoveAllWindows();

        this.PluginInterface.UiBuilder.OpenConfigUi -= this.DrawConfigUI;
        this.PluginInterface.UiBuilder.Draw -= this.WindowSystem.Draw;

        GC.SuppressFinalize(this);
    }

    private void DrawConfigUI() {
        var instance = this.WindowSystem.Windows.Any(window => window.WindowName == SettingsWindow.WindowKey);
        
        if (!instance) {
            this.WindowSystem.AddWindow(new SettingsWindow());
        }
    }
}