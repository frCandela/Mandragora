using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_Manager : MonoBehaviour
{
    [HideInInspector] public MTK_Setup activeSetup = null;

    // Use this for initialization
    void Awake ()
    {            
        // Activates the first MTK_Setup in the child hierarchy
        for (int i = 0; i < transform.childCount; ++i)
        {
            MTK_Setup setup = transform.GetChild(i).GetComponent<MTK_Setup>();
            if ( ! activeSetup && setup)
            {
                activeSetup = setup;
                setup.gameObject.SetActive(true);
            }
            else
                setup.gameObject.SetActive(false);
        }        
	}
	


}
