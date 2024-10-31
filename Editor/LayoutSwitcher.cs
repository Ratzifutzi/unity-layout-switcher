using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEditorInternal;

namespace UnityEditor
{
	[InitializeOnLoad]
	public class LayoutSwitcher
	{
		private const string PlayModeLayoutName = "PlayModeLayout";
		private const string EditModeLayoutName = "EditModeLayout";
		private static string layoutsPath;

		// Make this static so it can be accessed from the static context
		private static bool changeLayout = true;

		static LayoutSwitcher()
		{
			layoutsPath = Path.Combine(InternalEditorUtility.unityPreferencesFolder, "Layouts");
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			changeLayout = EditorPrefs.GetBool("LayoutSwitcher_ChangeLayout", true);
		}

		[MenuItem("Tools/Layout Switcher/Toggle Auto-Switch")]
		private static void ToggleAutoSwitch()
		{
			changeLayout = !changeLayout;
			EditorPrefs.SetBool("LayoutSwitcher_ChangeLayout", changeLayout);
			Debug.Log($"Layout auto-switch is now {(changeLayout ? "enabled" : "disabled")}");
		}

		private static void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			EditorApplication.delayCall += () =>
			{
				switch (state)
				{
					case PlayModeStateChange.EnteredPlayMode:
						if (changeLayout)
							LoadLayout(PlayModeLayoutName);
						break;
					case PlayModeStateChange.EnteredEditMode:
						if (changeLayout)
							LoadLayout(EditModeLayoutName);
						break;
				}
			};
		}

		private static void LoadLayout(string layoutName)
		{
			if (string.IsNullOrEmpty(layoutsPath) || string.IsNullOrEmpty(layoutName))
			{
				Debug.LogWarning("LayoutSwitcher: Invalid path or layout name.");
				return;
			}

			string layoutPath = Path.Combine(layoutsPath, layoutName + ".wlt");
			if (File.Exists(layoutPath))
			{
				try
				{
					EditorUtility.LoadWindowLayout(layoutPath);
				}
				catch (System.Exception e)
				{
					Debug.LogError($"LayoutSwitcher: Error loading layout: {e.Message}");
				}
			}
			else
			{
				Debug.LogWarning($"LayoutSwitcher: Layout '{layoutName}' not found at {layoutPath}. Create it in Unity and save it with this name.");
			}
		}
	}
}
