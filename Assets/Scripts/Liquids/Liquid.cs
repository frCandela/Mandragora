using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour
{
    [Header("Liquid properties")]
    [SerializeField] private float speed = 0.1f;
    [SerializeField] public Effect effectPrefab = null;

    public Vector3 top { get{ return m_top;} private set { } }
    public Vector3 bottom { get { return m_bottom; } private set { } }

    private float m_liquidVolume = 0f;
    private Vector3 m_top = new Vector3();
    private Vector3 m_bottom = new Vector3();
    private Effect m_liquidEffectPrefab = null;

    // Adds liquid
    public void AddLiquid(float volume)
    {
        m_liquidVolume += volume;
    }

    // Update the liquid to make it expand from top to bottom
    public void SetTransform( Vector3 top, Vector3 bottom)
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
        // Update transform
        m_top -= 0.8f*speed * Vector3.up;
        SetTransform(m_top, m_bottom);

        // Raycast down
        RaycastHit hit;
         if ( Physics.Raycast(m_bottom, Vector3.down, out hit,  speed))
         {
            if(effectPrefab)
            {
                hit.collider.gameObject.AddComponent(effectPrefab.GetType());
            }


            // If hits a liquid container
            WithLiquid wl = hit.collider.gameObject.GetComponent<WithLiquid>();
            if (wl )
            {
                float delta = Mathf.Min(0.5f * speed, m_liquidVolume) ;
                wl.Fill(delta);
                m_liquidVolume -= delta;
            }            
         }	
         else
         {
            m_bottom -= speed * Vector3.up; 
         }

        // Destroyed when the top reachs the ground
        if (m_top.y <= m_bottom.y)
        {
            Destroy(gameObject);
        }
    }

}
