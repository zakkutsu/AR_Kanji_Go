# 📱 ANALISIS PROYEK AR KANJI PROJECT

## 🎯 GAMBARAN UMUM
Ini adalah **aplikasi pembelajaran Bahasa Jepang berbasis Augmented Reality (AR)** yang dibangun menggunakan **Unity** dan **Vuforia**. Aplikasi ini fokus pada pembelajaran Kanji level N5 (pemula) dengan pendekatan interaktif dan gamifikasi.

---

## 🏗️ ARSITEKTUR APLIKASI

Aplikasi ini memiliki **5 Scene utama** yang terhubung:

1. **Scene_MainMenu** - Menu utama aplikasi
2. **Scene_MenuBelajar** - Mode AR untuk belajar Kanji
3. **Scene_Quiz** - Kuis pilihan ganda
4. **Scene_SusunKata** - Game menyusun kata
5. **Scene_Nilai** - Tampilan hasil/score

---

## 📂 KOMPONEN KODE UTAMA

### 1. ARManager.cs - Otak Mode Belajar AR
**Fungsi:**
- Memuat data Kanji dari file CSV (DataKanji_N5.csv)
- Menampilkan informasi Kanji (karakter, arti, onyomi, kunyomi, contoh) saat marker AR terdeteksi
- Memutar audio pengucapan Kanji
- Mengelola panel UI informasi

**Struktur Data:**
```
ID -> Nama marker (Marker_Ki, Marker_Yama, dll)
Kanji -> Karakter Jepang (木, 山, 花, dll)
Arti -> Terjemahan Indonesia
Onyomi -> Pembacaan Cina
Kunyomi -> Pembacaan Jepang
Contoh -> Contoh penggunaan
AudioFile -> File suara untuk diputar
```

**Fitur Khusus:**
- Mendukung Android dan PC/Editor
- Force refresh UI menggunakan `ForceMeshUpdate()` untuk TextMeshPro
- Tombol back HP Android untuk navigasi

---

### 2. SimpleTargetHandler.cs - Pendeteksi Marker AR
**Fungsi:**
- Menggunakan Vuforia untuk mendeteksi marker AR
- Memanggil `ARManager` saat marker terdeteksi/hilang
- Tracking status: TRACKED, EXTENDED_TRACKED, NOT_FOUND

**Alur Kerja:**
```
Marker terdeteksi → ARManager.ShowKanjiData(namaMarker)
Marker hilang → ARManager.HidePanel()
```

---

### 3. QuizManager.cs - Manager Kuis Pilihan Ganda
**Fungsi:**
- Memuat soal dari CSV berdasarkan level (Easy/Medium/Hard)
- Menampilkan soal random sejumlah yang dipilih (10/15/20 soal)
- Timer per sesi (5 menit default)
- Sistem navigasi soal (Next/Prev)
- Feedback benar/salah dengan warna dan sound

**Mekanisme Skor:**
- Benar: +10 poin per soal
- Warna hijau = benar, merah = salah
- Sound effect untuk feedback
- Skor diteruskan ke Scene_SusunKata

**Fitur Menarik:**
- Timer berubah merah jika < 10 detik
- Animasi "Pop" saat jawaban dipilih
- Semua soal bisa direview (bisa back ke soal sebelumnya)

---

### 4. SusunKataManager.cs - Game Menyusun Huruf
**Fungsi:**
- Memecah kata menjadi huruf-huruf acak
- Player menyusun huruf dengan klik
- Timer 30 detik per soal
- Validasi jawaban otomatis

**Mekanisme:**
```
1. Huruf diacak → Ditampilkan sebagai tombol
2. Player klik huruf → Pindah ke slot jawaban
3. Setelah huruf cukup → Auto-check jawaban
4. Benar: +10 poin, lanjut otomatis
5. Salah/Timeout: lanjut tanpa poin
```

**Estafet Skor:**
- Menerima skor dari QuizManager
- Menambah skor dari game ini
- Total dikirim ke ResultManager

---

### 5. ResultManager.cs - Tampilan Hasil
**Fungsi:**
- Menampilkan total skor dari Quiz + Susun Kata
- Sistem bintang (0-3 ⭐):
  - 3 bintang: ≥80% benar
  - 2 bintang: ≥50% benar  
  - 1 bintang: >10% benar
  - 0 bintang: ≤10% benar

