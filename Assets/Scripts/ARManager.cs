using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class ARManager : MonoBehaviour
{
    public static ARManager Instance;

    [Header("UI References")]
    [SerializeField] private GameObject panelInfo; 
    [SerializeField] private GameObject panelMode;
    [SerializeField] private GameObject panelAbout;
    [SerializeField] private TextMeshProUGUI txtKanji;
    [SerializeField] private TextMeshProUGUI txtArti;
    [SerializeField] private TextMeshProUGUI txtOnyomi;
    [SerializeField] private TextMeshProUGUI txtKunyomi;
    [SerializeField] private TextMeshProUGUI txtContoh;
    
    [Header("Audio System")]
    [SerializeField] private AudioSource audioSource;

    // Wadah data
    [System.Serializable]
    public class KanjiData
    {
        public string id;
        public string kanji;
        public string arti;
        public string onyomi;
        public string kunyomi;
        public string contoh;
        public string audioFile;
    }

    private Dictionary<string, KanjiData> databaseKanji = new Dictionary<string, KanjiData>();
    private string currentAudioFile = "";
    private string currentTrackedMarker = "";

    void Awake()
    {
        // Singleton pattern - hanya boleh ada satu instance
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    IEnumerator Start()
    {
        // Reset semua state di awal
        currentAudioFile = "";
        currentTrackedMarker = "";
        
        if(panelInfo != null) panelInfo.SetActive(false);
        if(panelMode != null) panelMode.SetActive(false);
        if(panelAbout != null) panelAbout.SetActive(false);
        
        if(txtKanji != null) txtKanji.text = "";
        if(txtArti != null) txtArti.text = "";
        if(txtOnyomi != null) txtOnyomi.text = "";
        if(txtKunyomi != null) txtKunyomi.text = "";
        if(txtContoh != null) txtContoh.text = "";
        
        yield return StartCoroutine(LoadCSV());
    }

    IEnumerator LoadCSV()
    {
        string fileName = "DataKanji_N5.csv";
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        string csvContent = "";

        // Check platform Android atau PC
        if (Application.platform == RuntimePlatform.Android)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(filePath))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success) 
                {
                    csvContent = www.downloadHandler.text;
                }
                else
                {
                    Debug.LogError("Gagal Load CSV: " + www.error);
                }
            }
        }
        else
        {
            // Baca file di PC/Editor
            if(File.Exists(filePath))
            {
                csvContent = File.ReadAllText(filePath);
            }
            else
            {
                Debug.LogError("File CSV tidak ditemukan di StreamingAssets!");
            }
        }

        ParseCSV(csvContent);
    }

    void ParseCSV(string content)
    {
        string[] lines = content.Split('\n');
        
        // Loop mulai dari 1 karena baris 0 header
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;
            
            string[] data = line.Split(',');

            // Pastikan ada 7 kolom data
            if (data.Length >= 7)
            {
                KanjiData k = new KanjiData();
                k.id = data[0];         // ID (kanji_ame)
                k.kanji = data[1];      // Kanji (雨)
                k.arti = data[2];       // Arti (Hujan)
                k.onyomi = data[3];     // Onyomi
                k.kunyomi = data[4];    // Kunyomi
                k.contoh = data[5];     // Contoh Kata
                k.audioFile = data[6].Trim(); // Audio

                if (!databaseKanji.ContainsKey(k.id)) 
                {
                    databaseKanji.Add(k.id, k);
                }
            }
        }
    }

    // Tampilkan data kanji
    public void ShowKanjiData(string markerID)
    {
        DebugLog("ShowKanjiData dipanggil dengan ID: " + markerID);
        
        // PERBAIKAN: Jika ada marker lain yang aktif, hide dulu
        if (!string.IsNullOrEmpty(currentTrackedMarker) && currentTrackedMarker != markerID)
        {
            DebugLog("Marker berubah dari " + currentTrackedMarker + " ke " + markerID + " - Hide panel lama");
            HidePanel();
        }
        
        // Update marker yang lagi tracked
        currentTrackedMarker = markerID;
        
        if (databaseKanji.ContainsKey(markerID))
        {
            KanjiData data = databaseKanji[markerID];
            
            DebugLog("Data ditemukan: " + data.kanji + " (" + data.arti + ")");

            // PENTING: Sembunyikan panel dulu untuk force refresh UI
            if(panelInfo != null) panelInfo.SetActive(false);

            // Masukkan data ke kotak UI masing-masing
            if(txtKanji != null) 
            {
                txtKanji.text = data.kanji;
                txtKanji.ForceMeshUpdate(); // Force update TextMeshPro
            }
            if(txtArti != null)
            {
                txtArti.text = data.arti;
                txtArti.ForceMeshUpdate();
            }
            if(txtOnyomi != null)
            {
                txtOnyomi.text = data.onyomi;
                txtOnyomi.ForceMeshUpdate();
            }
            if(txtKunyomi != null)
            {
                txtKunyomi.text = data.kunyomi;
                txtKunyomi.ForceMeshUpdate();
            }
            if(txtContoh != null)
            {
                txtContoh.text = data.contoh;
                txtContoh.ForceMeshUpdate();
            }

            currentAudioFile = data.audioFile;

            // Tampilkan panel setelah semua data di-update
            if(panelInfo != null) panelInfo.SetActive(true);
            
            DebugLog("UI Updated - Panel ditampilkan dengan data: " + data.kanji);
        }
        else
        {
            Debug.LogWarning("Data tidak ditemukan untuk ID: " + markerID);
        }
    }

    public void HidePanel()
    {
        HidePanelForMarker("");
    }
    
    // Overload untuk validation - hanya hide jika marker yang memanggil adalah marker aktif
    public void HidePanelForMarker(string markerID)
    {
        // PERBAIKAN: Hanya hide jika tidak ada markerID (force hide) atau markerID cocok dengan yang aktif
        if (!string.IsNullOrEmpty(markerID) && markerID != currentTrackedMarker)
        {
            DebugLog("HidePanel diabaikan - Marker " + markerID + " bukan marker aktif (" + currentTrackedMarker + ")");
            return;
        }
        
        DebugLog("HidePanel dipanggil - Marker: " + currentTrackedMarker);
        
        if(panelInfo != null) panelInfo.SetActive(false);
        
        // Reset semua data
        if(txtKanji != null) txtKanji.text = "";
        if(txtArti != null) txtArti.text = "";
        if(txtOnyomi != null) txtOnyomi.text = "";
        if(txtKunyomi != null) txtKunyomi.text = "";
        if(txtContoh != null) txtContoh.text = "";
        currentAudioFile = "";
        currentTrackedMarker = "";
        
        DebugLog("Panel berhasil di-hide dan data di-reset");
    }

    // Play audio kanji
    public void PlayAudio()
    {
        if (!string.IsNullOrEmpty(currentAudioFile))
        {
            // Load dari Resources/SuaraKanji
            AudioClip clip = Resources.Load<AudioClip>("SuaraKanji/" + currentAudioFile); 
            
            if (clip != null && audioSource != null) 
            {
                audioSource.PlayOneShot(clip);
            }
            else
            {
                Debug.LogWarning("Audio belum ada atau salah nama: " + currentAudioFile);
            }
        }
    }

    void Update()
    {
        // Deteksi tombol Back HP
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }

    void HandleBackButton()
    {
        // Kalo panel About terbuka, tutup dulu
        if (panelAbout != null && panelAbout.activeSelf)
        {
            panelAbout.SetActive(false);
        }
        // Kalo ga ada panel, baru balik ke menu
        else
        {
            BackToMainMenu();
        }
    }

    public void BackToMainMenu()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayClick();
        
        string sceneName = GameConstants.SCENE_MAIN_MENU;
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError($"Scene '{sceneName}' tidak ditemukan di Build Settings!");
        }
    }
    
    // Conditional debug logging (hanya aktif di Editor)
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    void DebugLog(string message)
    {
        Debug.Log(message);
    }
}