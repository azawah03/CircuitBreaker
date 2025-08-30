using UnityEngine;

[CreateAssetMenu(fileName = "WaveConfiguration", menuName = "CircuitBreaker/Wave Configuration")]
public class WaveConfiguration : ScriptableObject
{
    [Header("Wave Settings")]
    public WaveConfig[] waves;
    
    [Header("Difficulty Progression")]
    [Range(0f, 1f)]
    public float speedIncrease = 0.1f; // 10% increase per wave
    [Range(0f, 1f)]
    public float healthIncrease = 0.15f; // 15% increase per wave
    [Range(0f, 1f)]
    public float damageIncrease = 0.05f; // 5% increase per wave
    
    public void ApplyDifficultyScaling()
    {
        for (int i = 0; i < waves.Length; i++)
        {
            float waveMultiplier = i + 1; // Wave 1 = 1x, Wave 2 = 2x, etc.
            
            waves[i].enemySpeedMultiplier = 1f + (speedIncrease * i);
            waves[i].enemyHealthMultiplier = 1f + (healthIncrease * i);
            waves[i].enemyDamageMultiplier = 1f + (damageIncrease * i);
        }
    }
    
    [ContextMenu("Auto Generate Sample Waves")]
    public void GenerateSampleWaves()
    {
        // This would need to be implemented in the editor
        // For now, this serves as documentation for manual setup
    }
}
