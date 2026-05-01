# AR Kanji Project

> Aplikasi Pembelajaran Kanji N5 Berbasis Augmented Reality

## 📦 Download

👉 [Download APK (Latest Release)](https://github.com/zakkutsu/AR_Kanji_Go/releases/latest)

---

[![Unity](https://img.shields.io/badge/Unity-2021.3+-blue.svg)](https://unity.com/)
[![Vuforia](https://img.shields.io/badge/Vuforia-Engine-green.svg)](https://developer.vuforia.com/)
[![Platform](https://img.shields.io/badge/Platform-Android%20%7C%20PC-orange.svg)]()

## 📖 Tentang Proyek

**AR Kanji Project** adalah aplikasi edukasi interaktif untuk pembelajaran karakter Kanji Bahasa Jepang level N5 (pemula). Aplikasi ini menggabungkan teknologi Augmented Reality (AR) dengan mini-games edukatif untuk memberikan pengalaman belajar yang menyenangkan dan efektif.

### ✨ Fitur Utama

- 🎯 **Mode Belajar AR** - Scan marker untuk melihat informasi Kanji 3D dengan audio pengucapan
- 📝 **Kuis Interaktif** - Pilihan ganda dengan 3 tingkat kesulitan (Easy/Medium/Hard)
- 🎮 **Game Susun Kata** - Latihan menyusun huruf Romaji dengan timer
- ⭐ **Sistem Penilaian** - Skor dan rating bintang untuk memotivasi pembelajaran
- 🔊 **Audio Support** - Pengucapan native untuk setiap Kanji
- ⏱️ **Timer Challenge** - Melatih kecepatan dan ketepatan

## 🎯 Target Pengguna

- Pelajar pemula Bahasa Jepang (level N5)
- Siapa saja yang ingin belajar Kanji dengan cara yang fun
- Penggemar teknologi AR dan gamifikasi edukasi

## 🏗️ Struktur Aplikasi

```
AR Kanji Project
│
├── Scene_MainMenu          → Menu utama & navigasi
├── Scene_MenuBelajar       → Mode AR untuk belajar Kanji
├── Scene_Quiz              → Kuis pilihan ganda
├── Scene_SusunKata         → Game menyusun kata
└── Scene_Nilai             → Tampilan hasil & skor
```

## 🎮 Cara Bermain

### Mode Belajar (AR)
1. Pilih **"BELAJAR"** dari menu utama
2. Arahkan kamera ke marker Kanji
3. Lihat informasi lengkap: Kanji, arti, onyomi, kunyomi, contoh
4. Tap tombol speaker untuk mendengar pengucapan
5. Gesek objek 3D untuk merotasi

### Mode Ujian
1. Pilih **"UJIAN"** dari menu utama
2. Pilih tingkat kesulitan:
   - **Easy** - 10 soal
   - **Medium** - 15 soal
   - **Hard** - 20 soal
3. Jawab soal pilihan ganda (timer 5 menit)
4. Lanjut ke game susun kata
5. Lihat hasil akhir & rating bintang

## 📊 Sistem Penilaian

### Perhitungan Skor
- Quiz: **10 poin** per jawaban benar
- Susun Kata: **10 poin** per kata benar
- **Total maksimal**: 20 poin × jumlah soal

### Rating Bintang
- ⭐⭐⭐ (3 bintang) - Skor ≥ 80%
- ⭐⭐ (2 bintang) - Skor ≥ 50%
- ⭐ (1 bintang) - Skor > 10%
- (0 bintang) - Skor ≤ 10%

## 🛠️ Teknologi

### Platform & Engine
- **Unity 2021.3+** - Game engine
- **Vuforia Engine** - AR tracking
- **C#** - Programming language

### Dependencies
- TextMeshPro - Japanese font rendering
- UnityEngine.UI - User interface
- Vuforia - Augmented Reality

## 📁 Struktur Project

```
Assets/
├── Scenes/                     # Unity scenes
│   ├── Scene_MainMenu.unity
│   ├── Scene_MenuBelajar.unity
│   ├── Scene_Quiz.unity
│   ├── Scene_SusunKata.unity
│   └── Scene_Nilai.unity
│
├── Scripts/                    # C# Scripts
│   ├── ARManager.cs           # AR mode manager
│   ├── QuizManager.cs         # Quiz logic
│   ├── SusunKataManager.cs    # Word puzzle logic
│   ├── ResultManager.cs       # Score display
│   ├── MainMenuManager.cs     # Menu navigation
│   ├── GameConstants.cs       # Global constants
│   ├── SoundManager.cs        # Audio controller
│   ├── SimpleTargetHandler.cs # Vuforia marker handler
│   └── TouchRotate.cs         # 3D object rotation
│
├── Resources/                  # Runtime loadable assets
│   ├── SuaraKanji/            # Kanji audio files
│   ├── GambarSoal/            # Question images
│   ├── Quiz_Easy.csv          # Easy quiz data
│   ├── Quiz_Medium.csv        # Medium quiz data
│   ├── Quiz_Hard.csv          # Hard quiz data
│   ├── Susun_Easy.csv         # Easy word puzzle data
│   ├── Susun_Medium.csv       # Medium word puzzle data
│   └── Susun_Hard.csv         # Hard word puzzle data
│
├── StreamingAssets/
│   └── DataKanji_N5.csv       # AR Kanji database
│
├── Prefabs/                    # Reusable GameObjects
├── Models/                     # 3D models
├── Fonts/                      # Japanese fonts
└── Audio/                      # Background music & SFX
```

## 🎨 Daftar Kanji yang Tersedia

| Kanji | Arti | Onyomi | Kunyomi |
|-------|------|--------|---------|
| 木 | Pohon | モク (Moku) | き (Ki) |
| 山 | Gunung | サン (San) | やま (Yama) |
| 花 | Bunga | カ (Ka) | はな (Hana) |
| 雨 | Hujan | ウ (U) | あめ (Ame) |
| 水 | Air | スイ (Sui) | みず (Mizu) |
| 火 | Api | カ (Ka) | ひ (Hi) |
| 魚 | Ikan | ギョ (Gyo) | さかな (Sakana) |
| 人 | Orang | ジン (Jin) | ひと (Hito) |
| 猫 | Kucing | ビョウ | ねこ (Neko) |
| 車 | Mobil | シャ (Sha) | くるま (Kuruma) |
| 本 | Buku | ホン (Hon) | もと (Moto) |
| 門 | Gerbang | モン (Mon) | かど (Kado) |

## 🔧 Setup & Installation

### Requirements
- Unity 2021.3 atau lebih baru
- Vuforia Engine SDK
- Android SDK (untuk build Android)

### Langkah Setup
1. Clone atau download repository
2. Buka project di Unity
3. Import Vuforia Engine package
4. Setup Vuforia license key di `VuforiaConfiguration.asset`
5. Build & Run untuk platform target (Android/PC)

### Build untuk Android
1. File → Build Settings
2. Pilih platform **Android**
3. Switch Platform
4. Player Settings:
   - Set Package Name
   - Set Minimum API Level: Android 7.0 (API 24)
   - Enable AR Required
5. Build APK

## 📱 Kontrol

### PC/Editor
- **Mouse Click** - Pilih jawaban, klik tombol
- **Mouse Drag** - Rotasi objek 3D
- **ESC** - Tombol kembali

### Android
- **Tap** - Pilih jawaban, klik tombol
- **Swipe** - Rotasi objek 3D
- **Back Button** - Tombol kembali

## 🎵 Audio Files

### Sound Effects
- **Click** - Suara klik tombol
- **Benar** - Feedback jawaban benar
- **Salah** - Feedback jawaban salah
- **Result** - Jingle hasil akhir

## 🐛 Known Issues & Limitations

- Marker AR harus cukup terang dan jelas untuk tracking optimal
- Beberapa font Jepang mungkin tidak tampil di device tertentu tanpa font fallback
- Timer tidak pause saat aplikasi background (by design)

## 🔄 Future Improvements

- [ ] Tambah lebih banyak Kanji (N4, N3)
- [ ] Mode multiplayer
- [ ] Leaderboard global
- [ ] Achievement system
- [ ] Daily challenge
- [ ] Flashcard mode

## 👥 Credits

### Development Team
Proyek ini dikembangkan menggunakan Unity dan Vuforia Engine.

### Assets & Resources
- **TextMeshPro** - Font rendering
- **Vuforia** - AR tracking engine
- **Unity Asset Store** - 3D models dan audio

## 📄 License

Proyek ini dibuat untuk tujuan edukasi.

---

**Selamat Belajar Kanji! がんばって！(Ganbatte!)** 🎌
