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

		static bool Enabled
		{
			get
			{
				if(!m_mtkManager)
					return false;

				return m_mtkManager.activeSetup.GetType() == typeof(MTK_SetupSimulator);
			}
			set
			{
				if(m_mtkManager)
					m_mtkManager.SwitchSetup();
			}
		}

		static SimulationSwitch()
		{
			EditorApplication.update += UpdateDisplay;
			
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void UpdateDisplay()
		{
			if(!m_mtkManager)
				m_mtkManager = GameObject.FindObjectOfType<MTK_Manager>();
		}
		
		static void OnToolbarGUI()
		{
			GUI.changed = false;

			GUILayout.FlexibleSpace();

			GUILayout.Toggle(Enabled, new GUIContent("S", "Simulation Mode"), ToolbarStyles.commandButtonStyle);

			if (GUI.changed)
			{
				Enabled = !Enabled;
			}
		}
	}
}