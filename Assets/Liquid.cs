using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour
{
    public float speed = 0.1f;

    public Vector3 m_top = new Vector3();
    public Vector3 m_bottom = new Vector3();

    float test = 0;

    public float liquidVolume = 0f;
    public float maxDiameter = 0.1f;

    public void UpdateTransform( Vector3 top, Vector3 bottom)
    {
        m_top = top;
        m_bottom = bottom;

        float d = 0.5f * (top.y - bottom.y);

        float diameter;
        if (d > 0)
            diameter = Mathf.Min(maxDiameter, liquidVolume / (2f * d));
        else
            diameter = 0f;

        transform.localScale = new Vector3(diameter, d, diameter);
        transform.position = new Vector3(top.x, top.y - d, bottom.z);
    }

    // Update is called once per frame
    void Update ()
    {
        m_top -= 0.5f*speed * Vector3.up;

        UpdateTransform(m_top, m_bottom);

        Debug.DrawLine(m_bottom, m_bottom - Vector3.forward, Color.blue);

        RaycastHit hit;
         if ( Physics.Raycast(m_bottom, Vector3.down, out hit,  speed))
         {
            Debug.DrawLine(transform.position, hit.point, Color.red);

            float delta = speed * transform.localScale.x;
            WithLiquid wl = hit.collider.gameObject.GetComponent<WithLiquid>();
            if (wl )
            {
                wl.liquidHeight += liquidVolume;
                liquidVolume = 0;
            }
            else
                print("zob");

            
         }	
         else
        {
            m_bottom -= speed * Vector3.up; 
        }

        if (m_top.y < m_bottom.y)
        {
            print(liquidVolume);
            Destroy(gameObject);
        }


    }
}
