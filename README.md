<h1 align="center">STORM FELLOWSHIP ⚡</h1>

<p align="center">
  <img src="StormLogo.png" alt="STORM FELLOWSHIP Logo" width="220" />
</p>

<p align="center">
  <b>Высокопроизводительное, профессиональное десктопное приложение для общения, переписки, аудио- и видеосозвонов нового поколения на .NET / WPF XAML Fluent. Включает 10 новейших визуальных инноваций: Mesh Gradient Flow, Windows 11 Mica/Acrylic, динамический Glow Voice Pulse, 3D Parallax & Elevation Cards, Emote & Sticker Picker, 32-полосный спектральный FFT-эквалайзер, Compact Sidebar Dock, Floating Glass Bubbles и живой спарклайн стабильности сети.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows%2010%20%7C%20Windows%2011-0078D4?style=for-the-badge&logo=windows" />
  <img src="https://img.shields.io/badge/Framework-.NET%208%20LTS%20%2F%20WPF%20Fluent-00D2FF?style=for-the-badge&logo=windows-terminal" />
  <img src="https://img.shields.io/badge/Language-C%23%20%2F%20.NET-7928CA?style=for-the-badge&logo=c-sharp" />
  <img src="https://img.shields.io/badge/Version-v0.0.8-10B981?style=for-the-badge" />
  <img src="https://img.shields.io/badge/License-MIT-F59E0B?style=for-the-badge" />
</p>

---

## ⚡ О проекте

**STORM FELLOWSHIP v0.0.8** — это передовой клиент для командного, игрового и персонального общения, разработанный на базе высокопроизводительного рантайма **.NET 8 LTS** с современным интерфейсом **Fluent Glassmorphism** и глубокой интеграцией Windows 11 DWM.

### 🌟 Ключевые возможности версии 0.0.8:
1. 🌊 **Mesh Gradient Flow**: живые переливающиеся неоновые градиенты в шапке и окне звонка.
2. 🪟 **Windows 11 Mica / Acrylic Backdrop**: нативная полупрозрачность DWM (Mica Alt).
3. 💡 **Glow Voice Pulse**: динамический многослойный неоновый ореол активности голоса вокруг аватаров.
4. 🎴 **3D Parallax & Elevation Cards**: эффекты плавного возвышения и световых бликов при наведении.
5. 👾 **Интерактивный стикер- и эмодзи-пикер (Sticker & Emote Picker)**: всплывающее меню с анимированными векторными стикерами STORM и эмодзи.
6. 📊 **32-полосный FFT Spectrum Analyzer**: частотный эквалайзер реального времени в окне вызова с физикой затухания.
7. 📁 **Compact Sidebar Dock**: переключение сайдбара каналов в режим узкого дока (56px) с всплывающими подсказками.
8. 🎨 **Animated Fellowship Header Banner**: градиентные баннеры содружеств с кастомизацией.
9. 💬 **Floating Glass Bubbles**: режим отображения сообщений в виде плавающих стеклянных пузырей с акцентным свечением.
10. 📈 **Live Network Quality Sparkline**: микро-график пинга и стабильности сети в реальном времени.
11. 🎮 **Игровой оверлей (DirectX 11/12 / Vulkan)**: оверлей поверх полноэкранных игр (`Shift + ~`).
12. 🔒 **P2P + E2EE сквозное шифрование**: сквозное шифрование AES-256-GCM + ECDH Curve25519.
13. 🔊 **Индивидуальный микшер громкости участников**: регулировка громкости каждого участника (0–200%).
14. 🤖 **AI Шумоподавление (RNNoise / DeepFilterNet)**: подавление щелчков клавиатуры и шума улицы с нулевой задержкой.
15. 🖥️ **Трансляция экрана со звуком приложений (WASAPI Loopback)**: захват звука игры (1080p/4K 60/120 FPS).
16. 👑 **Система ролей и прав содружеств (RBAC)**: создание и настройка ролей с цветными бейджами.
17. 🤖 **Whisper AI расшифровка голосовых сообщений**: локальная транскрипция аудиосообщений в 1 клик.
18. 🔍 **Мгновенный локальный поиск (SQLite FTS5)**: полнотекстовый поиск (`Ctrl + F`).

---

## 📦 Встроенные программы и библиотеки (Zero-Dependency)

В сборку и установщик программы **нативно встроены все необходимые рантаймы и зависимости**:
- Нативные библиотеки **Microsoft Visual C++ 2015–2026 (x64)** (`msvcp140.dll`, `vcruntime140.dll`, `concrt140.dll` и др.) для портативной работы без предварительной установки.
- Автоматический установщик **VC++ Redistributable** (`vc_redist.x64.exe`).
- Автоматический установщик **Microsoft Edge WebView2 Runtime** (`MicrosoftEdgeWebview2Setup.exe`).
- Автономный пакет **.NET 8 LTS (Self-Contained)**.

---

## 📁 Структура директорий проекта

```
E:\STORM FELLOWSHIP\
├── Sources\                                # Полный исходный код решения
│   ├── StormFellowship/                    # Основное приложение .NET / WPF Fluent
│   │   ├── Assets/                         # Иконки, аватары, стикеры, логотипы
│   │   ├── Models/                         # Модели (User, Channel, Poll, Folder, Role...)
│   │   ├── Services/                       # Аудио-движок, E2EE, Whisper, FFT, 3D
│   │   ├── ViewModels/                     # MVVM-архитектура
│   │   ├── Views/                          # XAML-представления, оверлей, пикеры
│   │   └── Themes/                         # Ресурсы тем оформления и векторных иконок
│   ├── StormFellowship.Installer/          # Мастер установки приложения
│   └── StormFellowship.slnx                # Файл решения .NET
├── Assembling\                             # Готовая к запуску скомпилированная сборка
│   ├── StormFellowship.exe                 # Исполняемый файл программы
│   └── Assets/ & Redist/                   # Встроенные ресурсы и нативные DLL
├── Files\                                  # Пакеты установки для пользователей
│   ├── STORM_FELLOWSHIP_0.0.8_setup.exe    # Графический мастер установки
│   ├── Install.ps1                         # Скрипт быстрой автоматической установки
│   └── Uninstall.ps1                       # Скрипт полного удаления
├── Run_STORM_FELLOWSHIP.cmd                # Лаунчер приложения со снятием блокировок Windows
└── Run_Setup.cmd                           # Лаунчер мастера установки
```

---

## 🛠️ Установка и запуск

### Вариант 1: Быстрая установка через установщик
Запустите `Run_Setup.cmd` или `STORM_FELLOWSHIP_0.0.8_setup.exe` из папки `Files`, либо выполните команду в PowerShell:
```powershell
powershell -ExecutionPolicy Bypass -File "E:\STORM FELLOWSHIP\Files\Install.ps1"
```

### Вариант 2: Портативный запуск без установки
Запустите `Run_STORM_FELLOWSHIP.cmd` в корневой папке или напрямую `StormFellowship.exe` из папки `E:\STORM FELLOWSHIP\Assembling\`.

---

## 👤 Автор и контакты

- **Разработчик**: [ReiKatari](https://github.com/ReiKatari)
- **Репозиторий проекта**: [https://github.com/ReiKatari/STORM_FELLOWSHIP](https://github.com/ReiKatari/STORM_FELLOWSHIP)
- **Версия**: `0.0.8` (Релиз)
