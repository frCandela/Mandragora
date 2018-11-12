using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AirSig;
using VRTK;

public class GestureHandler : MonoBehaviour
{
	[SerializeField] AirSigManager m_airsigManager;

	[SerializeField] int MAX_TRAIN_COUNT = 5;
	int m_currentGestureID = 100;
	List<int> m_allGestureIDs = new List<int>(10);

	[SerializeField] UnityEventInt m_onGestureRecognition;

	int m_lastMatch = 0;

	public bool Collecting
	{
		set
		{
			if(value)
				m_airsigManager.startCollecting();
			else
				m_airsigManager.stopCollecting();
		}
	}

	void Awake()
	{
		m_airsigManager.onPlayerGestureAdd += HandleOnPlayerGestureAdd;
		m_airsigManager.onPlayerGestureMatch += HandleOnPlayerGestureMatch;
	}

	void Update()
	{
		if(m_lastMatch != 0)
		{
			m_onGestureRecognition.Invoke(m_lastMatch);
			m_lastMatch = 0;
		}
	}

	public void InitTracker(AngularVelocityTracker tracker)
	{
		m_airsigManager.m_angularTracker = tracker;
    }

	[ContextMenu("AddGesture")]
	public void AddGesture()
	{
		m_currentGestureID++;
		m_allGestureIDs.Add(m_currentGestureID);

        m_airsigManager.SetMode(AirSigManager.Mode.AddPlayerGesture);
        m_airsigManager.SetTarget(new List<int>{m_currentGestureID});
    }

	public void DeleteGesture(int target)
	{
		m_airsigManager.DeletePlayerRecord(target);
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
		print("GestureID : " + gestureId + " - Match : " + match + " - Exist : " + m_airsigManager.IsPlayerGestureExisted(m_airsigManager.GetFromCache(gestureId)));
		
		m_lastMatch = match;

		if(m_lastMatch > 0)
			m_lastMatch -= 100;
    }
}
