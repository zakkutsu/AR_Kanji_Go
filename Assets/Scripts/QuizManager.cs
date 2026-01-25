using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [Header("UI Komponen")]
    public TextMeshProUGUI txtPertanyaan;
    public TextMeshProUGUI txtSkorInfo; 
    public Image imgSoal;
    
    [Header("Tombol Jawaban")]
    public Button[] tombolJawaban;
    public TextMeshProUGUI[] txtTombol;
    public Image[] imgTombol; 

    [Header("Navigasi & Feedback")]
    public Button btnPrev;
    public Button btnNext;
    public TextMeshProUGUI txtStatus; 
    public TextMeshProUGUI txtKunci;  

    [Header("Fitur Timer & Animasi")]
    public TextMeshProUGUI txtTimer;
    public float durasiMenit = GameConstants.QUIZ_TIMER_MINUTES;

    [Header("Setting Import")]
    public bool pakaiFileCSV = true;
    public string namaFileCSV = "DatabaseSoal";

    [Header("Database Soal")]
    public List<DataSoal> bankSoal;

    // Private variables
    private List<DataSoal> listSoalUjian; 
    private int indeksSoal = 0; 
    private int totalSoal = 0;
    
    private float sisaWaktuDetik;
    private bool timerBerjalan = false;

    private Color warnaNormal = GameConstants.COLOR_NORMAL;
    private Color warnaBenar = GameConstants.COLOR_BENAR;
    private Color warnaSalah = GameConstants.COLOR_SALAH;

    void Start()
    {
        // Setup timer
        sisaWaktuDetik = durasiMenit * 60;
        timerBerjalan = true;

        // Load soal dari CSV
        if (pakaiFileCSV) LoadSoalDariCSV();

        if (bankSoal == null || bankSoal.Count == 0) return;

        // Siapkan soal random
        int targetSoal = PlayerPrefs.GetInt("TargetSoal", 5);
        totalSoal = Mathf.Min(targetSoal, bankSoal.Count);

        List<DataSoal> tempSoal = new List<DataSoal>(bankSoal);
        
        foreach(var s in tempSoal) {
            s.sudahDijawab = false;
            s.jawabanUser = -1;
            s.isBenar = false;
        }

        listSoalUjian = new List<DataSoal>();
        for (int i = 0; i < totalSoal; i++)
        {
            int rand = Random.Range(0, tempSoal.Count);
            listSoalUjian.Add(tempSoal[rand]);
            tempSoal.RemoveAt(rand);
        }

        indeksSoal = 0;
        
        btnNext.onClick.AddListener(KlikNext);
        btnPrev.onClick.AddListener(KlikPrev);

        UpdateTampilanSoal();
    }

    void Update()
    {
        // Update timer
        if (timerBerjalan)
        {
            sisaWaktuDetik -= Time.deltaTime;
            
            int menit = Mathf.FloorToInt(sisaWaktuDetik / 60);
            int detik = Mathf.FloorToInt(sisaWaktuDetik % 60);
            
            if(txtTimer != null)
            {
                txtTimer.text = string.Format("{0:00}:{1:00}", menit, detik);
                
                // Merah kalo waktu tinggal dikit
                if (sisaWaktuDetik <= GameConstants.TIMER_WARNING_SECONDS) 
                    txtTimer.color = GameConstants.COLOR_TIMER_WARNING;
                else 
                    txtTimer.color = GameConstants.COLOR_TIMER_NORMAL;
            }
            else
            {
                Debug.LogWarning("txtTimer belum di-assign di Inspector!");
                timerBerjalan = false;
            }

            // Waktu habis
            if (sisaWaktuDetik <= 0)
            {
                sisaWaktuDetik = 0;
                Debug.Log("Waktu Habis!");
                KlikNext();
            }
        }

        // Tombol back HP
        if (Input.GetKeyDown(KeyCode.Escape)) KeluarKeMenu();
    }

    void UpdateTampilanSoal()
    {
        sisaWaktuDetik = durasiMenit * 60; 
        timerBerjalan = true;
        
        // Reset warna timer
        if(txtTimer != null) txtTimer.color = GameConstants.COLOR_TIMER_NORMAL;

        DataSoal soalSaatIni = listSoalUjian[indeksSoal];

        txtPertanyaan.text = soalSaatIni.pertanyaan;
        if (txtSkorInfo != null) txtSkorInfo.text = "Soal " + (indeksSoal + 1) + " / " + totalSoal;

        if (soalSaatIni.gambarSoal != null) {
            imgSoal.gameObject.SetActive(true);
            imgSoal.sprite = soalSaatIni.gambarSoal;
        } else {
            imgSoal.gameObject.SetActive(false);
        }

        // Setup buttons dengan cleanup proper
        for (int i = 0; i < tombolJawaban.Length; i++)
        {
            if (i < soalSaatIni.pilihanJawaban.Length)
            {
                txtTombol[i].text = soalSaatIni.pilihanJawaban[i];
                
                // Proper cleanup: Remove listeners before adding new ones
                tombolJawaban[i].onClick.RemoveAllListeners();
                
                // Capture index untuk closure
                int indexPilihan = i;
                tombolJawaban[i].onClick.AddListener(() => JawabSoal(indexPilihan));
                
                // Reset warna tombol
                if(imgTombol.Length > i && imgTombol[i] != null) 
                    imgTombol[i].color = warnaNormal;
            }
        }

        if (soalSaatIni.sudahDijawab)
        {
            KunciTombolDanTampilHasil(soalSaatIni);
        }
        else
        {
            txtStatus.text = "";
            txtKunci.text = "";
            foreach (Button btn in tombolJawaban) btn.interactable = true;
        }

        btnPrev.interactable = (indeksSoal > 0); 
        
        if (indeksSoal == totalSoal - 1)
        {
            if(btnNext.GetComponentInChildren<TextMeshProUGUI>() != null)
                btnNext.GetComponentInChildren<TextMeshProUGUI>().text = "SELESAI";
        }
        else
        {
             if(btnNext.GetComponentInChildren<TextMeshProUGUI>() != null)
                btnNext.GetComponentInChildren<TextMeshProUGUI>().text = "LANJUT";
        }
    }

    void JawabSoal(int pilihanUser)
    {
        DataSoal soal = listSoalUjian[indeksSoal];
        if (soal.sudahDijawab) return; 

        soal.sudahDijawab = true;
        soal.jawabanUser = pilihanUser;

        if (pilihanUser == soal.kunciJawaban)
        {
            soal.isBenar = true;
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBenar();
        }
        else
        {
            soal.isBenar = false;
            if (SoundManager.Instance != null) SoundManager.Instance.PlaySalah();
        }
        timerBerjalan = false;

        KunciTombolDanTampilHasil(soal);
    }

    void KunciTombolDanTampilHasil(DataSoal soal)
    {
        foreach (Button btn in tombolJawaban) btn.interactable = false;

        if (soal.jawabanUser != -1) 
        {
            if (soal.isBenar)
            {
                if(imgTombol[soal.jawabanUser] != null) 
                    imgTombol[soal.jawabanUser].color = warnaBenar;
                
                txtStatus.text = "BENAR";
                txtStatus.color = warnaBenar;
            }
            else
            {
                if(imgTombol[soal.jawabanUser] != null)
                    imgTombol[soal.jawabanUser].color = warnaSalah;
                
                if(imgTombol[soal.kunciJawaban] != null)
                    imgTombol[soal.kunciJawaban].color = warnaBenar; 
                
                txtStatus.text = "SALAH";
                txtStatus.color = warnaSalah;
            }
        }
        else 
        {
             if(imgTombol[soal.kunciJawaban] != null)
                imgTombol[soal.kunciJawaban].color = warnaBenar;
             
             txtStatus.text = "TIDAK DIJAWAB";
             txtStatus.color = warnaSalah;
        }

        if(soal.pilihanJawaban.Length > soal.kunciJawaban)
            txtKunci.text = "Jawaban: " + soal.pilihanJawaban[soal.kunciJawaban];

        // --- ANIMASI POP (JUICY) ---
        StartCoroutine(EfekPop(txtStatus.transform));
    }

    // --- LOGIKA ANIMASI POP SEDERHANA ---
    IEnumerator EfekPop(Transform target)
    {
        // Membesar
        target.localScale = Vector3.one * 0.1f; // Mulai dari kecil
        float timer = 0;
        while(timer < 0.2f)
        {
            timer += Time.deltaTime;
            target.localScale = Vector3.Lerp(Vector3.one * 0.5f, Vector3.one * 1.2f, timer * 5);
            yield return null;
        }
        
        // Kembali ke ukuran normal
        target.localScale = Vector3.one; 
    }

    void KlikNext()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();

        DataSoal soalSekarang = listSoalUjian[indeksSoal];

        if (!soalSekarang.sudahDijawab)
        {
            soalSekarang.sudahDijawab = true;
            soalSekarang.jawabanUser = -1; 
            soalSekarang.isBenar = false;
        }

        if (indeksSoal < totalSoal - 1)
        {
            indeksSoal++; 
            UpdateTampilanSoal();
        }
        else
        {
            HitungSkorDanSelesai();
        }
    }

    void KlikPrev()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        if (indeksSoal > 0)
        {
            indeksSoal--; 
            UpdateTampilanSoal();
        }
    }

    void HitungSkorDanSelesai()
    {
        timerBerjalan = false; // Stop timer

        int skorTotal = 0;
        foreach (DataSoal s in listSoalUjian)
        {
            if (s.isBenar) skorTotal += GameConstants.POIN_QUIZ;
        }

        PlayerPrefs.SetInt(GameConstants.PREF_SKOR_SEMENTARA, skorTotal);
        PlayerPrefs.Save();
        
        Debug.Log("Quiz selesai - Skor: " + skorTotal + " - Lanjut ke Susun Kata");
        SceneManager.LoadScene(GameConstants.SCENE_SUSUN_KATA);
    }
    
    public void KeluarKeMenu() 
    { 
        timerBerjalan = false; // Stop timer sebelum keluar
        SceneManager.LoadScene(GameConstants.SCENE_MAIN_MENU); 
    }

    void LoadSoalDariCSV()
    {
        string level = PlayerPrefs.GetString(GameConstants.PREF_LEVEL_DIPILIH, GameConstants.LEVEL_EASY);
        string namaFileLoad = "Quiz_" + level; 
        namaFileCSV = namaFileLoad; 

        TextAsset dataCSV = Resources.Load<TextAsset>(namaFileLoad);
        if (dataCSV == null) return;

        bankSoal = new List<DataSoal>();
        string[] barisData = dataCSV.text.Split('\n');

        for (int i = 1; i < barisData.Length; i++)
        {
            string baris = barisData[i];
            if (string.IsNullOrWhiteSpace(baris)) continue;
            string[] kolom = baris.Split(new char[] { ',', ';' });
            if (kolom.Length < 6) continue; 

            DataSoal soalBaru = new DataSoal();
            soalBaru.pertanyaan = kolom[0].Replace("\"", "");
            soalBaru.pilihanJawaban = new string[4];
            soalBaru.pilihanJawaban[0] = kolom[1];
            soalBaru.pilihanJawaban[1] = kolom[2];
            soalBaru.pilihanJawaban[2] = kolom[3];
            soalBaru.pilihanJawaban[3] = kolom[4];
            int.TryParse(kolom[5], out soalBaru.kunciJawaban);

            if (kolom.Length > 6) {
                string namaGambar = kolom[6].Trim();
                if (namaGambar.Length > 1 && namaGambar.ToLower() != "none")
                    soalBaru.gambarSoal = Resources.Load<Sprite>("GambarSoal/" + namaGambar);
            }
            bankSoal.Add(soalBaru);
        }
    }
}

[System.Serializable]
public class DataSoal
{
    [TextArea] public string pertanyaan;
    public Sprite gambarSoal;
    public string[] pilihanJawaban;
    public int kunciJawaban;

    public bool sudahDijawab = false;
    public int jawabanUser = -1; 
    public bool isBenar = false;
}