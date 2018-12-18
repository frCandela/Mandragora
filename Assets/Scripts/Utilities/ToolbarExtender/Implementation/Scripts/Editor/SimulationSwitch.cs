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
		static string m_keyPrefName = "SimulationMode";

		static bool Enabled
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

		static SimulationSwitch()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}
		
		static void OnToolbarGUI()
		{
			GUILayout.FlexibleSpace();

			Enabled = GUILayout.Toggle(Enabled, new GUIContent("S", "Simulation Mode"), ToolbarStyles.commandButtonStyle);
		}
	}
}