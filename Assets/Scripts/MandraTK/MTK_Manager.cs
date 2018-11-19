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

    private void Awake()
    {
        Util.EditorAssert(activeSetup != null, "Please select a MTK_Setup in the MTK_Manager");
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
