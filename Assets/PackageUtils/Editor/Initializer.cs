using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Compilation;
using System.IO;

using ReplaceStr = System.Tuple<string, string>;

namespace CustomPackage.PackageUtils
{
	internal static class Initializer
	{
		[System.Serializable]
		public class PackageData
		{
			public readonly string directoryPath;

			public string displayName;
			public string name;
			public string version = "1.0.0";
			public int minUnityVersionMajor = 2019;
			public int minUnityVersionMinor = 3;
			//public string minUnityVersionRelease = "";
			public string description;

			public string asmdefPrefix;
			public readonly bool isExistsPackageJson;

			public ReplaceStr[] ReplaceStrs { get; private set; }

			public PackageData(string directoryPath)
			{
				this.directoryPath = directoryPath;
				isExistsPackageJson = File.Exists(directoryPath + "\\package.json");

				displayName = Path.GetFileNameWithoutExtension(directoryPath);
				name = ToLowerKebab("com." + ToLowerKebab(Application.companyName) + "." + ToLowerKebab(displayName));
				description = "TODO: Write package's description.";

				asmdefPrefix = Application.companyName + "." + displayName;
			}

			public void Initialize()
			{
				ReplaceStrs = new ReplaceStr[]
				{
					new ReplaceStr("{DISPLAY_NAME}", displayName),
					new ReplaceStr("{NAME}", name),
					new ReplaceStr("{VERSION}", version),
					new ReplaceStr("{MIN_UNITY_VERSION}", string.Format("{0}.{1}", minUnityVersionMajor, minUnityVersionMinor)),
					new ReplaceStr("{DESCRIPTION}", description),

					new ReplaceStr("{ASMDEF_PREFIX}", asmdefPrefix),
				};
			}
		}

		const string packageJsonFormat = @"{
	""name"": ""{NAME}"",
	""displayName"": ""{DISPLAY_NAME}"",
	""version"": ""{VERSION}"",
	""unity"": ""{MIN_UNITY_VERSION}"",
	""description"": ""{DESCRIPTION}"",
	""dependencies"": {
	}
}
";
		const string formatFileDirectoryPath = ".\\JSONFormat\\";

		public static void Initialize(PackageData packageData)
		{
			packageData.Initialize();

			const float taskNum = 4;

			EditorUtility.DisplayProgressBar("Package Initializer", "Craete Package JSON", 0f / taskNum);
			if (!CreatePackageJson(packageData))
			{
				return;
			}

			EditorUtility.DisplayProgressBar("Package Initializer", "Craete Runtime Directory", 1f / taskNum);
			InitializeDirectory(packageData, "Runtime");

			EditorUtility.DisplayProgressBar("Package Initializer", "Craete Editor Directory", 2f / taskNum);
			InitializeDirectory(packageData, "Editor");

			EditorUtility.DisplayProgressBar("Package Initializer", "Craete Tests Directory", 3f / taskNum);
			InitializeDirectory(packageData, "Tests");

			EditorUtility.ClearProgressBar();
			EditorUtility.DisplayDialog("Package Initializer", "Complete", "Ok");
		}

		static bool CreatePackageJson(PackageData packageData)
		{
			try
			{
				string text = ReplaceString(packageJsonFormat, packageData.ReplaceStrs);

				string outputPath = packageData.directoryPath + "\\package.json";
				File.WriteAllText(outputPath, text);
				AssetDatabase.ImportAsset(outputPath);
				return true;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				return false;
			}
		}

		static bool InitializeDirectory(PackageData packageData, string newDirectoryName)
		{
			string subDirectoryPath = TryCreateDirectory(packageData.directoryPath, newDirectoryName);
			if (string.IsNullOrEmpty(subDirectoryPath))
			{
				return false;
			}

			string format = Resources.Load<TextAsset>("JSONFormat/" + newDirectoryName).text;
			CreateAssemblyDefine(subDirectoryPath + @"\\" + packageData.asmdefPrefix + "." + newDirectoryName + ".asmdef", format, packageData.ReplaceStrs);
			return true;
		}

		static string TryCreateDirectory(string directoryPath, string newDirectoryName)
		{
			try
			{
				string newDirectoryPath = directoryPath + @"\\" + newDirectoryName;
				if (!Directory.Exists(newDirectoryPath))
				{
					Directory.CreateDirectory(newDirectoryPath);
				}
				AssetDatabase.ImportAsset(newDirectoryPath);
				return newDirectoryPath;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				return null;
			}
		}

		static bool CreateAssemblyDefine(string dstPath, string format, IEnumerable<ReplaceStr> replaceStrs)
		{
			try
			{
				string text = ReplaceString(format, replaceStrs);
				File.WriteAllText(dstPath, text);
				AssetDatabase.ImportAsset(dstPath);
				return true;
			}
			catch (System.Exception e)
			{
				Debug.LogError(e);
				return false;
			}
		}

		static string ReplaceString(string src, IEnumerable<ReplaceStr> values)
		{
			var sb = new System.Text.StringBuilder(src);
			foreach (var value in values)
			{
				sb.Replace(value.Item1, value.Item2);
			}
			return sb.ToString();
		}

		static string ToLowerKebab(string src)
		{
			string dst = src;
			int i = 1;
			while (i < dst.Length)
			{
				char c = dst[i];
				if (dst[i - 1] != '-' && 'A' <= c && c <= 'Z')
				{
					dst = dst.Insert(i, "-");
					i++;
				}
				i++;
			}
			return dst.ToLower();
		}
	}
}
