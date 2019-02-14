using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.Experimental.UIElements;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FastLoad();
        }
    }


    private static bool m_fastLoaded = false;
    public static void FastLoad()
    {

#if UNITY_EDITOR
        if (!m_fastLoaded && EditorApplication.isPlaying)
#else
        if (!m_fastLoaded)
#endif
            if ( !m_fastLoaded)
        {
            m_fastLoaded = true;

            Animator lightManager = FindObjectOfType<LightingManager>().GetComponent<Animator>();

            if (lightManager)
            {
                lightManager.Play("DEBUG");
            }

            foreach (DropZone dropzone in GameObject.FindObjectsOfType<DropZone>())
            {
                dropzone.enabled = true;
            }

            foreach (MTK_TPZone tpz in GameObject.FindObjectsOfType<MTK_TPZone>())
            {
                tpz.Appears(false);
            }

            GameObject.FindObjectOfType<Constellation>().Complete();
        }
    }

}
