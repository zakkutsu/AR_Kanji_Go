using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Sumber Suara")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Koleksi Klip")]
    public AudioClip clipClick;
    public AudioClip clipBenar;
    public AudioClip clipSalah;
    
    // 1. TAMBAHKAN BARIS INI (Slot baru buat suara jeng-jeng)
    public AudioClip clipResult; 

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayClick() { sfxSource.PlayOneShot(clipClick); }
    public void PlayBenar() { sfxSource.PlayOneShot(clipBenar); }
    public void PlaySalah() { sfxSource.PlayOneShot(clipSalah); }

    // 2. TAMBAHKAN FUNGSI INI
    public void PlayResult()
    {
        // PlayOneShot = Mainkan sekali tanpa memutus BGM
        sfxSource.PlayOneShot(clipResult);
    }
}