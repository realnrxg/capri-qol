# Capri Desktop

Rework of [Capri Desktop](https://github.com/lachydotmcg/capri-desktop).

## Changes

1. VoiceStudio integration for local voices.
2. Mac and Linux builds paused.
3. Charm meter added to Mainframe.
4. Mischief mode added to Mainframe.
5. Installer location picker added.
6. `Uninstall.exe` fixed.
7  X now closes to the tray.

## VoiceStudio

1. Open VoiceStudio and load a voice.
2. Open `Mainframe → VOICE → FREE VOICE`.
3. Select `VoiceStudio…`.
4. Enter the profile ID and select `Connect`.

API: `http://127.0.0.1:3900`

## Build

```powershell
.\\ci\\build-windows.ps1 -SmokeTest
```

Output:

- `out\\windows\\Capri.exe`
- `out\\windows\\Capri-Setup.exe`

## Smoke Test of the latest build

Latest result:

- 182 PASS
- 0 FAIL
- 3 LIMIT/manual checks

Limits:

- Physical mouse/tray UI not manually tested.
- Real Windows microphone dictation not tested.
- Actual install/uninstall not performed.

## Links

- [Original README](https://github.com/lachydotmcg/capri-desktop/blob/main/README.md)
- [VoiceStudio documentation](https://voicestudio.sh/docs/quickstart)
- [License](LICENSE.md)
