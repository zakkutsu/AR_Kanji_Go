using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel Info")]
    [SerializeField] private GameObject panelInfo;
    
    [Header("Panel Quit Confirmation")]
    [SerializeField] private GameObject panelQuitConfirm;

    public void BukaPanelInfo()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        panelInfo.SetActive(true);
    }

    public void TutupPanelInfo()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        panelInfo.SetActive(false);
    }

    [Header("Panel UI")]
    [SerializeField] private GameObject panelMode;
    [SerializeField] private GameObject panelLevel;

    // Fungsi tombol halaman depan

    public void BukaPanelMode()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        panelMode.SetActive(true);  // Munculin pilihan AR vs Quiz
        panelLevel.SetActive(false); // Pastikan panel level tutup
    }

    // 2. Dipasang di Tombol "X" pada Panel Mode
    public void TutupPanelMode()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        panelMode.SetActive(false);
    }

    // --- FUNGSI PILIHAN MODE ---

    // 3. Dipasang di Tombol "BELAJAR (AR)"
    public void MasukModeAR()
    {
        // Play sound dan langsung pindah ke Scene AR
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        
        if (Application.CanStreamedLevelBeLoaded(GameConstants.SCENE_MENU_BELAJAR))
        {
            SceneManager.LoadScene(GameConstants.SCENE_MENU_BELAJAR);
        }
        else
        {
            Debug.LogError($"Scene '{GameConstants.SCENE_MENU_BELAJAR}' tidak ditemukan di Build Settings!");
        }
    }

    // 4. Dipasang di Tombol "UJIAN (QUIZ)"
    public void BukaPanelLevel()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        panelMode.SetActive(false);
        panelLevel.SetActive(true);
    }

    public void KembaliKePilihMode()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        panelLevel.SetActive(false);
        panelMode.SetActive(true);
    }

    // Fungsi pilihan level quiz

    public void PilihLevel(int jumlahSoal)
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        
        // Simpan jumlah soal
        PlayerPrefs.SetInt(GameConstants.PREF_TARGET_SOAL, jumlahSoal);

        // Tentukan nama level
        string namaLevel = GameConstants.GetNamaLevel(jumlahSoal);

        PlayerPrefs.SetString(GameConstants.PREF_LEVEL_DIPILIH, namaLevel);
        PlayerPrefs.Save();

        DebugLog("Level dipilih: " + namaLevel + " (" + jumlahSoal + " soal)");
        
        if (Application.CanStreamedLevelBeLoaded(GameConstants.SCENE_QUIZ))
        {
            SceneManager.LoadScene(GameConstants.SCENE_QUIZ);
        }
        else
        {
            Debug.LogError($"Scene '{GameConstants.SCENE_QUIZ}' tidak ditemukan di Build Settings!");
        }
    }

    // Fungsi quit game
    
    // Tampilkan panel konfirmasi
    public void MintaKonfirmasiKeluar()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        
        if (panelQuitConfirm != null)
        {
            panelQuitConfirm.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Panel Quit Confirmation tidak di-assign!");
            KeluarGame();
        }
    }
    
    // User tekan Ya
    public void KonfirmasiKeluarGame()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        KeluarGame();
    }
    
    // User tekan Tidak
    public void BatalKeluar()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        
        if (panelQuitConfirm != null)
        {
            panelQuitConfirm.SetActive(false);
        }
    }
    
    // Keluar dari aplikasi
    public void KeluarGame()
    {
        // Clear game session data sebelum keluar (optional - untuk cleanup)
        // GameConstants.ClearGameSessionData();
        
        DebugLog("Keluar dari Aplikasi");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    
    // --- EVENT HANDLERS ---
    
    void Update()
    {
        // Handle back button (Android)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }
    
    void HandleBackButton()
    {
        // Jika panel quit confirmation terbuka, tutup dulu
        if (panelQuitConfirm != null && panelQuitConfirm.activeSelf)
        {
            BatalKeluar();
            return;
        }
        
        // Jika panel info terbuka, tutup
        if (panelInfo != null && panelInfo.activeSelf)
        {
            TutupPanelInfo();
            return;
        }
        
        // Jika panel level terbuka, kembali ke panel mode
        if (panelLevel != null && panelLevel.activeSelf)
        {
            KembaliKePilihMode();
            return;
        }
        
        // Jika panel mode terbuka, tutup
        if (panelMode != null && panelMode.activeSelf)
        {
            TutupPanelMode();
            return;
        }
        
        // Jika tidak ada panel terbuka, tanya konfirmasi keluar
        MintaKonfirmasiKeluar();
    }
    
    // Conditional debug logging (hanya aktif di Editor)
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    void DebugLog(string message)
    {
        Debug.Log(message);
    }
}