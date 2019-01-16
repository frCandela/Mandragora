using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colonisator : MonoBehaviour
{
    [SerializeField] private IcoPlanet m_icoPlanet;
    [SerializeField] private GameObject m_previousScene;
    [SerializeField] private GameObject m_managers;


    [SerializeField] private float m_planetScale = 100;
    [SerializeField, Range(0,79)] public int selectedSegment = 0;

    private bool m_colonized = false;

    // Update is called once per frame
    public bool test = false;
    void Update ()
    {
        if( Input.GetKeyDown( KeyCode.X ) || test)
        {
            test = false;
            Colonize();
        }

        Vector3 center = m_icoPlanet.Segments[selectedSegment].Center();
        Vector3 normal = m_icoPlanet.Segments[selectedSegment].Normal();

        Debug.DrawLine(m_icoPlanet.transform.position, m_icoPlanet.transform.TransformPoint(center));
        Debug.DrawLine(m_icoPlanet.transform.TransformPoint(center), m_icoPlanet.transform.TransformPoint(center) + m_icoPlanet.transform.TransformPoint(normal), Color.red);
       

        if (m_colonized)
        {           
            m_icoPlanet.transform.localRotation = Quaternion.FromToRotation(normal, Vector3.up);
            m_managers.transform.position = m_icoPlanet.transform.TransformPoint(center);
        } 
    }

    void Colonize()
    {
        m_colonized = true;
        print("YOLOOO");
        m_icoPlanet.transform.localScale = new Vector3(m_planetScale, m_planetScale, m_planetScale);
        m_icoPlanet.transform.position = Vector3.zero;
        m_icoPlanet.GetComponent<Rigidbody>().isKinematic = true;
        Destroy(m_icoPlanet.GetComponent<MTK_Interactable>());


        foreach( IcoSegment segment in m_icoPlanet.Segments)
        {
            Destroy(segment.GetComponent<MTK_Interactable>());
        }

        m_previousScene.SetActive(false);
    }    
}
