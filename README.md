# Keystroke Paster

A Windows Forms application that sends text as keystrokes to any active window, useful for pasting into VM Consoles, remote consoles, and applications where traditional paste doesn't work.

## Features

- ✅ Simple GUI with text input box
- ✅ Global hotkey trigger (default: Ctrl+F1)
- ✅ Configurable keystroke delay
- ✅ System tray minimization
- ✅ Always-on-top window
- ✅ Status indicator (Waiting/Typing/Done)
- ✅ Handles special characters, newlines, tabs, spaces
- ✅ Small single executable (~50KB)

## Usage

1. **Launch the application** - The window stays on top of other windows
2. **Paste your text** - Copy any text (commands, passwords, code, etc.) into the text box
3. **Switch to target window** - Click into the VMware console, terminal, or any target application
4. **Press the hotkey** - Default is `Ctrl+F1` (configurable in settings)
5. **Watch it type** - The status will show "Typing..." and then "Done!"

## Settings

Click the **⚙** gear icon to configure:

- **Hotkey**: Choose modifier (Ctrl, Alt, Shift) + Function key (F1-F12)
- **Keystroke Delay**: Time between each keystroke in milliseconds
  - 0ms = Fastest (may not work on slow systems)
  - 10ms = Default (good balance)
  - 50ms+ = Safer for slow/remote systems

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

See license.md
