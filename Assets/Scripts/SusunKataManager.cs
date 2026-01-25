using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq; 

public class SusunKataManager : MonoBehaviour
{
    [Header("UI Komponen")]
    public TextMeshProUGUI txtPertanyaan;
    public TextMeshProUGUI txtSkorInfo; 
    public Image imgSoal;
    public Transform slotJawaban; 
    public Transform slotPilihan; 

    [Header("Fitur Timer Per Soal")]
    public TextMeshProUGUI txtTimer; 
    public float durasiMenit = GameConstants.SUSUN_KATA_TIMER_MINUTES; // 30 detik per soal

    [Header("Prefab")]
    public GameObject prefabTombolHuruf; 

    [Header("Feedback")]
    public GameObject panelFeedback;
    public TextMeshProUGUI txtFeedback;
    public Button btnLanjut; 

    [Header("Setting Import")]
    public bool pakaiFileCSV = true; 
    public string namaFileCSV = "Susun_Easy"; 

    [Header("Database Soal")]
    public List<DataSoalSusun> bankSoal; 

    // VARIABEL LOGIC 
    private List<DataSoalSusun> soalTersisa; 
    private DataSoalSusun soalAktif;
    
    private List<string> jawabanUser = new List<string>(); 
    private List<GameObject> tombolTerpakai = new List<GameObject>(); 

    private int targetSoalLevel = 0; 
    private int soalSudahKeluar = 0; 
    private int batasanReal = 0; 

    int skorTotal = 0; 
    
    // Variabel Timer
    private float sisaWaktuDetik;
    private bool timerBerjalan = false;
    
    // Cooldown untuk prevent spam
    private bool sedangMemproses = false;

    void Start()
    {
        // 1. Terima Data Estafet dengan GameConstants
        skorTotal = PlayerPrefs.GetInt(GameConstants.PREF_SKOR_SEMENTARA, 0);
        targetSoalLevel = PlayerPrefs.GetInt(GameConstants.PREF_TARGET_SOAL, 5);

        // 2. Load CSV
        if (pakaiFileCSV) LoadSoalDariCSV();

        if (bankSoal == null || bankSoal.Count == 0) return;

        // 3. Persiapan Soal
        soalTersisa = new List<DataSoalSusun>(bankSoal);
        batasanReal = Mathf.Min(targetSoalLevel, bankSoal.Count);
        soalSudahKeluar = 0;

        AmbilSoalAcak();
    }

    void Update()
    {
        // --- LOGIKA TIMER PER SOAL ---
        if (timerBerjalan)
        {
            sisaWaktuDetik -= Time.deltaTime;
            
            // Format Menit:Detik
            int menit = Mathf.FloorToInt(sisaWaktuDetik / 60);
            int detik = Mathf.FloorToInt(sisaWaktuDetik % 60);
            
            if(txtTimer != null)
            {
                txtTimer.text = string.Format("{0:00}:{1:00}", menit, detik);
                
                // Merah jika < warning seconds, Normal jika masih aman
                if (sisaWaktuDetik <= GameConstants.TIMER_WARNING_SECONDS) 
                    txtTimer.color = GameConstants.COLOR_TIMER_WARNING;
                else 
                    txtTimer.color = GameConstants.COLOR_TIMER_NORMAL; 
            }

            // --- JIKA WAKTU HABIS ---
            if (sisaWaktuDetik <= 0)
            {
                sisaWaktuDetik = 0;
                timerBerjalan = false; // Stop timer
                
                Debug.Log("Waktu Susun Kata Habis! Anggap Salah.");
                
                // Mainkan suara salah
                if (SoundManager.Instance != null) SoundManager.Instance.PlaySalah();
                
                // Munculkan Feedback Salah (Otomatis lanjut nanti)
                TampilkanFeedback(false);
            }
        }
        
        // --- TOMBOL BACK HP ---
        if (Input.GetKeyDown(KeyCode.Escape)) KeluarKeMenu();
    }

    public void AmbilSoalAcak()
    {
        BersihkanUI();
        panelFeedback.SetActive(false);

        // --- RESET TIMER SETIAP GANTI SOAL ---
        sisaWaktuDetik = durasiMenit * 60; 
        timerBerjalan = true;
        sedangMemproses = false; // Reset cooldown
        if(txtTimer != null) txtTimer.color = GameConstants.COLOR_TIMER_NORMAL;
        // -------------------------------------

        // CEK KONDISI STOP
        if (soalTersisa.Count == 0 || soalSudahKeluar >= batasanReal)
        {
            FinishGame();
            return;
        }

        // KOCOK SOAL
        int indexAcak = Random.Range(0, soalTersisa.Count);
        soalAktif = soalTersisa[indexAcak];
        
        soalSudahKeluar++; 

        // Update UI
        txtPertanyaan.text = soalAktif.pertanyaan;
        
        if (txtSkorInfo != null)
             txtSkorInfo.text = "Soal " + soalSudahKeluar + " / " + batasanReal;

        if (soalAktif.gambarSoal != null)
        {
            imgSoal.gameObject.SetActive(true);
            imgSoal.sprite = soalAktif.gambarSoal;
        }
        else
        {
            imgSoal.gameObject.SetActive(false);
        }

        // Logic Pecah Huruf - dengan proper cleanup
        string jawabanBenar = soalAktif.kunciJawaban.ToUpper(); 
        List<char> hurufAcak = jawabanBenar.ToCharArray().ToList(); 
        hurufAcak = hurufAcak.OrderBy(x => Random.value).ToList(); 

        foreach (char huruf in hurufAcak)
        {
            GameObject tombolBaru = Instantiate(prefabTombolHuruf, slotPilihan);
            TextMeshProUGUI txtHuruf = tombolBaru.GetComponentInChildren<TextMeshProUGUI>();
            
            if (txtHuruf != null)
            {
                txtHuruf.text = huruf.ToString();
            }
            
            Button btnComponent = tombolBaru.GetComponent<Button>();
            if (btnComponent != null)
            {
                // Clear existing listeners untuk safety
                btnComponent.onClick.RemoveAllListeners();
                
                // Capture variables untuk closure
                string hurufString = huruf.ToString();
                GameObject tombolRef = tombolBaru;
                
                btnComponent.onClick.AddListener(() => PilihHuruf(hurufString, tombolRef));
            }
            
            tombolTerpakai.Add(tombolBaru);
        }

        soalTersisa.RemoveAt(indexAcak);
    }

