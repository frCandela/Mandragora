using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour
{
    public float speed = 0.1f;

    public Vector3 m_top = new Vector3();
    public Vector3 m_bottom = new Vector3();

    public float liquidVolume = 0f;
    public float maxDiameter = 0.3f;

    public void UpdateTransform( Vector3 top, Vector3 bottom)
    {
        m_top = top;
        m_bottom = bottom;

        float d = 0.5f * (top.y - bottom.y);

        transform.localScale = new Vector3(transform.localScale.x, d, transform.localScale.z);
        transform.position = new Vector3(top.x, top.y - d, top.z);
    }

    // Update is called once per frame
    void Update ()
    {
        m_top -= 0.8f*speed * Vector3.up;

        UpdateTransform(m_top, m_bottom);

        Debug.DrawLine(m_bottom, m_bottom - Vector3.forward, Color.blue);

        RaycastHit hit;
         if ( Physics.Raycast(m_bottom, Vector3.down, out hit,  speed))
         {
            Debug.DrawLine(m_bottom, hit.point, Color.red);


            WithLiquid wl = hit.collider.gameObject.GetComponent<WithLiquid>();
            if (wl )
            {
                float delta = Mathf.Min(0.5f * speed, liquidVolume) ;


                wl.Fill(delta);


                liquidVolume -= delta;
            }            
         }	
         else
         {
                m_bottom -= speed * Vector3.up; 
         }
        if (m_top.y < m_bottom.y)
        {
            Destroy(gameObject);
        }
    }
}
