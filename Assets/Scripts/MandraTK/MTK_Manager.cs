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
    MTK_Setup setupSimulation;
    [SerializeField]
    MTK_Setup setupSteamVR;

    [SerializeField]
    GameObject m_sharedControllerRight, m_sharedControllerLeft;

    private void Awake()
    {
        Util.EditorAssert(activeSetup != null, "Please select a MTK_Setup in the MTK_Manager");

        // Init
        m_sharedControllerRight.transform.SetParent(activeSetup.rightHand.transform, false);
        m_sharedControllerLeft.transform.SetParent(activeSetup.leftHand.transform, false);
    }

    public void SwitchSetup()
    {
        #if UNITY_EDITOR
        if (!EditorApplication.isPlaying)
        {
            activeSetup = (activeSetup == setupSteamVR) ? setupSimulation : setupSteamVR;
            activeSetup.gameObject.SetActive(true);
            activeSetup.UpdateSettings();
            // Activates the first MTK_Setup in the child hierarchy
            foreach (MTK_Setup setup in FindObjectsOfType<MTK_Setup>())
            {
                if (setup != activeSetup)
                    setup.gameObject.SetActive(false);
            }
        }
        #endif
    }
}
