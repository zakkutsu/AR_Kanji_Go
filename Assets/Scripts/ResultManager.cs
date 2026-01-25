using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class ResultManager : MonoBehaviour
{
    [Header("UI Utama")]
    public TextMeshProUGUI txtSkor;
    public TextMeshProUGUI txtInfoLevel; 

    [Header("Animasi Pop")]
    public Transform targetAnimasi;

    [Header("Panel Konfirmasi")]
    public GameObject panelKonfirmasi;
    private string aksiTertunda = "";

    [Header("Sistem Bintang")]
    public Image[] slotBintang; 
    public Sprite bintangKosong; 
    public Sprite bintangNyala;  

    void Start()
    {
        // Ambil data dari PlayerPrefs
        int nilaiAkhir = PlayerPrefs.GetInt(GameConstants.PREF_SKOR_TERAKHIR, 0);
        int jumlahSoal = PlayerPrefs.GetInt(GameConstants.PREF_TARGET_SOAL, GameConstants.SOAL_EASY);
        string namaLevel = PlayerPrefs.GetString(GameConstants.PREF_LEVEL_DIPILIH, GameConstants.LEVEL_EASY);
        
        // Validasi data
        if (nilaiAkhir < 0) nilaiAkhir = 0;
        if (jumlahSoal <= 0) jumlahSoal = GameConstants.SOAL_EASY;
        
        Debug.Log($"Result - Level: {namaLevel}, Soal: {jumlahSoal}, Skor: {nilaiAkhir}");
        
        // Tampilkan level
        if (txtInfoLevel != null) 
        {
            txtInfoLevel.text = namaLevel;
        }
        
        // Tampilkan skor
        if(txtSkor != null) 
        {
            txtSkor.text = nilaiAkhir.ToString();
        }
        else
        {
            Debug.LogWarning("txtSkor belum di-assign di Inspector!");
        }

        // Hitung bintang
        int nilaiMax = jumlahSoal * GameConstants.POIN_PER_SOAL;
        
        // Safety check
        if (nilaiMax <= 0) 
        {
            Debug.LogWarning("Nilai max invalid, pake default 100");
            nilaiMax = 100;
        }

        float persentase = (float)nilaiAkhir / (float)nilaiMax;
        
        // Clamp 0-1
        persentase = Mathf.Clamp01(persentase);
        
        Debug.Log($"Persentase: {persentase * 100:F1}%");

        // Hitung bintang pake helper
        int jumlahBintang = GameConstants.HitungBintang(persentase);
        SetBintang(jumlahBintang);

        // Sound effect
        if (SoundManager.Instance != null) SoundManager.Instance.PlayResult();
        
        // Animasi
        if (targetAnimasi != null) 
        {
            StartCoroutine(EfekPop(targetAnimasi));
        }
    }

    // Set bintang yang nyala (0-3)
    void SetBintang(int jumlah)
    {
        // Validasi
        jumlah = Mathf.Clamp(jumlah, 0, 3);
        
        // Reset semua bintang
        for(int i = 0; i < slotBintang.Length; i++)
        {
            if(slotBintang[i] != null)
            {
                slotBintang[i].sprite = bintangKosong;
            }
        }
        
        // Nyalakan bintang sesuai jumlah
        for(int i = 0; i < jumlah && i < slotBintang.Length; i++) 
        {
            if(slotBintang[i] != null)
            {
                slotBintang[i].sprite = bintangNyala;
            }
        }
        
        Debug.Log($"Bintang yang menyala: {jumlah}");
    }

    public void PlaySuaraTombol()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
    }

    public void KlikMenu_MintaKonfirmasi()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        aksiTertunda = "menu";
        panelKonfirmasi.SetActive(true);
    }

    public void KlikUlangi_MintaKonfirmasi()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        aksiTertunda = "ulangi";
        panelKonfirmasi.SetActive(true);
    }

    // Dipasang di tombol YA
    public void Jawaban_YA()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        
        // Cek user mau kemana
        if (aksiTertunda == "menu")
        {
            // Clear data session saat balik ke menu
            GameConstants.ClearGameSessionData();
            SceneManager.LoadScene(GameConstants.SCENE_MAIN_MENU);
        }
        else if (aksiTertunda == "ulangi")
        {
            // Keep level data tapi clear skor
            PlayerPrefs.DeleteKey(GameConstants.PREF_SKOR_SEMENTARA);
            PlayerPrefs.DeleteKey(GameConstants.PREF_SKOR_TERAKHIR);
            PlayerPrefs.Save();
            
            SceneManager.LoadScene(GameConstants.SCENE_QUIZ);
        }
    }

    // Dipasang di tombol BATAL
    public void Jawaban_TIDAK()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        panelKonfirmasi.SetActive(false);
    }

    // Fungsi tombol lainnya
    public void KeMenuUtama()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        
        // Clear session data
        GameConstants.ClearGameSessionData();
        
        SceneManager.LoadScene(GameConstants.SCENE_MAIN_MENU);
    }

    public void UlangiQuiz()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        
        // Clear skor aja, keep level data
        PlayerPrefs.DeleteKey(GameConstants.PREF_SKOR_SEMENTARA);
        PlayerPrefs.DeleteKey(GameConstants.PREF_SKOR_TERAKHIR);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(GameConstants.SCENE_QUIZ);
    }

    void Update()
    {
        // Tombol Back HP
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            KlikMenu_MintaKonfirmasi();
        }
    }

    IEnumerator EfekPop(Transform target)
    {
        target.localScale = Vector3.one * 0.1f; 
        
        float timer = 0;
        while(timer < 0.3f)
        {
            timer += Time.deltaTime;
            target.localScale = Vector3.Lerp(Vector3.one * 0.1f, Vector3.one * 1.2f, timer * 4);
            yield return null;
        }
        
        target.localScale = Vector3.one; 
    }
}
