using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

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
		static bool m_enabled;
		static bool m_visible;
		
		static MTK_Manager m_mtkManager;

		public static bool Enabled
		{
			get { return m_enabled; }
			set
			{
				m_enabled = value;
				m_mtkManager.SwitchSetup();
			}
		}

		static SimulationSwitch()
		{
			EditorSceneManager.sceneOpened += OnSceneOpened;
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
		}

		static void OnSceneOpened(Scene scene, OpenSceneMode mode)
		{
			m_mtkManager = GameObject.FindObjectOfType<MTK_Manager>();
			m_visible = m_mtkManager != null;

			if(m_visible)
				m_enabled = m_mtkManager.activeSetup.GetType() == typeof(MTK_SetupSimulator);
		}
		
		static void OnToolbarGUI()
		{
			if(m_visible)
			{
				GUI.changed = false;

				GUILayout.FlexibleSpace();

				GUILayout.Toggle(m_enabled, new GUIContent("S", "Simulation Mode"), ToolbarStyles.commandButtonStyle);

				if (GUI.changed)
				{
					Enabled = !Enabled;
				}
			}
		}
	}
}