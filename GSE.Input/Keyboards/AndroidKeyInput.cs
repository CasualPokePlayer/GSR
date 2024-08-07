// Copyright (c) 2024 CasualPokePlayer
// SPDX-License-Identifier: MPL-2.0

using System;
using System.Collections.Frozen;
using System.Collections.Generic;

namespace GSE.Input.Keyboards;

internal sealed class AndroidKeyInput : IKeyInput
{
	// ReSharper disable UnusedMember.Local
	private enum AKeyCode
	{
		UNKNOWN,
		SOFT_LEFT,
		SOFT_RIGHT,
		HOME,
		BACK,
		CALL,
		ENDCALL,
		_0,
		_1,
		_2,
		_3,
		_4,
		_5,
		_6,
		_7,
		_8,
		_9,
		STAR,
		POUND,
		DPAD_UP,
		DPAD_DOWN,
		DPAD_LEFT,
		DPAD_RIGHT,
		DPAD_CENTER,
		VOLUME_UP,
		VOLUME_DOWN,
		POWER,
		CAMERA,
		CLEAR,
		A,
		B,
		C,
		D,
		E,
		F,
		G,
		H,
		I,
		J,
		K,
		L,
		M,
		N,
		O,
		P,
		Q,
		R,
		S,
		T,
		U,
		V,
		W,
		X,
		Y,
		Z,
		COMMA,
		PERIOD,
		ALT_LEFT,
		ALT_RIGHT,
		SHIFT_LEFT,
		SHIFT_RIGHT,
		TAB,
		SPACE,
		SYM,
		EXPLORER,
		ENVELOPE,
		ENTER,
		DEL,
		GRAVE,
		MINUS,
		EQUALS,
		LEFT_BRACKET,
		RIGHT_BRACKET,
		BACKSLASH,
		SEMICOLON,
		APOSTROPHE,
		SLASH,
		AT,
		NUM,
		HEADSETHOOK,
		FOCUS,
		PLUS,
		MENU,
		NOTIFICATION,
		SEARCH,
		MEDIA_PLAY_PAUSE,
		MEDIA_STOP,
		MEDIA_NEXT,
		MEDIA_PREVIOUS,
		MEDIA_REWIND,
		MEDIA_FAST_FORWARD,
		MUTE,
		PAGE_UP,
		PAGE_DOWN,
		PICTSYMBOLS,
		SWITCH_CHARSET,
		BUTTON_A,
		BUTTON_B,
		BUTTON_C,
		BUTTON_X,
		BUTTON_Y,
		BUTTON_Z,
		BUTTON_L1,
		BUTTON_R1,
		BUTTON_L2,
		BUTTON_R2,
		BUTTON_THUMBL,
		BUTTON_THUMBR,
		BUTTON_START,
		BUTTON_SELECT,
		BUTTON_MODE,
		ESCAPE,
		FORWARD_DEL,
		CTRL_LEFT,
		CTRL_RIGHT,
		CAPS_LOCK,
		SCROLL_LOCK,
		META_LEFT,
		META_RIGHT,
		FUNCTION,
		SYSRQ,
		BREAK,
		MOVE_HOME,
		MOVE_END,
		INSERT,
		FORWARD,
		MEDIA_PLAY,
		MEDIA_PAUSE,
		MEDIA_CLOSE,
		MEDIA_EJECT,
		MEDIA_RECORD,
		F1,
		F2,
		F3,
		F4,
		F5,
		F6,
		F7,
		F8,
		F9,
		F10,
		F11,
		F12,
		NUM_LOCK,
		NUMPAD_0,
		NUMPAD_1,
		NUMPAD_2,
		NUMPAD_3,
		NUMPAD_4,
		NUMPAD_5,
		NUMPAD_6,
		NUMPAD_7,
		NUMPAD_8,
		NUMPAD_9,
		NUMPAD_DIVIDE,
		NUMPAD_MULTIPLY,
		NUMPAD_SUBTRACT,
		NUMPAD_ADD,
		NUMPAD_DOT,
		NUMPAD_COMMA,
		NUMPAD_ENTER,
		NUMPAD_EQUALS,
		NUMPAD_LEFT_PAREN,
		NUMPAD_RIGHT_PAREN,
		VOLUME_MUTE,
		INFO,
		CHANNEL_UP,
		CHANNEL_DOWN,
		ZOOM_IN,
		ZOOM_OUT,
		TV,
		WINDOW,
		GUIDE,
		DVR,
		BOOKMARK,
		CAPTIONS,
		SETTINGS,
		TV_POWER,
		TV_INPUT,
		STB_POWER,
		STB_INPUT,
		AVR_POWER,
		AVR_INPUT,
		PROG_RED,
		PROG_GREEN,
		PROG_YELLOW,
		PROG_BLUE,
		APP_SWITCH,
		BUTTON_1,
		BUTTON_2,
		BUTTON_3,
		BUTTON_4,
		BUTTON_5,
		BUTTON_6,
		BUTTON_7,
		BUTTON_8,
		BUTTON_9,
		BUTTON_10,
		BUTTON_11,
		BUTTON_12,
		BUTTON_13,
		BUTTON_14,
		BUTTON_15,
		BUTTON_16,
		LANGUAGE_SWITCH,
		MANNER_MODE,
		_3D_MODE,
		CONTACTS,
		CALENDAR,
		MUSIC,
		CALCULATOR,
		ZENKAKU_HANKAKU,
		EISU,
		MUHENKAN,
		HENKAN,
		KATAKANA_HIRAGANA,
		YEN,
		RO,
		KANA,
		ASSIST,
		BRIGHTNESS_DOWN,
		BRIGHTNESS_UP,
		MEDIA_AUDIO_TRACK,
		SLEEP,
		WAKEUP,
	}

