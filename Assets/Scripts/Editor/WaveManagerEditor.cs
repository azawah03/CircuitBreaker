using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveManager))]
public class WaveManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Wave Control", EditorStyles.boldLabel);
        
        WaveManager waveManager = (WaveManager)target;
        
        if (Application.isPlaying)
        {
            EditorGUILayout.LabelField($"Current Wave: {waveManager.CurrentWave}/{waveManager.TotalWaves}");
            EditorGUILayout.LabelField($"Wave Active: {waveManager.IsWaveActive}");
            EditorGUILayout.LabelField($"Enemies Remaining: {waveManager.EnemiesRemaining}");
            
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Force Next Wave"))
            {
                waveManager.ForceNextWave();
            }
            
            if (GUILayout.Button("Restart Waves"))
            {
                waveManager.RestartWaves();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Wave controls are only available during play mode.", MessageType.Info);
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Setup", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Create Sample Wave Configuration"))
        {
            CreateSampleWaveConfig(waveManager);
        }
    }
    
    void CreateSampleWaveConfig(WaveManager waveManager)
    {
        // Create a basic 5-wave configuration
        waveManager.waves = new WaveConfig[5];
        
        for (int i = 0; i < 5; i++)
        {
            waveManager.waves[i] = new WaveConfig();
            waveManager.waves[i].waveName = $"Wave {i + 1}";
            waveManager.waves[i].waveNumber = i + 1;
            waveManager.waves[i].timeBetweenEnemySpawns = Mathf.Max(0.5f, 2f - (i * 0.3f)); // Faster spawning each wave
            waveManager.waves[i].timeBeforeNextWave = 8f;
            
            // Difficulty scaling
            waveManager.waves[i].enemySpeedMultiplier = 1f + (i * 0.1f);
            waveManager.waves[i].enemyHealthMultiplier = 1f + (i * 0.15f);
            waveManager.waves[i].enemyDamageMultiplier = 1f + (i * 0.05f);
            
            // Create sample enemy wave data (you'll need to assign actual enemy types)
            waveManager.waves[i].enemyWaves = new EnemyWaveData[1];
            waveManager.waves[i].enemyWaves[0] = new EnemyWaveData();
            waveManager.waves[i].enemyWaves[0].enemyCount = 3 + (i * 2); // More enemies each wave
            waveManager.waves[i].enemyWaves[0].spawnDelay = 0f;
            waveManager.waves[i].enemyWaves[0].spawnSimultaneously = false;
        }
        
        EditorUtility.SetDirty(waveManager);
        Debug.Log("Sample wave configuration created! Don't forget to assign enemy types in the inspector.");
    }
}
