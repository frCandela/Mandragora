using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Colonisator : MonoBehaviour
{
    [SerializeField, Range(0, 79)] public int selectedSegment = 0;


    [SerializeField] private GameObject m_room;

    private void Awake()
    {
        Assert.IsTrue(m_room);
    }


    /*[SerializeField] private GameObject m_prefab;
    [SerializeField] private IcoPlanet m_icoPlanet;
    [SerializeField] private GameObject m_previousScene;
    [SerializeField] private GameObject m_managers;
    [SerializeField] private float m_planetScale = 100;*/




    private bool m_colonized = false;

    // Update is called once per frame

    void Update()
    {
        /*Vector3 center = m_icoPlanet.Segments[selectedSegment].Center();
        Vector3 normal = m_icoPlanet.Segments[selectedSegment].Normal();

        if (m_colonized)
        {           
            m_icoPlanet.transform.localRotation = Quaternion.FromToRotation(normal, Vector3.up);
            m_managers.transform.position = m_icoPlanet.transform.TransformPoint(center);
        }*/
    }


    [ContextMenu("Colonize")]
    void Colonize()
    {
        
        m_colonized = true;

        print("YOLOOO");


        /* m_icoPlanet.transform.position = Vector3.zero;
         m_icoPlanet.transform.rotation = Quaternion.identity;
         m_icoPlanet.GetComponent<Rigidbody>().isKinematic = true;
         Destroy(m_icoPlanet.GetComponent<MTK_Interactable>());

         foreach ( IcoSegment segment in m_icoPlanet.Segments)
         {
             Destroy(segment.GetComponent<MTK_Interactable>());
         }

         m_previousScene.SetActive(false);

         m_icoPlanet.transform.localScale = new Vector3(1, 1, 1);
         m_icoPlanet.m_nbSubdivisions = 3;
         m_icoPlanet.UpdateTesselationLevel();
         m_icoPlanet.transform.localScale = new Vector3(m_planetScale, m_planetScale, m_planetScale);


         foreach(MTK_InteractHand hand in FindObjectsOfType<MTK_InteractHand>())
         {
             hand.gameObject.AddComponent<HandTerraformer>();

             GameObject laser = Instantiate(m_prefab);
             laser.transform.parent = hand.transform;
             laser.transform.localPosition = new Vector3(0, 0, 50);
             laser.transform.localScale = new Vector3(0.01f, 0.01f, 100);
             laser.transform.localRotation = Quaternion.identity;
         }*/

    }
}
