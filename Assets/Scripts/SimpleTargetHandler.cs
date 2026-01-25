using UnityEngine;
using Vuforia;

public class SimpleTargetHandler : MonoBehaviour
{
    private ObserverBehaviour mObserverBehaviour;
    private bool isCurrentlyTracked = false;

    void Start()
    {
        // Cari komponen Vuforia
        mObserverBehaviour = GetComponent<ObserverBehaviour>();
        if (mObserverBehaviour)
        {
            mObserverBehaviour.OnTargetStatusChanged += OnTargetStatusChanged;
        }
    }

    private void OnTargetStatusChanged(ObserverBehaviour behaviour, TargetStatus targetStatus)
    {
        string myName = gameObject.name;
        
        // Marker terdeteksi (TRACKED atau EXTENDED_TRACKED)
        if (targetStatus.Status == Status.TRACKED || targetStatus.Status == Status.EXTENDED_TRACKED)
        {
            // Tampilkan panel kalo belum tracked sebelumnya
            if (!isCurrentlyTracked)
            {
                DebugLog("Marker Terdeteksi: " + myName + " (Status: " + targetStatus.Status + ")");
                
                if (ARManager.Instance != null)
                {
                    ARManager.Instance.ShowKanjiData(myName);
                }
                
                isCurrentlyTracked = true;
            }
        }
        // Marker hilang
        else
        {
            if (isCurrentlyTracked)
            {
                DebugLog("Marker Hilang: " + myName + " (Status: " + targetStatus.Status + ")");
                
                if (ARManager.Instance != null) 
                {
                    // PERBAIKAN: Pass markerID untuk validation
                    ARManager.Instance.HidePanelForMarker(myName);
                }
                
                isCurrentlyTracked = false;
            }
        }
    }
    
    void OnDisable()
    {
        // Hide panel kalo marker di-disable
        if (isCurrentlyTracked && ARManager.Instance != null)
        {
            ARManager.Instance.HidePanelForMarker(gameObject.name);
            isCurrentlyTracked = false;
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe untuk prevent memory leak
        if (mObserverBehaviour != null)
        {
            mObserverBehaviour.OnTargetStatusChanged -= OnTargetStatusChanged;
        }
        
        // Cleanup tracking state
        if (isCurrentlyTracked && ARManager.Instance != null)
        {
            ARManager.Instance.HidePanelForMarker(gameObject.name);
        }
    }
    
    // Conditional debug logging (hanya aktif di Editor)
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    void DebugLog(string message)
    {
        Debug.Log(message);
    }
}