**Fitur:**
- Animasi pop untuk skor
- Sound effect "jeng-jeng"
- Opsi ulangi atau ke menu
- Panel konfirmasi sebelum keluar

---

### 6. MainMenuManager.cs - Kontrol Menu Utama
**Fungsi:**
- Navigasi ke mode AR atau mode Quiz
- Pemilihan level (Easy/Medium/Hard)
- Panel info tentang aplikasi
- Konfirmasi keluar aplikasi

**Flow Pemilihan:**
```
Tombol MAIN → Panel Mode
├─ BELAJAR → Scene AR langsung
└─ UJIAN → Panel Level
   ├─ Easy (10 soal)
   ├─ Medium (15 soal)
   └─ Hard (20 soal) → Scene_Quiz
```

---

### 7. GameConstants.cs - Pusat Konfigurasi
**Isi:**
- Nama scene (string constants)
- PlayerPrefs keys (untuk menyimpan data)
- Level names dan jumlah soal
- Poin sistem (10 per soal Quiz + 10 per soal Susun)
- Timer settings
- Warna UI (hijau/merah/putih)
- Helper functions (hitung bintang, konversi level, dll)

**Manfaat:**
- Tidak ada hardcode string
- Mudah maintenance
- Satu tempat untuk semua konstanta

---

### 8. SoundManager.cs - Singleton Audio
**Fungsi:**
- Persistent across scenes (DontDestroyOnLoad)
- Memisahkan BGM dan SFX
- Koleksi sound:
  - Click
  - Benar (correct answer)
  - Salah (wrong answer)
  - Result (jeng-jeng di akhir)

**Pola Singleton:**
```csharp
if (Instance == null) Instance = this;
else Destroy(gameObject);
```

---

### 9. TouchRotate.cs - Interaksi 3D Object
**Fungsi:**
- Memutar objek 3D dengan drag/swipe
- Rotasi sumbu Y (horizontal)
- Rotasi sumbu X (vertical)
- Dipakai untuk objek AR yang muncul

---

## 📊 ALUR APLIKASI LENGKAP

```
START
  ↓
[MAIN MENU]
  ├─ Tombol Info → Panel About
  ├─ Tombol Main → Panel Mode
  │   ├─ BELAJAR → [SCENE AR]
  │   │   └─ Scan Marker → Tampil Info Kanji + Audio + 3D Object
  │   └─ UJIAN → Panel Level
  │       └─ Pilih Level → [SCENE QUIZ]
  │           ├─ Quiz 10/15/20 soal
  │           ├─ Timer 5 menit
  │           ├─ Dapat Skor → Simpan
  │           └─ Selesai → [SCENE SUSUN KATA]
  │               ├─ Susun huruf 10/15/20 soal
  │               ├─ Timer 30 detik per soal
  │               ├─ Tambah Skor
  │               └─ Selesai → [SCENE NILAI]
  │                   ├─ Tampil Total Skor
  │                   ├─ Tampil Bintang (0-3)
  │                   └─ Opsi: Ulangi / Menu
  └─ Tombol Quit → Konfirmasi → Keluar App
```

---

## 🎮 SISTEM GAMIFIKASI

### 1. Skor Bertahap (Estafet)
- Quiz: 10 poin × jumlah benar
- Susun Kata: 10 poin × jumlah benar
- Total maksimal: 20 × jumlah soal

### 2. Timer dengan Pressure
- Quiz: 5 menit untuk semua soal
- Susun Kata: 30 detik per soal
- Timer berubah merah saat < 10 detik (visual warning)

### 3. Feedback Instant
- Sound benar/salah
- Warna hijau/merah
- Animasi pop
- Kunci jawaban ditampilkan

### 4. Sistem Bintang
- Motivasi untuk score lebih tinggi
- Feedback visual pencapaian

---

## 💾 PERSISTENSI DATA

Menggunakan **PlayerPrefs** untuk menyimpan:
```
TargetSoal → Jumlah soal yang dipilih
LevelDipilih → Easy/Medium/Hard
SkorSementara → Skor dari Quiz (diteruskan ke Susun)
SkorTerakhir → Total skor akhir (untuk Result)
```

**Cleanup:** Data di-clear saat kembali ke menu utama atau mulai sesi baru.

---

