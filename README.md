<h1 align="center">STORM FELLOWSHIP ⚡</h1>

<p align="center">
  <img src="StormLogo.png" alt="STORM FELLOWSHIP Logo" width="220" />
</p>

<p align="center">
  <b>Высокопроизводительное, профессиональное десктопное приложение для общения, переписки, аудио- и видеосозвонов нового поколения на .NET 8 / WPF XAML Fluent. Включает бесплатный облачный бэкенд и синхронизацию 24/7, формы входа и регистрации, подключение по коду/ссылке (storm://invite/) и Direct LAN P2P, HD видео с веб-камеры, 100% векторную графику без черных силуэтов, игровой оверлей с отображением аватаров, Text-to-Speech (TTS), студийный Саундборд, командную панель (Ctrl+K), Windows 11 Mica/Acrylic, 32-полосный FFT-эквалайзер и сквозное E2EE шифрование.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Platform-Windows%2010%20%7C%20Windows%2011-0078D4?style=for-the-badge&logo=windows" />
  <img src="https://img.shields.io/badge/Framework-.NET%208%20LTS%20%2F%20WPF%20Fluent-00D2FF?style=for-the-badge&logo=windows-terminal" />
  <img src="https://img.shields.io/badge/Language-C%23%20%2F%20.NET-7928CA?style=for-the-badge&logo=c-sharp" />
  <img src="https://img.shields.io/badge/Version-v0.2.2-10B981?style=for-the-badge" />
  <img src="https://img.shields.io/badge/License-MIT-F59E0B?style=for-the-badge" />
</p>

---

## ⚡ О проекте

**STORM FELLOWSHIP v0.2.2** — это передовой клиент для командного, игрового и персонального общения, разработанный на базе высокопроизводительного рантайма **.NET 8 LTS** с современным интерфейсом **Fluent Glassmorphism** и глубокой интеграцией Windows 11 DWM.

### 🌟 Ключевые возможности версии 0.2.2:
1. ☁️ **Бесплатный облачный бэкенд и синхронизация (Free 24/7 Cloud)**:
   - Поддержка Supabase / Firebase Realtime DB для мгновенной 24/7 синхронизации каналов, сообщений и профилей.
   - Полноценные формы авторизации: **Вход**, **Регистрация**, **Смена аккаунта** и офлайн-гостевой режим.
2. 🔗 **Два способа совместного подключения и звонков**:
   - **Способ 1 (Инвайт-коды и ссылки)**: создание и переход по ссылкам `storm://invite/<код>` в один клик.
   - **Способ 2 (Direct LAN / VPN P2P)**: прямое P2P-соединение по IP (ZeroTier, Radmin, локальная сеть) без промежуточных серверов.
3. 📹 **HD видео с веб-камеры и локальный предпросмотр (Selfie View)**:
   - Захват и отображение видеопотока с веб-камеры с разрешением 1080p 60 FPS прямо в карточке участника звонка.
4. 🎮 **Игровой оверлей с поддержкой аватаров (DirectX/Vulkan Shift + ~)**:
   - Отображение кастомных аватаров и векторных глифов в оверлее, клик насквозь (`WS_EX_TRANSPARENT`) и быстрый чат.
5. 🎨 **100% Векторная графика без чёрных силуэтов**:
   - Векторные SVG-пиктограммы и геометрии с неоновыми градиентными контейнерами в стиле STORM SYSTEM OPTIMIZER.
6. ⚡ **Zero-Copy Audio Pipeline (AVX2/AVX-512 SIMD DSP)**:
   - Векторизация обработки звука на 256-битных AVX2 инструкциях (гейт, микширование, софт-лимитер), задержка < 2.5 мс.
7. 🎛️ **Audio Ducking & Game Audio Attenuation**:
   - Автоматическое плавное приглушение фоновых игр и музыки (10–70%) во время речи участников.
8. 🎯 **Низкоуровневые глобальные горячие клавиши (Mouse4 / Mouse5 Hook)**:
   - Перехват Push-to-Talk, Push-to-Mute и оверлея даже в полноэкранных играх с правами администратора.
9. 🎙️ **30 моделей изменения голоса (Voice Morpher FX) и Саундборд**:
   - Студийные пресеты с живым предпрослушиванием (Live Loopback) и коллекция звуковых эффектов.
10. 🔒 **P2P + E2EE сквозное шифрование**:
    - Сквозное шифрование AES-256-GCM + ECDH Curve25519 для сообщений и голосовых вызовов.
11. 📊 **32-полосный FFT Spectrum Analyzer**:
    - Частотный эквалайзер реального времени в окне вызова с физикой затухания.
12. ⚡ **Командная строка быстрого перехода (Quick Switcher `Ctrl + K`)**:
    - Мгновенный переход к любому каналу, содружеству, личному чату или настройкам.

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
│   │   ├── Services/                       # Аудио, Камера, Облако, E2EE, TTS, Саундборд
│   │   ├── ViewModels/                     # MVVM-архитектура
│   │   ├── Views/                          # XAML-представления, оверлей, палитры
│   │   └── Themes/                         # Темы оформления и векторные ресурсы
│   ├── StormFellowship.Installer/          # Мастер установки приложения
│   └── StormFellowship.slnx                # Файл решения .NET
├── Assembling\                             # Готовая к запуску скомпилированная сборка
│   ├── StormFellowship.exe                 # Исполняемый файл программы
│   └── Assets/ & Redist/                   # Встроенные ресурсы и нативные DLL
├── Files\                                  # Пакеты установки для пользователей
│   ├── STORM_FELLOWSHIP_0.2.2_setup.exe    # Графический мастер установки
│   ├── STORM_FELLOWSHIP_0.2.2.zip          # Портативный релизный архив
│   ├── Install.ps1                         # Скрипт быстрой автоматической установки
│   └── Uninstall.ps1                       # Скрипт полного удаления
├── Run_STORM_FELLOWSHIP.cmd                # Лаунчер приложения со снятием блокировок Windows
└── Run_Setup.cmd                           # Лаунчер мастера установки
```

---

## 🛠️ Установка и запуск

### Вариант 1: Быстрая установка через установщик
Запустите `Run_Setup.cmd` или `STORM_FELLOWSHIP_0.2.2_setup.exe` из папки `Files`, либо выполните команду в PowerShell:
```powershell
powershell -ExecutionPolicy Bypass -File "E:\STORM FELLOWSHIP\Files\Install.ps1"
```

### Вариант 2: Портативный запуск без установки
Запустите `Run_STORM_FELLOWSHIP.cmd` в корневой папке или напрямую `StormFellowship.exe` из папки `E:\STORM FELLOWSHIP\Assembling\`.

---

## 👤 Автор и контакты

- **Разработчик**: [ReiKatari](https://github.com/ReiKatari)
- **Репозиторий проекта**: [https://github.com/ReiKatari/STORM_FELLOWSHIP](https://github.com/ReiKatari/STORM_FELLOWSHIP)
- **Версия**: `0.2.2` (Релиз)
