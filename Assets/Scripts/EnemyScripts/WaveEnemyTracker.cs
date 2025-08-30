using UnityEngine;

public class WaveEnemyTracker : MonoBehaviour
{
    private WaveManager waveManager;
    
    public void Initialize(WaveManager manager)
    {
        waveManager = manager;
    }
    
    void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.RegisterEnemyDestroyed(gameObject);
        }
    }
}
