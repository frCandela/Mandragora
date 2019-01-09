using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MTK_PlanetMasterJoint : MTK_JointType_Fixed
{
    private bool m_used = false;
    private MTK_JointType m_jointUsed;

    public override bool Used()
    {
        return m_used || base.Used();
    }

    public bool SetUsed( bool state, MTK_JointType joint )
    {
        if( state )
        {
            if(m_used)
            {
                return false;
            }
            else
            {
                m_used = true;
                m_jointUsed = joint;
                return true;
            }
            
        }
        else
        {
            if (m_used)
            {
                m_used = false;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

}