## 🎨 ELEMEN UI KHUSUS

1. **TextMeshPro** - Font rendering Jepang yang baik
2. **Force UI Refresh** - `ForceMeshUpdate()` untuk update teks Kanji
3. **Panel System** - SetActive untuk show/hide panel
4. **Button Listeners** - RemoveAllListeners sebelum add new (prevent memory leak)
5. **Animasi Pop** - Coroutine untuk scale animation

---

## 🔊 AUDIO SYSTEM

1. **Kanji Audio** - File .wav/.mp3 di Resources/SuaraKanji/
2. **SFX** - Click, Benar, Salah, Result
3. **PlayOneShot** - Tidak interrupt BGM

---

## 📱 MOBILE OPTIMIZATION

1. **Back Button Handler** - ESC key untuk Android
2. **CSV Loading** - UnityWebRequest untuk Android, File.ReadAllText untuk PC
3. **Konfirmasi Keluar** - Prevent accidental quit
4. **Panel Konfirmasi** - Untuk setiap aksi penting

---

## 🛠️ FITUR TEKNIS MENARIK

1. **Singleton Pattern** - ARManager, SoundManager
2. **Coroutine Animation** - Smooth pop effect
3. **Event-Driven AR** - Vuforia callback untuk marker
4. **CSV Parsing** - Dynamic data loading
5. **Randomization** - Soal dan huruf diacak
6. **Closure dalam Lambda** - Proper index capture untuk button listeners
7. **Memory Management** - RemoveAllListeners, Destroy unused objects

---

## 🎯 KESIMPULAN

Aplikasi ini adalah **educational game** yang well-structured dengan:
- ✅ **Separation of Concerns** - Setiap manager punya tugas jelas
- ✅ **Reusability** - GameConstants, SoundManager bisa dipakai semua scene
- ✅ **User Experience** - Feedback instant, animasi, sound
- ✅ **Scalability** - Data dari CSV, mudah tambah soal/kanji
- ✅ **Mobile-Ready** - Handle Android back button, responsive UI
- ✅ **Gamification** - Timer, score, bintang, estafet antar scene

**Target Audience:** Pemula yang ingin belajar Kanji N5 dengan cara fun dan interaktif melalui AR dan mini-games.

---

## 📋 DAFTAR FILE SCRIPT

| File | Lokasi | Fungsi Utama |
|------|--------|--------------|
| ARManager.cs | Assets/Scripts/ | Manager mode AR, load data Kanji |
| SimpleTargetHandler.cs | Assets/Scripts/ | Deteksi marker Vuforia |
| QuizManager.cs | Assets/Scripts/ | Manager kuis pilihan ganda |
| SusunKataManager.cs | Assets/Scripts/ | Manager game susun huruf |
| ResultManager.cs | Assets/Scripts/ | Tampilan hasil & bintang |
| MainMenuManager.cs | Assets/Scripts/ | Navigasi menu utama |
| GameConstants.cs | Assets/Scripts/ | Konstanta & konfigurasi |
| SoundManager.cs | Assets/Scripts/ | Singleton audio manager |
| TouchRotate.cs | Assets/Scripts/ | Rotasi objek 3D dengan touch |

---

## 📁 STRUKTUR DATA

### CSV Files Location:
- **Assets/StreamingAssets/DataKanji_N5.csv** - Data 12 Kanji untuk AR
- **Assets/Resources/Quiz_Easy.csv** - Soal quiz level mudah
- **Assets/Resources/Quiz_Medium.csv** - Soal quiz level sedang
- **Assets/Resources/Quiz_Hard.csv** - Soal quiz level sulit
- **Assets/Resources/Susun_Easy.csv** - Soal susun kata level mudah
- **Assets/Resources/Susun_Medium.csv** - Soal susun kata level sedang
- **Assets/Resources/Susun_Hard.csv** - Soal susun kata level sulit

### Audio Files:
- **Assets/Resources/SuaraKanji/** - Audio pengucapan Kanji (sfx_ki.wav, sfx_yama.wav, dll)

### Image Files:
- **Assets/Resources/GambarSoal/** - Gambar pendukung soal (opsional)

---

**Dokumentasi ini dibuat:** 25 Januari 2026  
**Unity Version:** 2021.3+  
**Dependencies:** Vuforia Engine, TextMeshPro
