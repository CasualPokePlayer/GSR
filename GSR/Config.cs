using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

using GSR.Audio;
using GSR.Emu;
using GSR.Gui;
using GSR.Input;

using static SDL2.SDL;

namespace GSR;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(Config))]
internal partial class ConfigSerializerContext : JsonSerializerContext;

internal sealed class Config
{
	public GBPlatform GbPlatform { get; set; } = GBPlatform.GBP;
	public int FastForwardSpeed { get; set; } = 4;
	public bool ApplyColorCorrection { get; set; } = true;
	public bool DisableGbaRtc { get; set; } = true;
	public bool HideSgbBorder { get; set; }
	public bool HideStatusBar { get; set; }
	public bool DisableWin11RoundCorners { get; set; }

	public string GbBiosPath { get; set; }
	public string GbcBiosPath { get; set; }
	public string Sgb2BiosPath { get; set; }
	public string GbaBiosPath { get; set; }

	public List<string> RecentRoms { get; set; } = [];

	public int SaveStateSet { get; set; }
	public int SaveStateSlot { get; set; }

	public EmuControllerBindings EmuControllerBindings { get; set; } = new();
	public HotkeyBindings HotkeyBindings { get; set; } = new();
	public bool AllowBackgroundInput { get; set; }
	public bool BackgroundInputForJoysticksOnly { get; set; }

	public bool KeepAspectRatio { get; set; } = true;
	public ScalingFilter OutputFilter { get; set; } = ScalingFilter.SharpBilinear;
	public string RenderDriver { get; set; } = ImGuiWindow.DEFAULT_RENDER_DRIVER;
	public int WindowScale { get; set; } = 3;
	public bool AllowManualResizing { get; set; }

	public string AudioDeviceName { get; set; } = AudioManager.DEFAULT_AUDIO_DEVICE;
	public int LatencyMs { get; set; } = 68;
	public int Volume { get; set; } = 100;

	[JsonConstructor]
	public Config()
	{
	}

	public void DeserializeInputBindings(InputManager inputManager, IntPtr mainWindow)
	{
		try
		{
			EmuControllerBindings.DeserializeInputBindings(inputManager);
			HotkeyBindings.DeserializeInputBindings(inputManager);
		}
		catch
		{
			_ = SDL_ShowSimpleMessageBox(
				flags: SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING,
				title: "Config load failure",
				message: "Input bindings failed to be deserizalized, default input bindings will be used instead.",
				window: mainWindow
			);

			EmuControllerBindings = new();
			EmuControllerBindings.DeserializeInputBindings(inputManager);
			HotkeyBindings = new();
			HotkeyBindings.DeserializeInputBindings(inputManager);
		}
	}

	public void SaveConfig(string configPath)
	{
		File.Delete(configPath);
		using var configFile = File.OpenWrite(configPath);
		JsonSerializer.Serialize(configFile, this, ConfigSerializerContext.Default.Config);
	}

	private void SanitizeConfig()
	{
		if (!Enum.IsDefined(GbPlatform))
		{
			GbPlatform = GBPlatform.GBP;
		}

		FastForwardSpeed = Math.Clamp(FastForwardSpeed, 2, 64);
		RecentRoms ??= [];
		SaveStateSet = Math.Clamp(SaveStateSet, 0, 9);
		SaveStateSlot = Math.Clamp(SaveStateSlot, 0, 9);

		// don't need to sanitize input bindings, since DeserializeInputBindings will revert to default bindings if something is wrong

		if (!Enum.IsDefined(OutputFilter))
		{
			OutputFilter = ScalingFilter.SharpBilinear;
		}

		RenderDriver ??= ImGuiWindow.DEFAULT_RENDER_DRIVER;
		WindowScale = Math.Clamp(WindowScale, 1, 15);
		AudioDeviceName ??= AudioManager.DEFAULT_AUDIO_DEVICE;
		LatencyMs = Math.Clamp(LatencyMs, AudioManager.MINIMUM_LATENCY_MS, AudioManager.MAXIMUM_LATENCY_MS);
		Volume = Math.Clamp(Volume, 0, 100);
	}

	public static Config LoadConfig(string configPath)
	{
		if (!File.Exists(configPath))
		{
			return new();
		}

		try
		{
			using var configFile = File.OpenRead(configPath);
			var ret = JsonSerializer.Deserialize(configFile, ConfigSerializerContext.Default.Config);
			ret.SanitizeConfig();
			return ret;
		}
		catch
		{
			_ = SDL_ShowSimpleMessageBox(
				flags: SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING,
				title: "Config load failure",
				message: "Config file failed to load, the default config will be used instead.",
				window: IntPtr.Zero
			);

			return new();
		}
	}
}
