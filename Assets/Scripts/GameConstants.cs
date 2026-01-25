using UnityEngine;

// File ini buat nyimpen konstanta biar ga hardcode string di mana-mana
public static class GameConstants
{
    // Scene names
    public const string SCENE_MAIN_MENU = "Scene_MainMenu";
    public const string SCENE_MENU_BELAJAR = "Scene_MenuBelajar";
    public const string SCENE_QUIZ = "Scene_Quiz";
    public const string SCENE_SUSUN_KATA = "Scene_SusunKata";
    public const string SCENE_NILAI = "Scene_Nilai";

    // PlayerPrefs keys
    public const string PREF_TARGET_SOAL = "TargetSoal";
    public const string PREF_LEVEL_DIPILIH = "LevelDipilih";
    public const string PREF_SKOR_SEMENTARA = "SkorSementara";
    public const string PREF_SKOR_TERAKHIR = "SkorTerakhir";

    // Level names
    public const string LEVEL_EASY = "Easy";
    public const string LEVEL_MEDIUM = "Medium";
    public const string LEVEL_HARD = "Hard";

    // Game settings
    public const int SOAL_EASY = 10;
    public const int SOAL_MEDIUM = 15;
    public const int SOAL_HARD = 20;

    public const int POIN_QUIZ = 10;
    public const int POIN_SUSUN_KATA = 10;
    public const int POIN_PER_SOAL = POIN_QUIZ + POIN_SUSUN_KATA; // 20

    // Timer settings
    public const float QUIZ_TIMER_MINUTES = 5.0f;
    public const float SUSUN_KATA_TIMER_MINUTES = 0.5f; // 30 detik
    public const float TIMER_WARNING_SECONDS = 10.0f;

    // Warna UI
    public static readonly Color COLOR_NORMAL = Color.white;
    public static readonly Color COLOR_BENAR = Color.green;
    public static readonly Color COLOR_SALAH = Color.red;
    public static readonly Color COLOR_TIMER_NORMAL = Color.black;
    public static readonly Color COLOR_TIMER_WARNING = Color.red;

    // Helper functions
    
    // Get jumlah soal dari nama level
    public static int GetJumlahSoal(string namaLevel)
    {
        switch (namaLevel)
        {
            case LEVEL_EASY: return SOAL_EASY;
            case LEVEL_MEDIUM: return SOAL_MEDIUM;
            case LEVEL_HARD: return SOAL_HARD;
            default: return SOAL_EASY;
        }
    }

    // Get nama level dari jumlah soal
    public static string GetNamaLevel(int jumlahSoal)
    {
        if (jumlahSoal == SOAL_MEDIUM) return LEVEL_MEDIUM;
        if (jumlahSoal == SOAL_HARD) return LEVEL_HARD;
        return LEVEL_EASY;
    }

    // Clear semua data session game
    public static void ClearGameSessionData()
    {
        PlayerPrefs.DeleteKey(PREF_TARGET_SOAL);
        PlayerPrefs.DeleteKey(PREF_LEVEL_DIPILIH);
        PlayerPrefs.DeleteKey(PREF_SKOR_SEMENTARA);
        PlayerPrefs.DeleteKey(PREF_SKOR_TERAKHIR);
        PlayerPrefs.Save();
        Debug.Log("Game session data cleared");
    }

    // Hitung bintang dari persentase
    public static int HitungBintang(float persentase)
    {
        if (persentase >= 0.8f) return 3;
        if (persentase >= 0.5f) return 2;
        if (persentase > 0.1f) return 1;
        return 0;
    }
}
