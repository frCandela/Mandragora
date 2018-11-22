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
				return m_mtkManager.activeSetup.GetType() == typeof(MTK_SetupSimulator);
			}
			set
			{
				m_mtkManager.SwitchSetup();
			}
		}

		static bool Visible
		{
			get
			{
				return m_mtkManager != null;
			}
		}

		static SimulationSwitch()
		{
			EditorApplication.hierarchyChanged += UpdateDisplay;
			
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void UpdateDisplay()
		{
			m_mtkManager = GameObject.FindObjectOfType<MTK_Manager>();
		}
		
		static void OnToolbarGUI()
		{
			if(Visible)
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
}