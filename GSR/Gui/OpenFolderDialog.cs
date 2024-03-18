using System;
#if GSR_WINDOWS
using System.Runtime.InteropServices;
#endif

#if GSR_WINDOWS
using Windows.Win32;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Shell;
#endif

#if GSR_OSX
using AppKit;
#endif

namespace GSR.Gui;

/// <summary>
/// IMPORTANT! This can only be used on the GUI thread (on Windows this should be the only STA thread)
/// </summary>
internal static class OpenFolderDialog
{
#if GSR_WINDOWS
	public static unsafe string ShowDialog(string description, string baseDir, ImGuiWindow mainWindow)
	{
		if (!OperatingSystem.IsWindowsVersionAtLeast(6, 0, 6000))
		{
			return null;
		}

		// the newer IFileDialog must be used, as the older APIs will not give newer Vista style dialogs
		if (PInvoke.CoCreateInstance<IFileOpenDialog>(
			    rclsid: IFileOpenDialog.IID_Guid,
			    pUnkOuter: null,
			    dwClsContext: CLSCTX.CLSCTX_ALL,
			    ppv: out var fileDialog).Failed)
		{
			return null;
		}

		try
		{
			fileDialog->SetTitle($"Select {description} Folder");
			fileDialog->SetOptions(FILEOPENDIALOGOPTIONS.FOS_NOCHANGEDIR | FILEOPENDIALOGOPTIONS.FOS_PICKFOLDERS | FILEOPENDIALOGOPTIONS.FOS_PATHMUSTEXIST);

			if (PInvoke.SHCreateItemFromParsingName(baseDir ?? AppContext.BaseDirectory, null, in IShellItem.IID_Guid, out var ppv).Succeeded)
			{
				var folder = (IShellItem*)ppv;
				try
				{
					fileDialog->SetDefaultFolder(folder);
				}
				finally
				{
					folder->Release();
				}
			}

			fileDialog->Show(new(mainWindow.SdlSysWMInfo.info.win.window));

			IShellItem* result;
			fileDialog->GetResult(&result);
			try
			{
				result->GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out var path);
				try
				{
					return new(path);
				}
				finally
				{
					Marshal.FreeCoTaskMem((IntPtr)path.Value);
				}
			}
			finally
			{
				result->Release();
			}
		}
		catch
		{
			return null;
		}
		finally
		{
			fileDialog->Release();
		}
	}
#endif

#if GSR_OSX
	public static string ShowDialog(string description, string baseDir, ImGuiWindow mainWindow)
	{
		_ = mainWindow;
		using var keyWindow = NSApplication.SharedApplication.KeyWindow;
		try
		{
			using var dialog = NSOpenPanel.OpenPanel;
			dialog.AllowsMultipleSelection = false;
			dialog.CanChooseDirectories = true;
			dialog.CanCreateDirectories = true;
			dialog.CanChooseFiles = false;
			dialog.Title = $"Select {description} Folder";
			dialog.DirectoryUrl = new(baseDir ?? AppContext.BaseDirectory);
			return (NSModalResponse)dialog.RunModal() == NSModalResponse.OK ? dialog.Url.Path : null;
		}
		catch
		{
			return null;
		}
		finally
		{
			keyWindow.MakeKeyAndOrderFront(null);
		}
	}
#endif

#if GSR_LINUX
	public static string ShowDialog(string description, string baseDir, ImGuiWindow mainWindow)
	{
		// select folder dialogs are not available in portal until version 3
		if (PortalFileChooser.IsAvailable && PortalFileChooser.Version >= 3)
		{
			try
			{
				using var portal = new PortalFileChooser();
				using var openQuery = portal.CreateSelectFolderQuery(description, baseDir ?? AppContext.BaseDirectory, mainWindow);
				return portal.RunQuery(openQuery);
			}
			catch (Exception ex)
			{
				Console.Error.WriteLine(ex);
				// we'll only mark portal as "unavailable" if the gtk file chooser is available
				// just in case something oddly goes wrong with the portal and yet still be usable
				if (GtkFileChooser.IsAvailable)
				{
					Console.WriteLine("Portal file chooser assumed to be unavailable, falling back on GTK file chooser");
					PortalFileChooser.IsAvailable = false;
				}
			}
		}

		if (GtkFileChooser.IsAvailable)
		{
			using var dialog = new GtkFileChooser($"Select {description} Folder", GtkFileChooser.FileChooserAction.SelectFolder);
			dialog.AddButton("_Cancel", GtkFileChooser.Response.Cancel);
			dialog.AddButton("_Select", GtkFileChooser.Response.Accept);
			dialog.SetCurrentFolder(baseDir ?? AppContext.BaseDirectory);
			return dialog.RunDialog() == GtkFileChooser.Response.Accept ? dialog.GetFilename() : null;
		}

		return null;
	}
#endif
}