	private static readonly FrozenDictionary<AKeyCode, ScanCode> _keyCodeMap = new Dictionary<AKeyCode, ScanCode>
	{
		[AKeyCode.HOME] = ScanCode.SC_BROWSERHOME,
		[AKeyCode.BACK] = ScanCode.SC_BROWSERBACK,
		[AKeyCode._0] = ScanCode.SC_0,
		[AKeyCode._1] = ScanCode.SC_1,
		[AKeyCode._2] = ScanCode.SC_2,
		[AKeyCode._3] = ScanCode.SC_3,
		[AKeyCode._4] = ScanCode.SC_4,
		[AKeyCode._5] = ScanCode.SC_5,
		[AKeyCode._6] = ScanCode.SC_6,
		[AKeyCode._7] = ScanCode.SC_7,
		[AKeyCode._8] = ScanCode.SC_8,
		[AKeyCode._9] = ScanCode.SC_9,
		[AKeyCode.DPAD_UP] = ScanCode.SC_UP,
		[AKeyCode.DPAD_DOWN] = ScanCode.SC_DOWN,
		[AKeyCode.DPAD_LEFT] = ScanCode.SC_LEFT,
		[AKeyCode.DPAD_RIGHT] = ScanCode.SC_RIGHT,
		[AKeyCode.VOLUME_UP] = ScanCode.SC_VOLUMEUP,
		[AKeyCode.VOLUME_DOWN] = ScanCode.SC_VOLUMEDOWN,
		[AKeyCode.POWER] = ScanCode.SC_POWER,
		[AKeyCode.A] = ScanCode.SC_A,
		[AKeyCode.B] = ScanCode.SC_B,
		[AKeyCode.C] = ScanCode.SC_C,
		[AKeyCode.D] = ScanCode.SC_D,
		[AKeyCode.E] = ScanCode.SC_E,
		[AKeyCode.F] = ScanCode.SC_F,
		[AKeyCode.G] = ScanCode.SC_G,
		[AKeyCode.H] = ScanCode.SC_H,
		[AKeyCode.I] = ScanCode.SC_I,
		[AKeyCode.J] = ScanCode.SC_J,
		[AKeyCode.K] = ScanCode.SC_K,
		[AKeyCode.L] = ScanCode.SC_L,
		[AKeyCode.M] = ScanCode.SC_M,
		[AKeyCode.N] = ScanCode.SC_N,
		[AKeyCode.O] = ScanCode.SC_O,
		[AKeyCode.P] = ScanCode.SC_P,
		[AKeyCode.Q] = ScanCode.SC_Q,
		[AKeyCode.R] = ScanCode.SC_R,
		[AKeyCode.S] = ScanCode.SC_S,
		[AKeyCode.T] = ScanCode.SC_T,
		[AKeyCode.U] = ScanCode.SC_U,
		[AKeyCode.V] = ScanCode.SC_V,
		[AKeyCode.W] = ScanCode.SC_W,
		[AKeyCode.X] = ScanCode.SC_X,
		[AKeyCode.Y] = ScanCode.SC_Y,
		[AKeyCode.Z] = ScanCode.SC_Z,
		[AKeyCode.COMMA] = ScanCode.SC_COMMA,
		[AKeyCode.PERIOD] = ScanCode.SC_PERIOD,
		[AKeyCode.ALT_LEFT] = ScanCode.SC_LEFTALT,
		[AKeyCode.ALT_RIGHT] = ScanCode.SC_RIGHTALT,
		[AKeyCode.SHIFT_LEFT] = ScanCode.SC_LEFTSHIFT,
		[AKeyCode.SHIFT_RIGHT] = ScanCode.SC_RIGHTSHIFT,
		[AKeyCode.TAB] = ScanCode.SC_TAB,
		[AKeyCode.SPACE] = ScanCode.SC_SPACEBAR,
		[AKeyCode.ENVELOPE] = ScanCode.SC_MAIL,
		[AKeyCode.ENTER] = ScanCode.SC_ENTER,
		[AKeyCode.DEL] = ScanCode.SC_BACKSPACE,
		[AKeyCode.GRAVE] = ScanCode.SC_GRAVE,
		[AKeyCode.MINUS] = ScanCode.SC_MINUS,
		[AKeyCode.EQUALS] = ScanCode.SC_EQUALS,
		[AKeyCode.LEFT_BRACKET] = ScanCode.SC_LEFTBRACKET,
		[AKeyCode.RIGHT_BRACKET] = ScanCode.SC_RIGHTBRACKET,
		[AKeyCode.BACKSLASH] = ScanCode.SC_BACKSLASH,
		[AKeyCode.SEMICOLON] = ScanCode.SC_SEMICOLON,
		[AKeyCode.APOSTROPHE] = ScanCode.SC_APOSTROPHE,
		[AKeyCode.SLASH] = ScanCode.SC_SLASH,
		[AKeyCode.HEADSETHOOK] = ScanCode.SC_MEDIASELECT,
		[AKeyCode.MENU] = ScanCode.SC_APPS,
		[AKeyCode.SEARCH] = ScanCode.SC_BROWSERSEARCH,
		[AKeyCode.MEDIA_PLAY_PAUSE] = ScanCode.SC_PLAYPAUSE,
		[AKeyCode.MEDIA_STOP] = ScanCode.SC_STOP,
		[AKeyCode.MEDIA_NEXT] = ScanCode.SC_NEXTTRACK,
		[AKeyCode.MEDIA_PREVIOUS] = ScanCode.SC_PREVTRACK,
		[AKeyCode.PAGE_UP] = ScanCode.SC_PAGEUP,
		[AKeyCode.PAGE_DOWN] = ScanCode.SC_PAGEDOWN,
		[AKeyCode.ESCAPE] = ScanCode.SC_ESCAPE,
		[AKeyCode.FORWARD_DEL] = ScanCode.SC_DELETE,
		[AKeyCode.CTRL_LEFT] = ScanCode.SC_LEFTCONTROL,
		[AKeyCode.CTRL_RIGHT] = ScanCode.SC_RIGHTCONTROL,
		[AKeyCode.CAPS_LOCK] = ScanCode.SC_CAPSLOCK,
		[AKeyCode.SCROLL_LOCK] = ScanCode.SC_SCROLLLOCK,
		[AKeyCode.META_LEFT] = ScanCode.SC_LEFTGUI,
		[AKeyCode.META_RIGHT] = ScanCode.SC_RIGHTGUI,
		[AKeyCode.SYSRQ] = ScanCode.SC_PRINTSCREEN,
		[AKeyCode.BREAK] = ScanCode.SC_PAUSE,
		[AKeyCode.MOVE_HOME] = ScanCode.SC_HOME,
		[AKeyCode.MOVE_END] = ScanCode.SC_END,
		[AKeyCode.INSERT] = ScanCode.SC_INSERT,
		[AKeyCode.FORWARD] = ScanCode.SC_BROWSERFORWARD,
		[AKeyCode.F1] = ScanCode.SC_F1,
		[AKeyCode.F2] = ScanCode.SC_F2,
		[AKeyCode.F3] = ScanCode.SC_F3,
		[AKeyCode.F4] = ScanCode.SC_F4,
		[AKeyCode.F5] = ScanCode.SC_F5,
		[AKeyCode.F6] = ScanCode.SC_F6,
		[AKeyCode.F7] = ScanCode.SC_F7,
		[AKeyCode.F8] = ScanCode.SC_F8,
		[AKeyCode.F9] = ScanCode.SC_F9,
		[AKeyCode.F10] = ScanCode.SC_F10,
		[AKeyCode.F11] = ScanCode.SC_F11,
		[AKeyCode.F12] = ScanCode.SC_F12,
		[AKeyCode.NUM_LOCK] = ScanCode.SC_NUMLOCK,
		[AKeyCode.NUMPAD_0] = ScanCode.SC_NUMPAD0,
		[AKeyCode.NUMPAD_1] = ScanCode.SC_NUMPAD1,
		[AKeyCode.NUMPAD_2] = ScanCode.SC_NUMPAD2,
		[AKeyCode.NUMPAD_3] = ScanCode.SC_NUMPAD3,
		[AKeyCode.NUMPAD_4] = ScanCode.SC_NUMPAD4,
		[AKeyCode.NUMPAD_5] = ScanCode.SC_NUMPAD5,
		[AKeyCode.NUMPAD_6] = ScanCode.SC_NUMPAD6,
		[AKeyCode.NUMPAD_7] = ScanCode.SC_NUMPAD7,
		[AKeyCode.NUMPAD_8] = ScanCode.SC_NUMPAD8,
		[AKeyCode.NUMPAD_9] = ScanCode.SC_NUMPAD9,
		[AKeyCode.NUMPAD_DIVIDE] = ScanCode.SC_DIVIDE,
		[AKeyCode.NUMPAD_MULTIPLY] = ScanCode.SC_MULTIPLY,
		[AKeyCode.NUMPAD_SUBTRACT] = ScanCode.SC_SUBSTRACT,
		[AKeyCode.NUMPAD_ADD] = ScanCode.SC_ADD,
		[AKeyCode.NUMPAD_DOT] = ScanCode.SC_DECIMAL,
		[AKeyCode.NUMPAD_COMMA] = ScanCode.SC_SEPARATOR,
		[AKeyCode.NUMPAD_ENTER] = ScanCode.SC_NUMPADENTER,
		[AKeyCode.NUMPAD_EQUALS] = ScanCode.SC_NUMPADEQUALS,
		[AKeyCode.VOLUME_MUTE] = ScanCode.SC_MUTE,
		[AKeyCode.BOOKMARK] = ScanCode.SC_BROWSERFAVORITES,
		[AKeyCode.CALCULATOR] = ScanCode.SC_CALCULATOR,
		[AKeyCode.ZENKAKU_HANKAKU] = ScanCode.SC_F24,
		[AKeyCode.MUHENKAN] = ScanCode.SC_INTL5,
		[AKeyCode.HENKAN] = ScanCode.SC_INTL4,
		[AKeyCode.KATAKANA_HIRAGANA] = ScanCode.SC_INTL2,
		[AKeyCode.YEN] = ScanCode.SC_INTL3,
		[AKeyCode.RO] = ScanCode.SC_INTL1,
		[AKeyCode.KANA] = ScanCode.SC_LANG3,
		[AKeyCode.SLEEP] = ScanCode.SC_SLEEP,
		[AKeyCode.WAKEUP] = ScanCode.SC_WAKE,
	}.ToFrozenDictionary();

