using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_DebugGesture : Spawner
{
    public override void SpawnIntParameter(int parameter)
    {
        GameObject go = Spawn();

		go.GetComponent<TextMesh>().text = parameter.ToString();
    }
}
