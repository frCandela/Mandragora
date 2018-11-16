using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MTK_JointType : MonoBehaviour
{
    public abstract bool JoinWith(GameObject other);
    public abstract bool RemoveJoinWith(GameObject other);
}

