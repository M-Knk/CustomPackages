using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CustomPackage.PackageUtils
{
	internal class InitializerWindow : EditorWindow
	{
		Initializer.PackageData packageData;
		bool isOverwrite = false;

		internal static InitializerWindow Show(Initializer.PackageData packageData)
		{
			GetWindow<InitializerWindow>().Close();

			var window = CreateInstance<InitializerWindow>();
			window.titleContent.text = "Package Initializer";
			window.packageData = packageData;
			window.ShowUtility();
			return window;
		}

		private void OnGUI()
		{
			packageData.name = EditorGUILayout.TextField("Name", packageData.name);
			packageData.displayName = EditorGUILayout.TextField("Display Name", packageData.displayName);
			packageData.version = EditorGUILayout.TextField("Version", packageData.version);

			//EditorGUILayout.Toggle("Minimal Unity Version", true);
			{
				packageData.minUnityVersionMajor = EditorGUILayout.IntField("Major", packageData.minUnityVersionMajor);
				packageData.minUnityVersionMinor = EditorGUILayout.IntField("Minor", packageData.minUnityVersionMinor);
				//EditorGUILayout.TextField("Release", "");
			}

			EditorGUILayout.LabelField("Brief Description");
			packageData.description = EditorGUILayout.TextArea(packageData.description, GUILayout.Height(100f));

			packageData.asmdefPrefix = EditorGUILayout.TextField("Assemby Define Prefix", packageData.asmdefPrefix);

			bool canCreate = true;
			if (packageData.isExistsPackageJson)
			{
				isOverwrite = EditorGUILayout.Toggle("Overwrite?", isOverwrite);
				canCreate = isOverwrite;
			}

			using (new EditorGUI.DisabledGroupScope(!canCreate))
			{
				if (GUILayout.Button("Create"))
				{
					Close();

					Initializer.Initialize(packageData);
				}
			}
		}
	}
}
