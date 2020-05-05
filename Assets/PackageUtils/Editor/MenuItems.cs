using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.IO;

namespace CustomPackage.PackageUtils
{
	static class MenuItems
	{
		const string itemNameInitialize = "Assets/Package Utils/Initialize";
		[MenuItem(itemNameInitialize, true)]
		static bool ValidateInitialize()
		{
			Object target = Selection.activeObject;
			if (target == null)
			{
				return false;
			}

			string targetPath = AssetDatabase.GetAssetPath(target);
			return Directory.Exists(targetPath);
		}
		[MenuItem(itemNameInitialize)]
		static void Initialize()
		{
			var packageData = new Initializer.PackageData(AssetDatabase.GetAssetPath(Selection.activeObject));
			InitializerWindow.Show(packageData);
		}
	}
}