# Keystroke Paster

A Windows Forms application that sends text as keystrokes to any active window, useful for pasting into VM Consoles, remote consoles, and applications where traditional paste doesn't work.

## Features

- ✅ Simple GUI with text input box
- ✅ Global hotkey trigger (default: Ctrl+F1)
- ✅ Second hotkey types the clipboard directly (default: Ctrl+F2)
- ✅ Configurable keystroke delay, remembered between launches
- ✅ System tray minimization
- ✅ Always-on-top window
- ✅ Status indicator (Waiting/Typing/Done)
- ✅ Handles special characters, newlines, tabs, spaces
- ✅ Small single executable (~50KB)

## Usage

There are two ways to send text, each with its own hotkey.

### Text box (`Ctrl+F1`)

Use this when the text needs formatting - multi-line commands, scripts, anything
you want to review or edit before it gets typed.

1. **Launch the application** - The window stays on top of other windows
2. **Paste your text** - Copy any text (commands, passwords, code, etc.) into the text box
3. **Switch to target window** - Click into the VMware console, terminal, or any target application
4. **Press the hotkey** - Default is `Ctrl+F1` (configurable in settings)
5. **Watch it type** - The status will show "Typing..." and then "Done!"

### Clipboard (`Ctrl+F2`)

Use this for quick one-offs. Copy anything, click into the target window, and press
`Ctrl+F2` - it types whatever is on the clipboard without touching the text box, so
there is no need to paste it in first. The text box keeps its contents either way,
so a command you use often can stay parked there while you fire off clipboard text.

If the clipboard is empty or holds something that isn't text (an image, a file), the
status line says so and nothing is typed.

## Settings

Click the **⚙** gear icon to configure:

- **Text box hotkey**: Choose modifier (Ctrl, Alt, Shift) + Function key (F1-F12)
- **Clipboard hotkey**: Same choices; must differ from the text box hotkey
- **Keystroke Delay**: Time between each keystroke in milliseconds
  - 0ms = Fastest (may not work on slow systems)
  - 10ms = Default (good balance)
  - 50ms+ = Safer for slow/remote systems

Settings are saved when you click OK and restored on the next launch. They live in
the registry under `HKEY_CURRENT_USER\SOFTWARE\KeystrokePaster`, so the app stays a
single portable exe with no config file beside it. Delete that key to get the
defaults back (Ctrl+F1, Ctrl+F2, 10ms).

If a stored hotkey isn't a combination the settings dialog can produce - say the key
was hand-edited - the app ignores the stored pair and falls back to the defaults
rather than starting up with no working hotkeys.

## System Tray

- Minimize the window to hide it in the system tray (near the clock)
- Double-click the tray icon to restore the window
- Right-click the tray icon for menu options

## Building

### Requirements
- Visual Studio 2017 or later
- .NET Framework 4.7.2 or later

### Build Steps

1. Open `KeystrokePaster.sln` in Visual Studio
2. Set build configuration to **Release**
3. Build > Build Solution (Ctrl+Shift+B)
4. Find the executable at: `bin\Release\KeystrokePaster.exe`

### Command Line Build

```bash
# Using MSBuild
msbuild KeystrokePaster.csproj /p:Configuration=Release

# Or using dotnet (if you have .NET SDK)
dotnet build -c Release
```

### Single EXE Output

The Release build produces a single portable executable with no dependencies (requires .NET Framework 4.7.2 installed on target system, which is included in Windows 10 1803+).

## Technical Details

- **Framework**: .NET Framework 4.7.2
- **UI**: Windows Forms
- **Keyboard Input**: SendInput API (Unicode support)
- **Global Hotkey**: RegisterHotKey API
- **Special Characters**: Full Unicode support including newlines, tabs, symbols

## Use Cases

- Pasting passwords into VMware ESX or VCSA/VirtualBox consoles
- Sending commands to remote desktop sessions
- Entering text into legacy applications
- Bypassing clipboard restrictions
- Automating repetitive text entry

## Troubleshooting

**Hotkey doesn't work:**
- Make sure the hotkey isn't already used by another application
- Try changing the hotkey in settings
- Check that a second copy of Keystroke Paster isn't already running - the first
  one to start owns the hotkey, and the second will report that it can't register it

**Text types too fast:**
- Increase the keystroke delay in settings (try 20-50ms)

**Special characters don't work:**
- This should work for all Unicode characters
- If issues persist, try increasing the delay

**Application doesn't minimize to tray:**
- Click the minimize button (not close)
- Closing the window minimizes to tray by default

## ScreenConnect / ConnectWise Control Compatibility

This tool works in ScreenConnect backstage and remote sessions. The single EXE can be transferred and run without installation.

## License

See [LICENSE](LICENSE).
