using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

// [ExecuteInEditMode]
public class MTK_Manager : MonoBehaviour
{
    public MTK_Setup activeSetup = null;

    [Header("Available setups")]
    [SerializeField]
    public MTK_Setup setupSimulation;
    [SerializeField]
    public MTK_Setup setupSteamVR;

    private void Awake()
    {
        foreach (MTK_Setup setup in FindObjectsOfType<MTK_Setup>())
        {
            if (setup.enabled)
                activeSetup = setup;
        }
    }

    public void SetSetup(bool sim)
    {
        activeSetup = sim ? setupSimulation : setupSteamVR;
        activeSetup.gameObject.SetActive(true);
        activeSetup.UpdateSettings();
        
        foreach (MTK_Setup setup in FindObjectsOfType<MTK_Setup>())
        {
            if (setup != activeSetup)
                setup.gameObject.SetActive(false);
        }
    }
}
