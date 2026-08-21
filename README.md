<h1 align="center">STORM FELLOWSHIP ⚡</h1>

<p align="center">
  <img src="StormLogo.png" alt="STORM FELLOWSHIP Logo" width="220" />
</p>

<p align="center">
  <b>Высокопроизводительное, профессиональное десктопное приложение для общения, переписки, аудио- и видеосозвонов нового поколения на .NET / WPF XAML Fluent. Включает унифицированный дизайн иконок в стиле STORM SYSTEM OPTIMIZER (градиентные микро-контейнеры, неоновые подложки), Text-to-Speech (TTS) озвучивание сообщений, студийный Саундборд (Soundboard FX), командную панель быстрого перехода (Quick Switcher Ctrl+K), идеально симметричные карточки участников звонка, Mesh Gradient Flow, Windows 11 Mica/Acrylic, 32-полосный FFT-эквалайзер и сквозное шифрование E2EE.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows%2010%20%7C%20Windows%2011-0078D4?style=for-the-badge&logo=windows" />
  <img src="https://img.shields.io/badge/Framework-.NET%208%20LTS%20%2F%20WPF%20Fluent-00D2FF?style=for-the-badge&logo=windows-terminal" />
  <img src="https://img.shields.io/badge/Language-C%23%20%2F%20.NET-7928CA?style=for-the-badge&logo=c-sharp" />
  <img src="https://img.shields.io/badge/Version-v0.1.0-10B981?style=for-the-badge" />
  <img src="https://img.shields.io/badge/License-MIT-F59E0B?style=for-the-badge" />
</p>

---

## ⚡ О проекте

**STORM FELLOWSHIP v0.1.0** — это передовой клиент для командного, игрового и персонального общения, разработанный на базе высокопроизводительного рантайма **.NET 8 LTS** с современным интерфейсом **Fluent Glassmorphism** и глубокой интеграцией Windows 11 DWM.

### 🌟 Ключевые возможности версии 0.1.0:
1. 🎨 **Единый графический стиль STORM SYSTEM OPTIMIZER**:
   - Градиентные микро-контейнеры для иконок (`IconGradCyan`, `IconGradPurple`, `IconGradEmerald`, `IconGradAmber`, `IconGradSky`) с неоновыми полупрозрачными подложками (`#1A00D2FF`).
   - Исправлено отображение иконки приложения в окнах, панели задач, трее и заголовках.
2. 🔊 **Встроенный голосовой синтезатор Text-to-Speech (TTS)**:
   - Озвучивание любого сообщения в чате по правому клику мыши («🔊 Озвучить сообщение»).
3. 📐 **Идеальная симметрия окон участников в звонке**:
   - Окна «Собеседник» и «Пользователь (Вы)» имеют абсолютно равные пропорции (`280x220`), одинаковые аватары (`84x84`), одинаковые шрифты и отступы.
4. 🎵 **Студийный Саундборд (Soundboard FX)**:
   - Панель воспроизведения звуковых эффектов в звонок (*Airhorn, GG WP, Аплодисменты, STORM Гром, Лазер, Победный фанфар*).
5. ⚡ **Командная строка быстрого перехода (Quick Switcher `Ctrl + K`)**:
   - Мгновенный переход к любому каналу, содружеству, личному чату или настройкам с клавиатуры.
6. 🌊 **Mesh Gradient Flow**: живые переливающиеся неоновые градиенты в шапке и окне звонка.
7. 🪟 **Windows 11 Mica / Acrylic Backdrop**: нативная полупрозрачность DWM (Mica Alt).
8. 💡 **Glow Voice Pulse**: динамический многослойный неоновый ореол активности голоса вокруг аватаров.
9. 👾 **Интерактивный стикер- и эмодзи-пикер**: быстрый поиск и каталог анимированных стикеров STORM.
10. 📊 **32-полосный FFT Spectrum Analyzer**: частотный эквалайзер реального времени в окне вызова с физикой затухания.
11. 📁 **Compact Sidebar Dock**: сворачивание сайдбара каналов в узкий док (56px) с всплывающими подсказками.
12. 💬 **Floating Glass Bubbles**: режим отображения сообщений в виде плавающих стеклянных пузырей.
13. 📈 **Live Network Quality Sparkline**: микро-график пинга и стабильности сети в реальном времени.
14. 🎮 **Игровой оверлей (DirectX 11/12 / Vulkan)**: оверлей поверх полноэкранных игр (`Shift + ~`).
15. 🔒 **P2P + E2EE сквозное шифрование**: шифрование AES-256-GCM + ECDH Curve25519.
16. 🔊 **Индивидуальный микшер громкости участников**: регулировка громкости каждого участника (0–200%).
17. 🤖 **AI Шумоподавление (RNNoise / DeepFilterNet)**: фильтрация кликов и шумов с нулевой задержкой.
18. 🖥️ **Трансляция экрана со звуком приложений (WASAPI Loopback)**: 1080p/4K 60/120 FPS.
19. 👑 **Система ролей и прав содружеств (RBAC)**: создание и настройка ролей с цветными бейджами.
20. 🤖 **Whisper AI расшифровка голосовых сообщений**: локальная транскрипция аудио в 1 клик.
21. 🔍 **Мгновенный локальный поиск (SQLite FTS5)**: полнотекстовый поиск (`Ctrl + F`).

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
│   │   ├── Services/                       # Аудио-движок, E2EE, TTS, Саундборд, FFT
│   │   ├── ViewModels/                     # MVVM-архитектура
│   │   ├── Views/                          # XAML-представления, оверлей, палитры
│   │   └── Themes/                         # Ресурсы тем оформления и векторных иконок SSO-стиля
│   ├── StormFellowship.Installer/          # Мастер установки приложения
│   └── StormFellowship.slnx                # Файл решения .NET
├── Assembling\                             # Готовая к запуску скомпилированная сборка
│   ├── StormFellowship.exe                 # Исполняемый файл программы
│   └── Assets/ & Redist/                   # Встроенные ресурсы и нативные DLL
├── Files\                                  # Пакеты установки для пользователей
│   ├── STORM_FELLOWSHIP_0.1.0_setup.exe    # Графический мастер установки
│   ├── Install.ps1                         # Скрипт быстрой автоматической установки
│   └── Uninstall.ps1                       # Скрипт полного удаления
├── Run_STORM_FELLOWSHIP.cmd                # Лаунчер приложения со снятием блокировок Windows
└── Run_Setup.cmd                           # Лаунчер мастера установки
```

---

## 🛠️ Установка и запуск

### Вариант 1: Быстрая установка через установщик
Запустите `Run_Setup.cmd` или `STORM_FELLOWSHIP_0.1.0_setup.exe` из папки `Files`, либо выполните команду в PowerShell:
```powershell
powershell -ExecutionPolicy Bypass -File "E:\STORM FELLOWSHIP\Files\Install.ps1"
```

### Вариант 2: Портативный запуск без установки
Запустите `Run_STORM_FELLOWSHIP.cmd` в корневой папке или напрямую `StormFellowship.exe` из папки `E:\STORM FELLOWSHIP\Assembling\`.

---

## 👤 Автор и контакты

- **Разработчик**: [ReiKatari](https://github.com/ReiKatari)
- **Репозиторий проекта**: [https://github.com/ReiKatari/STORM_FELLOWSHIP](https://github.com/ReiKatari/STORM_FELLOWSHIP)
- **Версия**: `0.1.0` (Релиз)