    void PilihHuruf(string huruf, GameObject tombolAsal)
    {
        // Prevent spam click saat sedang memproses jawaban
        if (sedangMemproses || tombolAsal == null) return;
        
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();

        jawabanUser.Add(huruf);
        tombolAsal.transform.SetParent(slotJawaban); 

        // Cek jika jumlah huruf sudah pas
        if (jawabanUser.Count == soalAktif.kunciJawaban.Length)
        {
            CekJawabanAkhir();
        }
    }

    void CekJawabanAkhir()
    {
        // Prevent double-check
        if (sedangMemproses) return;
        sedangMemproses = true;
        
        // Stop timer saat user selesai menyusun (biar user bisa lihat hasilnya tenang)
        timerBerjalan = false; 

        string jawabanString = string.Join("", jawabanUser);

        if (jawabanString == soalAktif.kunciJawaban.ToUpper())
        {
            skorTotal += GameConstants.POIN_SUSUN_KATA; 
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBenar();
            TampilkanFeedback(true);
        }
        else
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySalah();
            TampilkanFeedback(false);
        }
    }

    // ... (SISA FUNGSI LAINNYA SAMA SEPERTI SEBELUMNYA: LoadSoal, TampilkanFeedback, dll) ...
    // Pastikan fungsi LoadSoalDariCSV, TampilkanFeedback, LanjutOtomatis, EfekPop, FinishGame tetap ada.
    
    // BIAR AMAN, COPY PASTE SISA FUNGSI DI BAWAH INI JUGA:

    void TampilkanFeedback(bool isBenar)
    {
        panelFeedback.SetActive(true);
        if (isBenar)
        {
            txtFeedback.text = "BENAR! " + soalAktif.kunciJawaban;
            txtFeedback.color = GameConstants.COLOR_BENAR;
        }
        else
        {
            txtFeedback.text = "SALAH! Jawabannya: " + soalAktif.kunciJawaban;
            txtFeedback.color = GameConstants.COLOR_SALAH;
        }

        StartCoroutine(LanjutOtomatis());
    }

    IEnumerator LanjutOtomatis()
    {
        StartCoroutine(EfekPop(panelFeedback.transform));
        yield return new WaitForSeconds(1.0f);
        AmbilSoalAcak();
    }
    
    IEnumerator EfekPop(Transform target)
    {
        target.localScale = Vector3.one * 0.1f; 
        float timer = 0;
        while(timer < 0.2f)
        {
            timer += Time.deltaTime;
            target.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.0f, timer * 5);
            yield return null;
        }
        target.localScale = Vector3.one; 
    }

    void BersihkanUI()
    {
        jawabanUser.Clear();
        
        // Proper cleanup dengan null check dan remove listeners
        foreach (GameObject t in tombolTerpakai)
        {
            if (t != null)
            {
                Button btn = t.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.RemoveAllListeners(); // Cleanup listeners
                }
                Destroy(t);
            }
        }
        tombolTerpakai.Clear();
    }

    void FinishGame()
    {
        timerBerjalan = false;
        sedangMemproses = false;
        
        Debug.Log("Estafet Selesai! Total Skor: " + skorTotal);
        PlayerPrefs.SetInt(GameConstants.PREF_SKOR_TERAKHIR, skorTotal);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(GameConstants.SCENE_NILAI);
    }
    
    public void KeluarKeMenu()
    {
         timerBerjalan = false;
         if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
         SceneManager.LoadScene(GameConstants.SCENE_MAIN_MENU);
    }

    void LoadSoalDariCSV()
    {
        string level = PlayerPrefs.GetString(GameConstants.PREF_LEVEL_DIPILIH, GameConstants.LEVEL_EASY);
        string namaFileLoad = "Susun_" + level;
        namaFileCSV = namaFileLoad;

        TextAsset dataCSV = Resources.Load<TextAsset>(namaFileLoad);
        if (dataCSV == null) return;

        bankSoal = new List<DataSoalSusun>();
        string[] barisData = dataCSV.text.Split('\n');

        for (int i = 1; i < barisData.Length; i++)
        {
            string baris = barisData[i];
            if (string.IsNullOrWhiteSpace(baris)) continue;
            string[] kolom = baris.Split(new char[] { ',', ';' });
            if (kolom.Length < 2) continue; 

            DataSoalSusun soalBaru = new DataSoalSusun();
            soalBaru.pertanyaan = kolom[0].Replace("\"", "");
            soalBaru.kunciJawaban = kolom[1].Trim().ToUpper().Replace("\"", ""); 

            if (kolom.Length > 2)
            {
                string namaGambar = kolom[2].Trim().Replace("\"", "");
                if (namaGambar.Length > 1 && namaGambar.ToLower() != "none")
                    soalBaru.gambarSoal = Resources.Load<Sprite>("GambarSoal/" + namaGambar);
            }
            bankSoal.Add(soalBaru);
        }
    }
}

[System.Serializable]
public class DataSoalSusun
{
    [TextArea] public string pertanyaan;
    public Sprite gambarSoal;
    public string kunciJawaban; 
}