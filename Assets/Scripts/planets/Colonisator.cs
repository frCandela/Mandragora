using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Colonisator : MonoBehaviour
{
    [SerializeField] public int selectedSegment = 0;

    [SerializeField] private float m_planetScale = 100;

    [SerializeField] private GameObject m_room;
    [SerializeField] private GameObject m_newRoom;
    [SerializeField] private GameObject m_laserPrefab;

    private IcoPlanet m_icoPlanet;
    private MTK_Manager m_manager;
    private MTK_Setup m_setup;

    private void Awake()
    {
        Assert.IsTrue(m_room);
        Assert.IsTrue(m_newRoom);
        Assert.IsTrue(m_laserPrefab);
        Assert.IsTrue(transform.position == Vector3.zero, "Colonisator gameobject position must be zero");
        Assert.IsTrue(transform.rotation == Quaternion.identity, "Colonisator gameobject rotation must be zero");
        Assert.IsTrue(transform.lossyScale == Vector3.one, "Colonisator gameobject scale must be one");

        m_manager = FindObjectOfType<MTK_Manager>();
        m_setup = m_manager.activeSetup;
        m_newRoom.SetActive(false);
    }

    private bool m_colonized = false;

    // Update is called once per frame
    void Update()
    {
        if(m_colonized)
        {
            Vector3 center = m_icoPlanet.Segments[selectedSegment].Center();
            Vector3 normal = m_icoPlanet.Segments[selectedSegment].Normal();

            if (m_colonized)
            {
                m_icoPlanet.transform.localRotation = Quaternion.FromToRotation(normal, Vector3.up);
                m_manager.transform.position = m_icoPlanet.transform.TransformPoint(center);
            }
        }
    }

    IcoPlanet FindPlanet()
    {
        IcoPlanet[] planets = FindObjectsOfType<IcoPlanet>();
        if(planets.Length == 0)
        {
            return null;
        }

        foreach( IcoPlanet planet in planets)
        {
            if( planet.isActiveAndEnabled)
            {
                return planet;
            }
        }
        return planets[0];
    }

    void InitializePlanet()
    {

    }

    void CleanupPlanet( IcoPlanet planet )
    {
        // Set transforms
        planet.GetComponent<Rigidbody>().isKinematic = true; ;

        planet.transform.parent = transform;
        planet.transform.rotation = Quaternion.identity;
        planet.transform.position = Vector3.zero;

        foreach(IcoSegment segment in planet.Segments)
        {
            segment.transform.localScale = Vector3.one;
        }
        planet.transform.localScale = Vector3.one;


        // Destroy useless components
        Destroy(planet.GetComponent<MTK_Interactable>());
        Destroy(planet.GetComponent<MeshCollider>());
        Destroy(planet.GetComponent<Outline>());
        foreach (MTK_JointType joint in planet.GetComponents< MTK_JointType>())
            Destroy(joint);

        foreach (IcoSegment segment in planet.Segments)
        {
            segment.GetComponent<MeshCollider>().enabled = true;
            Destroy(segment.GetComponent<Outline>());
            Destroy(segment.GetComponent<MTK_Interactable>());
            foreach (MTK_JointType joint in segment.GetComponents<MTK_JointType>())
                Destroy(joint);
        }
    }

    [ContextMenu("Colonize")]
    public void Colonize()
    {
        m_icoPlanet = FindPlanet();
        if ( ! m_icoPlanet)
        {
            print("Failed to colonize space");
            return;
        }

        AkSoundEngine.PostEvent("Play_Amb_TP", gameObject);

        m_colonized = true;

        m_room.SetActive(false);

        CleanupPlanet(m_icoPlanet);

        m_icoPlanet.heightDelta /= 4;
        m_icoPlanet.nbLevels *= 4;
        m_icoPlanet.SetTesselationLevel(3, 4);
        m_icoPlanet.transform.localScale = new Vector3(m_planetScale, m_planetScale, m_planetScale);

        m_manager.transform.position = Vector3.zero;
        m_manager.transform.rotation = Quaternion.identity;
        m_setup.transform.position = Vector3.zero;
        m_setup.transform.rotation = Quaternion.identity;

        foreach (MTK_InteractHand hand in FindObjectsOfType<MTK_InteractHand>())
        {
            hand.gameObject.AddComponent<HandTerraformer>();

            GameObject laser = Instantiate(m_laserPrefab);
            laser.transform.parent = hand.transform;
            laser.transform.localPosition = new Vector3(0, 0, 50);
            laser.transform.localScale = new Vector3(0.01f, 0.01f, 100);
            laser.transform.localRotation = Quaternion.identity;
        }

        m_newRoom.SetActive(true);

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

*/

    }
}
