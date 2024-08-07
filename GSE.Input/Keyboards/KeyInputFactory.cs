// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

#if GSE_LINUX
using System;
#endif

using static SDL2.SDL;

namespace GSE.Input.Keyboards;

internal static class KeyInputFactory
{
	public static IKeyInput CreateKeyInput(in SDL_SysWMinfo mainWindowWmInfo)
	{
#if GSE_WINDOWS
		return new RawKeyInput();
#endif

#if GSE_OSX
		return new QuartzKeyInput();
#endif

#if GSE_LINUX
		switch (mainWindowWmInfo.subsystem)
		{
			// if we're using a wayland window, we must use wayland apis for key input
			// as wayland typically doesn't allow for x11 to do inputs unless xwayland or similar is used
			case SDL_SYSWM_TYPE.SDL_SYSWM_WAYLAND when WlImports.HasDisplay:
				return new WlKeyInput(mainWindowWmInfo.info.wl.display);
			// in this case, we're using XWayland, which would not allow background input with the X11 backend
			// we can still do background input however, if we have root access (and thus can use evdev directly)
			// we still of course want to grab a new wayland connection just to obtain a keymap
			case SDL_SYSWM_TYPE.SDL_SYSWM_X11 when WlImports.HasDisplay && EvDevImports.HasEvDev && LibcImports.HasRoot:
				return new WlKeyInput(0);
			case SDL_SYSWM_TYPE.SDL_SYSWM_X11 when X11Imports.HasDisplay:
				return new X11KeyInput();
			case SDL_SYSWM_TYPE.SDL_SYSWM_DIRECTFB or SDL_SYSWM_TYPE.SDL_SYSWM_KMSDRM or SDL_SYSWM_TYPE.SDL_SYSWM_VIVANTE when EvDevImports.HasEvDev:
				// these subsystems just use evdev directly for keyboard input, and don't need root to use it
				return new EvDevKeyInput();
		}

		// assume that we need root to use evdev otherwise
		if (LibcImports.HasRoot && EvDevImports.HasEvDev)
		{
			return new EvDevKeyInput();
		}

		throw new NotSupportedException("Linux key input requires Wayland, X11, or rootless evdev");
#endif

#if GSE_ANDROID
		return new AndroidKeyInput();
#endif
	}
}
