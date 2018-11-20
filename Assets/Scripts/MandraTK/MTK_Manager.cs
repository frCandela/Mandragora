using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class MTK_Manager : MonoBehaviour
{
    public MTK_Setup activeSetup = null;

    [Header("Available setups")]
    [SerializeField]
    MTK_Setup setupSimulation;
    [SerializeField]
    MTK_Setup setupSteamVR; 

    private void Awake()
    {
        Util.EditorAssert(activeSetup != null, "Please select a MTK_Setup in the MTK_Manager");
    }

    public void SwitchSetup()
    {
        if(activeSetup)
        {
            activeSetup = (activeSetup == setupSteamVR) ? setupSimulation : setupSteamVR;
            OnValidate();
        }
    }

    private void OnValidate()
    {
        if (activeSetup && ! EditorApplication.isPlaying)
        {
            activeSetup.gameObject.SetActive(true);
            activeSetup.UpdateSettings();
            // Activates the first MTK_Setup in the child hierarchy
            foreach (MTK_Setup setup in FindObjectsOfType<MTK_Setup>())
            {
                if (setup != activeSetup)
                {
                    setup.gameObject.SetActive(false);
                }
            }
        }
    }
}
