using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using AirSig;
using VRTK;

public class GestureHandler : MonoBehaviour
{
	AirSigManager m_airsigManager;
	[SerializeField] AngularVelocityTracker m_tracker;

	[SerializeField] int MAX_TRAIN_COUNT = 5;
	int m_currentGestureID = 100;
	List<int> m_allGestureIDs = new List<int>(10);

	[SerializeField] UnityEventInt m_onGestureRecognition;
	[SerializeField] UnityEvent m_onGestureRecognitionSucceeded;
	[SerializeField] UnityEvent m_onGestureRecognitionFailed;
	[SerializeField] UnityEventBool m_onCollecting;

	int m_lastMatch = 0;

	public bool Collecting
	{
		set
		{
			if(value)
				m_airsigManager.startCollecting();
			else
				m_airsigManager.stopCollecting();

			m_onCollecting.Invoke(value);
		}
	}

	public bool Active
	{
		set
		{
			if(value)
			{
				m_airsigManager.onPlayerGestureAdd += HandleOnPlayerGestureAdd;
				m_airsigManager.onPlayerGestureMatch += HandleOnPlayerGestureMatch;

				m_airsigManager.m_angularTracker = m_tracker;
				AddGesture();
			}
			else
			{
				m_airsigManager.onPlayerGestureAdd -= HandleOnPlayerGestureAdd;
				m_airsigManager.onPlayerGestureMatch -= HandleOnPlayerGestureMatch;

				DeleteGestures();
			}
		}
	}

	void Start()
	{
		m_airsigManager = AirSigManager.sInstance;
	}

	void Update()
	{
		// Invoke Ddbug Messages if any
		if(m_lastMatch != 0)
		{
			if(m_lastMatch > 0)
				m_onGestureRecognitionSucceeded.Invoke();
			else
				m_onGestureRecognitionFailed.Invoke();

			m_onGestureRecognition.Invoke(m_lastMatch);
			m_lastMatch = 0;
		}
	}

	[ContextMenu("AddGesture")]
	void AddGesture()
	{
		m_currentGestureID++;
		m_allGestureIDs.Add(m_currentGestureID);

        m_airsigManager.SetMode(AirSigManager.Mode.AddPlayerGesture);
        m_airsigManager.SetTarget(new List<int>{m_currentGestureID});
    }

	void DeleteGestures()
	{
		foreach (int gestureID in m_allGestureIDs)
			m_airsigManager.DeletePlayerRecord(gestureID);
	}

	void SwitchToIdentify()
	{
        m_airsigManager.SetPlayerGesture(m_allGestureIDs, true);

        m_airsigManager.SetMode(AirSigManager.Mode.IdentifyPlayerGesture);
        m_airsigManager.SetTarget(m_allGestureIDs);
    }

	void HandleOnPlayerGestureAdd(long gestureId, Dictionary<int, int> result)
	{
        int count = result[m_currentGestureID];

		if(count >= MAX_TRAIN_COUNT)
			SwitchToIdentify();
    }

	void HandleOnPlayerGestureMatch(long gestureId, int match)
	{
		// print("GestureID : " + gestureId + " - Match : " + match + " - Exist : " + m_airsigManager.IsPlayerGestureExisted(m_airsigManager.GetFromCache(gestureId)));
		
		m_lastMatch = match;

		if(m_lastMatch > 0)
			m_lastMatch -= 100;
    }
}
