# ⚡ STORM FELLOWSHIP v0.0.1

[![Platform](https://img.shields.io/badge/Platform-Windows%2010%2F11%20WinUI%203-00A3FF.svg)](https://microsoft.com)
[![Framework](https://img.shields.io/badge/.NET-10%20%2F%20WindowsAppSDK%202.4-7000FF.svg)](https://dotnet.microsoft.com)
[![Version](https://img.shields.io/badge/Version-0.0.1-22C55E.svg)]()
[![License](https://img.shields.io/badge/License-MIT-blue.svg)]()

> High-performance, next-generation desktop communication, voice, video, and fellowship platform built on **WinUI 3 (Windows App SDK / .NET)**. Combines the best ultra low-latency, high-fidelity audio engine of **TeamSpeak 5** with the modern interface, rich messaging, emojis, stickers, and 1-on-1 direct calls of **Discord**.

---

## 🌟 Key Features

### 🎧 TeamSpeak-Grade Audio Engine
- **Ultra Low-Latency Voice Channels**: Opus codec streaming at 64k, 96k, 128k, and 256k bitrates.
- **Push-to-Talk (PTT) & Voice Activity Detection (VAD)**: Customizable sensitivity threshold slider with live VU-meter feedback.
- **3D Positional Spatial Audio**: Channel node positioning for immersion.
- **AI Noise Gate & Echo Cancellation**: Clean voice communication with studio clarity.
- **Audio Feedback Cues**: Sound cues for mute, unmute, deafen, user join/leave, call ringtones.

### 💬 Discord-Inspired Rich Messaging & Community
- **1-on-1 Direct Calls**: High-tech call interface with animated avatar voice rings, 7-bar dynamic equalizer audio waveform, call timer, camera & screen sharing controls.
- **Fellowships (Servers) & Groups**: Create, join via `storm://invite/...`, rename, customize icons, assign roles with granular permissions.
- **Channel Hierarchies**: Text `#`, Voice `🔊`, and Announcement `📢` channels grouped in collapsible categories.
- **Emojis & Stickers**: Custom Storm Fellowship stickers (Storm GG, Storm Hype, Storm Rage, Storm Victory) + categorized emoji picker.
- **Reactions & Quoting**: Quick emoji reactions on messages, quoted replies, pinned messages, message history.

---

## 🎨 4 Bespoke Custom Storm Themes

1. **STORM DARK** (Default): Deep navy and slate dark palette with electric cyan storm accents (`#00A3FF`).
2. **STORM NIGHT**: Pure OLED pitch-black carbon aesthetic with neon cyber cyan glow (`#00E5FF`).
3. **STORM DAY**: Clean, crisp high-contrast modern light theme with storm blue accents (`#0284C7`).
4. **STORM MIDNIGHT**: Cyberpunk neon violet and purple storm aesthetic (`#A855F7`).

---

## 🚀 Quick Start & Building

### Prerequisites
- Windows 10 (version 1809+) or Windows 11
- .NET 8 / 9 / 10 SDK with Windows App SDK 2.4+
- Visual Studio 2022 / JetBrains Rider / VS Code with C# Dev Kit

### Build from Sources
```bash
# Clone the repository
git clone https://github.com/ReiKatari/STORM_FELLOWSHIP.git
cd STORM_FELLOWSHIP

# Build solution in Release mode
dotnet build Sources/StormFellowship/StormFellowship.csproj -c Release -r win-x64 -p:WindowsPackageType=None

# Publish standalone executable
dotnet publish Sources/StormFellowship/StormFellowship.csproj -c Release -r win-x64 --self-contained true -p:WindowsPackageType=None -o E:\STORM FELLOWSHIP\Assembling
```

---

## 📦 Directory Structure

```
E:\STORM FELLOWSHIP\
├── Sources\           # Full C# WinUI 3 source code repository
│   ├── StormFellowship/          # Main WinUI 3 desktop client
│   ├── StormFellowship.Installer/# Standalone setup wizard
│   └── Assets/                   # Multi-resolution ICOs, PNGs, Avatars, Stickers
├── Assembling\        # Standalone compiled release binaries
└── Files\             # Setup installer (StormFellowshipSetup.exe) & PowerShell scripts
```

---

## 👤 Author & Credits
- **Developer**: ReiKatari
- **Repository**: [https://github.com/ReiKatari/STORM_FELLOWSHIP](https://github.com/ReiKatari/STORM_FELLOWSHIP)
