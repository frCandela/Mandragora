using UnityEditor;
using UnityEngine;

namespace UnityToolbarExtender
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageAbove,
				fontStyle = FontStyle.Bold
			};
		}
	}
	
	[InitializeOnLoad]
	static class SimulationSwitch
	{		
		static MTK_Manager m_mtkManager;
		static Animator m_lightManager;
		static string m_keyPrefName = "SimulationMode";

		static bool EnableSimulation
		{
			get
			{
				return EditorPrefs.GetBool(m_keyPrefName, false);
			}
			set
			{
				if(!m_mtkManager)
					m_mtkManager = GameObject.FindObjectOfType<MTK_Manager>();

				if(m_mtkManager && ! EditorApplication.isPlaying)
				{
					m_mtkManager.SetSetup(value);
					EditorPrefs.SetBool(m_keyPrefName, value);
				}
			}
		}

		static bool EnableFast
		{
			get
			{
				return false;
			}
			set
			{
				if(value)
				{
					if(!m_lightManager)
					m_lightManager = GameObject.FindObjectOfType<LightingManager>().GetComponent<Animator>();

					if(m_lightManager && EditorApplication.isPlaying)
						m_lightManager.Play("DEBUG");
				}
			}
		}

		static SimulationSwitch()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}
		
		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			EnableFast = GUILayout.Toggle(false, new GUIContent("F", "Fast Mode"), ToolbarStyles.commandButtonStyle);
			EnableSimulation = GUILayout.Toggle(EnableSimulation, new GUIContent("S", "Simulation Mode"), ToolbarStyles.commandButtonStyle);
		}
	}
}