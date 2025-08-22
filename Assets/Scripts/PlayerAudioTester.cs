using UnityEngine;

public class PlayerAudioTester : MonoBehaviour
{
    [Header("Test Player Audio")]
    public KeyCode testWalkKey = KeyCode.F1;
    public KeyCode testRunKey = KeyCode.F2;
    public KeyCode testJumpKey = KeyCode.F3;
    public KeyCode testLandingKey = KeyCode.F4;
    
    private PlayerAudioManager playerAudioManager;
    
    void Start()
    {
        playerAudioManager = FindObjectOfType<PlayerAudioManager>();
    }
    
    void Update()
    {
        if (playerAudioManager == null) return;
        
        if (Input.GetKeyDown(testWalkKey))
        {
            TestWalkFootstep();
        }
        
        if (Input.GetKeyDown(testRunKey))
        {
            TestRunFootstep();
        }
        
        if (Input.GetKeyDown(testJumpKey))
        {
            TestJumpSound();
        }
        
        if (Input.GetKeyDown(testLandingKey))
        {
            TestLandingSound();
        }
    }
    
    void TestWalkFootstep()
    {
        if (playerAudioManager.walkFootsteps != null && playerAudioManager.walkFootsteps.Length > 0)
        {
            AudioClip randomClip = playerAudioManager.walkFootsteps[Random.Range(0, playerAudioManager.walkFootsteps.Length)];
            if (randomClip != null && playerAudioManager.footstepAudioSource != null)
            {
                AudioManager.PlaySFX(playerAudioManager.footstepAudioSource, randomClip, playerAudioManager.footstepVolume);
                Debug.Log("Played walk footstep");
            }
        }
    }
    
    void TestRunFootstep()
    {
        if (playerAudioManager.runFootsteps != null && playerAudioManager.runFootsteps.Length > 0)
        {
            AudioClip randomClip = playerAudioManager.runFootsteps[Random.Range(0, playerAudioManager.runFootsteps.Length)];
            if (randomClip != null && playerAudioManager.footstepAudioSource != null)
            {
                AudioManager.PlaySFX(playerAudioManager.footstepAudioSource, randomClip, playerAudioManager.footstepVolume);
                Debug.Log("Played run footstep");
            }
        }
    }
    
    void TestJumpSound()
    {
        if (playerAudioManager.jumpSound != null && playerAudioManager.jumpAudioSource != null)
        {
            AudioManager.PlaySFX(playerAudioManager.jumpAudioSource, playerAudioManager.jumpSound, playerAudioManager.jumpVolume);
            Debug.Log("Played jump sound");
        }
    }
    
    void TestLandingSound()
    {
        if (playerAudioManager.landingSound != null && playerAudioManager.jumpAudioSource != null)
        {
            AudioManager.PlaySFX(playerAudioManager.jumpAudioSource, playerAudioManager.landingSound, playerAudioManager.landingVolume);
            Debug.Log("Played landing sound");
        }
    }
    
    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 220, 400, 200));
        GUILayout.Label("Player Audio Tester");
        GUILayout.Label($"Press {testWalkKey} to test walk footstep");
        GUILayout.Label($"Press {testRunKey} to test run footstep");
        GUILayout.Label($"Press {testJumpKey} to test jump sound");
        GUILayout.Label($"Press {testLandingKey} to test landing sound");
        
        if (playerAudioManager != null)
        {
            GUILayout.Label("Player Audio Manager found ✓");
        }
        else
        {
            GUILayout.Label("Player Audio Manager NOT found ✗");
        }
        
        GUILayout.EndArea();
    }
}
