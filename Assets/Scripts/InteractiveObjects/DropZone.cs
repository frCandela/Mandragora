using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(Rigidbody))]
public class DropZone : MonoBehaviour
{    
    [SerializeField] private bool m_snapToCenter = true;
    [SerializeField] private float m_activationCooldown = 2f;
    [SerializeField] private float m_ejectForce = 1f;
    [SerializeField] private Animator m_workshopAnimator;
    [SerializeField] private SocleSounds m_sounds;
    [SerializeField] private AnimationCurve m_planetShineAnimCurve;
    private GameObject m_visual;

    public UnityEventBool onObjectCatched;
    public MTK_Interactable catchedObject { get; private set; }


    private Outline m_outline;
    [SerializeField] TriggerButton m_button;

    private int m_nbObjectsInTrigger = 0;
    private float m_lastActivationTime;

    Collider m_collider;

    // Use this for initialization
    void Awake ()
    {
        m_outline = GetComponent<Outline>();
        m_outline.enabled = false;

        m_collider = GetComponent<Collider>();

        m_lastActivationTime = Time.time;

        m_visual = transform.Find("dropZone_Final").gameObject;

        onObjectCatched.AddListener((bool catched) =>
        {
            m_workshopAnimator.SetBool("isOn", catched);
            m_sounds.State = catched;
        });
    }
    
    private void OnEnable()
    {
        if(!catchedObject)
        {
            m_visual.SetActive(true);
        }
    }

    private void OnDisable()
    {
        m_visual.SetActive(false);

        if(objInTrigger)
        {
            Catch(objInTrigger);
        }

        if(!catchedObject)
        {
            Release();
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if(m_nbObjectsInTrigger > 0 && !catchedObject)
        {
            m_outline.enabled = true;
        }
        else
        {
            m_outline.enabled = false;
        }        
    }

    public void EnableButton()
    {
        m_button.SetActive(true);
    }

    public void Release()
    {        
        if (catchedObject)
        {
            catchedObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

            m_lastActivationTime = Time.time;
            onObjectCatched.Invoke(false);
            catchedObject.jointType.onJointBreak.RemoveListener(Release);

            MTK_Interactable tmp = catchedObject;
            catchedObject = null;

            IcoPlanet icoplanet = tmp.GetComponent<IcoPlanet>();

            if(icoplanet)
            {
                icoplanet.Joined = false;
                StartCoroutine(AnimatePlanet(icoplanet, 0.04f, 4.2f));
            }

            tmp.jointType.RemoveJoint();
            tmp.GetComponent<Rigidbody>().AddForce(m_ejectForce * Vector3.up, ForceMode.Impulse);

            m_nbObjectsInTrigger = 0;
            objInTrigger = null;

            m_button.SetActive(false);
        }
    }

    private void OnJointBreak(float breakForce)
    {
        Release();
    }

    void Catch(MTK_Interactable interactable)
    {
        if(Time.time > m_lastActivationTime + m_activationCooldown)
        {
            if (!interactable.jointType.Used() && !catchedObject && !interactable.isDistanceGrabbed)
            {
                m_lastActivationTime = Time.time;
                if (m_snapToCenter)
                {
                    interactable.transform.position = transform.position;
                }

                interactable.jointType.JoinWith(gameObject);
                GetComponent<Joint>().breakForce = float.MaxValue;
                AkSoundEngine.PostEvent("Socle_Activated_Play", gameObject);
                catchedObject = interactable;
                interactable.jointType.onJointBreak.AddListener(Release);
                interactable.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

                IcoPlanet icoplanet = interactable.GetComponent<IcoPlanet>();

                if(icoplanet)
                {
                    icoplanet.Joined = true;
                    StartCoroutine(AnimatePlanet(icoplanet, 0.12f, 15));
                }

                m_visual.SetActive(false);
                m_nbObjectsInTrigger = 0;

                onObjectCatched.Invoke(true);
            }
        }
    }

    IEnumerator AnimatePlanet(IcoPlanet icoplanet, float targetShadowAmount, float targetLuminosity)
    {
        List<Material> materials = new List<Material>();
        Material waterMaterial = icoplanet.transform.GetChild(0).GetComponent<MeshRenderer>().material;

        foreach (Transform tr in icoplanet.transform)
            materials.Add(tr.GetComponent<MeshRenderer>().material);

        materials.RemoveAt(0);

        float baseShadowAmount = materials[0].GetFloat("_PlanetColorShadowAmount"),
                baseLuminosity = waterMaterial.GetFloat("_Luminosity");

        float currentLuminosity;

        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            currentLuminosity = Mathf.LerpUnclamped(baseShadowAmount, targetShadowAmount, m_planetShineAnimCurve.Evaluate(t));

            foreach (Material mat in materials)
                mat.SetFloat("_PlanetColorShadowAmount", currentLuminosity);

            waterMaterial.SetFloat("_Luminosity", Mathf.Lerp(baseLuminosity, targetLuminosity, m_planetShineAnimCurve.Evaluate(t)));

            yield return new WaitForEndOfFrame();
        }
    }
    

    MTK_Interactable objInTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if (enabled && other.attachedRigidbody)
        {
            MTK_Interactable interact = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (interact && interact.isDroppable)
            {
                objInTrigger = interact;
                m_nbObjectsInTrigger++;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (enabled && other.attachedRigidbody)
        {
            MTK_Interactable interact = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (interact && interact.isDroppable)
            {
                objInTrigger = null;
                m_nbObjectsInTrigger--;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (enabled && other.attachedRigidbody)
        {
            MTK_Interactable interact = other.attachedRigidbody.GetComponent<MTK_Interactable>();
            if (interact && interact.isDroppable)
            {
                Catch(interact);
            }
        }
    }
}