	private readonly List<KeyEvent> _keyEvents = [];
	private readonly object _keyEventLock = new();

	public void DispatchKeyEvent(int keycode, bool pressed)
	{
		var key = _keyCodeMap.GetValueOrDefault((AKeyCode)keycode);
		if (key == 0)
		{
			return;
		}

		lock (_keyEventLock)
		{
			_keyEvents.Add(new(key, pressed));
		}
	}

	public void Dispose()
	{
	}

	public IEnumerable<KeyEvent> GetEvents()
	{
		lock (_keyEventLock)
		{
			var ret = new KeyEvent[_keyEvents.Count];
			_keyEvents.CopyTo(ret.AsSpan());
			_keyEvents.Clear();
			return ret;
		}
	}

	private static readonly FrozenDictionary<ScanCode, string> _scanCodeStrMap = new Dictionary<ScanCode, string>
	{
		[ScanCode.SC_BROWSERHOME] = "Browser Home",
		[ScanCode.SC_BROWSERBACK] = "Back",
		[ScanCode.SC_0] = "0",
		[ScanCode.SC_1] = "1",
		[ScanCode.SC_2] = "2",
		[ScanCode.SC_3] = "3",
		[ScanCode.SC_4] = "4",
		[ScanCode.SC_5] = "5",
		[ScanCode.SC_6] = "6",
		[ScanCode.SC_7] = "7",
		[ScanCode.SC_8] = "8",
		[ScanCode.SC_9] = "9",
		[ScanCode.SC_UP] = "Up",
		[ScanCode.SC_DOWN] = "Down",
		[ScanCode.SC_LEFT] = "Left",
		[ScanCode.SC_RIGHT] = "Right",
		[ScanCode.SC_VOLUMEUP] = "Volume Up",
		[ScanCode.SC_VOLUMEDOWN] = "Volume Down",
		[ScanCode.SC_POWER] = "Power",
		[ScanCode.SC_A] = "A",
		[ScanCode.SC_B] = "B",
		[ScanCode.SC_C] = "C",
		[ScanCode.SC_D] = "D",
		[ScanCode.SC_E] = "E",
		[ScanCode.SC_F] = "F",
		[ScanCode.SC_G] = "G",
		[ScanCode.SC_H] = "H",
		[ScanCode.SC_I] = "I",
		[ScanCode.SC_J] = "J",
		[ScanCode.SC_K] = "K",
		[ScanCode.SC_L] = "L",
		[ScanCode.SC_M] = "M",
		[ScanCode.SC_N] = "N",
		[ScanCode.SC_O] = "O",
		[ScanCode.SC_P] = "P",
		[ScanCode.SC_Q] = "Q",
		[ScanCode.SC_R] = "R",
		[ScanCode.SC_S] = "S",
		[ScanCode.SC_T] = "T",
		[ScanCode.SC_U] = "U",
		[ScanCode.SC_V] = "V",
		[ScanCode.SC_W] = "W",
		[ScanCode.SC_X] = "X",
		[ScanCode.SC_Y] = "Y",
		[ScanCode.SC_Z] = "Z",
		[ScanCode.SC_COMMA] = "Comma",
		[ScanCode.SC_PERIOD] = "Period",
		[ScanCode.SC_LEFTALT] = "Left Alt",
		[ScanCode.SC_RIGHTALT] = "Right Alt",
		[ScanCode.SC_LEFTSHIFT] = "Left Shift",
		[ScanCode.SC_RIGHTSHIFT] = "Right Shift",
		[ScanCode.SC_TAB] = "Tab",
		[ScanCode.SC_SPACEBAR] = "Spacebar",
		[ScanCode.SC_MAIL] = "Mail",
		[ScanCode.SC_ENTER] = "Enter",
		[ScanCode.SC_BACKSPACE] = "Backspace",
		[ScanCode.SC_GRAVE] = "Tilde",
		[ScanCode.SC_MINUS] = "Minus",
		[ScanCode.SC_EQUALS] = "Equals",
		[ScanCode.SC_LEFTBRACKET] = "Left Bracket",
		[ScanCode.SC_RIGHTBRACKET] = "Right Bracket",
		[ScanCode.SC_BACKSLASH] = "Pipe",
		[ScanCode.SC_SEMICOLON] = "Semicolon",
		[ScanCode.SC_APOSTROPHE] = "Quotes",
		[ScanCode.SC_SLASH] = "Question",
		[ScanCode.SC_MEDIASELECT] = "Media",
		[ScanCode.SC_APPS] = "Menu",
		[ScanCode.SC_BROWSERSEARCH] = "Search",
		[ScanCode.SC_PLAYPAUSE] = "Play/Pause",
		[ScanCode.SC_STOP] = "Stop",
		[ScanCode.SC_NEXTTRACK] = "Next Track",
		[ScanCode.SC_PREVTRACK] = "Prev Track",
		[ScanCode.SC_PAGEUP] = "Page Up",
		[ScanCode.SC_PAGEDOWN] = "Page Down",
		[ScanCode.SC_ESCAPE] = "Escape",
		[ScanCode.SC_DELETE] = "Delete",
		[ScanCode.SC_LEFTCONTROL] = "Left Control",
		[ScanCode.SC_RIGHTCONTROL] = "Right Control",
		[ScanCode.SC_CAPSLOCK] = "Caps Lock",
		[ScanCode.SC_SCROLLLOCK] = "Scroll Lock",
		[ScanCode.SC_LEFTGUI] = "Left Meta",
		[ScanCode.SC_RIGHTGUI] = "Right Meta",
		[ScanCode.SC_PRINTSCREEN] = "Print Screen",
		[ScanCode.SC_PAUSE] = "Pause",
		[ScanCode.SC_HOME] = "Home",
		[ScanCode.SC_END] = "End",
		[ScanCode.SC_INSERT] = "Insert",
		[ScanCode.SC_BROWSERFORWARD] = "Forward",
		[ScanCode.SC_F1] = "F1",
		[ScanCode.SC_F2] = "F2",
		[ScanCode.SC_F3] = "F3",
		[ScanCode.SC_F4] = "F4",
		[ScanCode.SC_F5] = "F5",
		[ScanCode.SC_F6] = "F6",
		[ScanCode.SC_F7] = "F7",
		[ScanCode.SC_F8] = "F8",
		[ScanCode.SC_F9] = "F9",
		[ScanCode.SC_F10] = "F10",
		[ScanCode.SC_F11] = "F11",
		[ScanCode.SC_F12] = "F12",
		[ScanCode.SC_NUMLOCK] = "Num Lock",
		[ScanCode.SC_NUMPAD0] = "Numpad 0",
		[ScanCode.SC_NUMPAD1] = "Numpad 1",
		[ScanCode.SC_NUMPAD2] = "Numpad 2",
		[ScanCode.SC_NUMPAD3] = "Numpad 3",
		[ScanCode.SC_NUMPAD4] = "Numpad 4",
		[ScanCode.SC_NUMPAD5] = "Numpad 5",
		[ScanCode.SC_NUMPAD6] = "Numpad 6",
		[ScanCode.SC_NUMPAD7] = "Numpad 7",
		[ScanCode.SC_NUMPAD8] = "Numpad 8",
		[ScanCode.SC_NUMPAD9] = "Numpad 9",
		[ScanCode.SC_DIVIDE] = "Divide",
		[ScanCode.SC_MULTIPLY] = "Multiply",
		[ScanCode.SC_SUBSTRACT] = "Substract",
		[ScanCode.SC_ADD] = "Add",
		[ScanCode.SC_DECIMAL] = "Decimal",
		[ScanCode.SC_SEPARATOR] = "Separator",
		[ScanCode.SC_NUMPADENTER] = "Numpad Enter",
		[ScanCode.SC_NUMPADEQUALS] = "Numpad Equals",
		[ScanCode.SC_MUTE] = "Mute",
		[ScanCode.SC_BROWSERFAVORITES] = "Favorites",
		[ScanCode.SC_CALCULATOR] = "Calculator",
		[ScanCode.SC_F24] = "Lang 5",
		[ScanCode.SC_INTL5] = "Intl 5",
		[ScanCode.SC_INTL4] = "Intl 4",
		[ScanCode.SC_INTL2] = "Intl 2",
		[ScanCode.SC_INTL3] = "Intl 3",
		[ScanCode.SC_INTL1] = "Intl 1",
		[ScanCode.SC_LANG3] = "Lang 3",
		[ScanCode.SC_SLEEP] = "Sleep",
		[ScanCode.SC_WAKE] = "Wake",
	}.ToFrozenDictionary();

	public string ConvertScanCodeToString(ScanCode key)
	{
		return _scanCodeStrMap.GetValueOrDefault(key);
	}
}
