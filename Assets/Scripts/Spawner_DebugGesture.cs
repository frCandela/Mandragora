using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_DebugGesture : Spawner
{
    public override void SpawnIntParameter(int parameter)
    {
        TextMesh tm = Spawn().GetComponent<TextMesh>();

        if(parameter < 0)
        {
            tm.text = "?";
		    tm.color = Color.red;
        }
        else
        {
            tm.text = parameter.ToString();
		    tm.color = Color.green;
        }

        tm.transform.LookAt(Camera.main.transform);
        tm.transform.localScale = new Vector3(-1,1,1);

        Destroy(tm.gameObject, 1);
    }
}
