﻿using System;
using System.Collections.Generic;
using System.Numerics;
using BatteryGauge.Base;
using BatteryGauge.Battery;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Style;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace BatteryGauge.UI.Windows; 

public class SettingsWindow : Window {
    public const string WindowKey = "BatteryGauge Settings";

    private static readonly Dictionary<ChargingDisplayMode, string> ChargingDisplayModeStrings = new() {
        {ChargingDisplayMode.Hide, "Hide"},
        {ChargingDisplayMode.PercentageOnly, "Show Percentage Only"},
        {ChargingDisplayMode.TextOnly, "Show \"Charging\" Only"},
        {ChargingDisplayMode.TextPercentage, "Show \"Charging\" and Percentage"}
    };

    private static readonly Dictionary<DischargingDisplayMode, string> DischargingDisplayModeStrings = new() {
        {DischargingDisplayMode.Hide, "Hide"},
        {DischargingDisplayMode.PercentageOnly, "Show Percentage Only"},
        {DischargingDisplayMode.RuntimeOnly, "Show Runtime Only"},
        {DischargingDisplayMode.PercentageRuntime, "Show Percentage and Runtime"}
    };

    private readonly BatteryGauge _plugin = BatteryGauge.Instance;
    private readonly PluginConfig _config = BatteryGauge.Instance.Configuration;

    public SettingsWindow() : base(WindowKey, ImGuiWindowFlags.None, true) {
        this.IsOpen = true;

        this.SizeCondition = ImGuiCond.FirstUseEver;
        this.SizeConstraints = new WindowSizeConstraints {
            MinimumSize = new Vector2(350, 250),
            MaximumSize = new Vector2(350, 250)
        };
        this.Size = this.SizeConstraints.Value.MinimumSize;
    }

    public override void OnOpen() {

    }

    public override void OnClose() {
        this._plugin.WindowSystem.RemoveWindow(this);
    }
    
    public override void Draw() {
        /* Battery Warning */
        if (!SystemPower.HasBattery) {
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.DalamudRed);
            ImGui.PushTextWrapPos();
            ImGui.Text("This device either does not have a battery, or the battery is in an error state!");
            ImGui.PopTextWrapPos();
            ImGui.PopStyleColor();
        }

        /* Selector Content */
        if (ImGui.BeginCombo("Charge Mode", ChargingDisplayModeStrings[this._config.ChargingDisplayMode])) {
            foreach (var mode in Enum.GetValues<ChargingDisplayMode>()) {
                var selected = (mode == this._config.ChargingDisplayMode);
                if (ImGui.Selectable(ChargingDisplayModeStrings[mode], selected)) {
                    this._config.ChargingDisplayMode = mode;
                    this._config.Save();
                }

                if (selected) {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
        if (ImGui.Checkbox("Hide When Battery Charged", ref this._config.HideWhenFull)) {
            this._config.Save();
        }

        ImGui.Dummy(new Vector2(0, 10));
        
        if (ImGui.BeginCombo("Discharge Mode", DischargingDisplayModeStrings[this._config.DischargingDisplayMode])) {
            foreach (var mode in Enum.GetValues<DischargingDisplayMode>()) {
                var selected = (mode == this._config.DischargingDisplayMode);
                if (ImGui.Selectable(DischargingDisplayModeStrings[mode], selected)) {
                    this._config.DischargingDisplayMode = mode;
                    this._config.Save();
                }

                if (selected) {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }

        if (Injections.PluginInterface.IsDev || Injections.PluginInterface.IsDevMenuOpen) {
            ImGui.Dummy(new Vector2(0, 10));

            ImGui.Text($"Battery Percentage: {SystemPower.ChargePercentage}%");
            ImGui.Text($"Limetime (sec): {SystemPower.LifetimeSeconds}");
            ImGui.Text($"Charging: {SystemPower.IsCharging}");
            ImGui.Text($"Has Battery: {SystemPower.HasBattery}");
        }
    }
    
}