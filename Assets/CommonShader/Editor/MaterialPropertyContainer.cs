using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace CustomPackage.CommonShader
{
	public class MaterialPropertyContainer
	{
		readonly List<MaterialProperty> restProperties;
		MaterialProperty[] restPropertyArray;
		bool isUpdatedRestProperties = true;

		readonly List<System.Action<MaterialEditor, List<Material>>> drawActions = new List<System.Action<MaterialEditor, List<Material>>>();
		readonly List<System.Action<MaterialPropertyContainer>> subContainers = new List<System.Action<MaterialPropertyContainer>>();

		public MaterialProperty[] RestProperties
		{
			get
			{
				if (isUpdatedRestProperties)
				{
					restPropertyArray = restProperties.ToArray();
					isUpdatedRestProperties = false;
				}
				return restPropertyArray;
			}
		}

		public MaterialPropertyContainer(MaterialProperty[] properties)
		{
			restProperties = new List<MaterialProperty>(properties);
		}

		private MaterialPropertyContainer(MaterialPropertyContainer parent)
		{
			this.restProperties = parent.restProperties;
		}

		bool RemoveUsedProperty(string name)
		{
			isUpdatedRestProperties = true;
			return restProperties.RemoveAll(prop => prop.name == name) > 0;
		}

		public void Draw(MaterialEditor materialEditor)
		{
			var materials = materialEditor.targets.Cast<Material>().ToList();
			foreach (var action in drawActions)
			{
				action.Invoke(materialEditor, materials);
			}
		}

		public void FoldoutSub(string label, System.Action<MaterialPropertyContainer> foldoutAction)
		{
			int subContainerIndex = subContainers.Count;
			bool foldout = false;
			MaterialPropertyContainer subContainer = new MaterialPropertyContainer(this);

			var style = new GUIStyle(EditorStyles.foldout);
			style.fontStyle = FontStyle.Bold;

			drawActions.Add((materialEditor, targets) =>
			{
				if (foldout = EditorGUILayout.Foldout(foldout, label, style))
				{
					using (new EditorGUI.IndentLevelScope())
					{
						subContainer.Draw(materialEditor);
					}
				}
			});

			foldoutAction.Invoke(subContainer);
		}

		public void IntSlider(string propertyName, int min, int max)
		{
			IntSlider(ObjectNames.NicifyVariableName(propertyName), propertyName, min, max);
		}
		public void IntSlider(string label, string propertyName, int leftValue, int rightValue)
		{
			drawActions.Add((materialEditor, targets) =>
			{
				int value = targets[0].GetInt(propertyName);
				using (var changeCheck = new EditorGUI.ChangeCheckScope())
				{
					value = EditorGUILayout.IntSlider(label, value, leftValue, rightValue);
					if (changeCheck.changed)
					{
						foreach (Material t in targets)
						{
							t.SetInt(propertyName, value);
						}
					}
				}
			});

			RemoveUsedProperty(propertyName);
		}

		public void EnumPopup<T>(string propertyName, int min, int max) where T : struct
		{
			EnumPopup<T>(ObjectNames.NicifyVariableName(propertyName), propertyName);
		}
		public void EnumPopup<T>(string label, string propertyName) where T : struct
		{
			drawActions.Add((materialEditor, targets) =>
			{
				int value = targets[0].GetInt(propertyName);
				using (var changeCheck = new EditorGUI.ChangeCheckScope())
				{
					System.Enum valueEnum = (System.Enum)System.Enum.ToObject(typeof(T), value);
					valueEnum = EditorGUILayout.EnumPopup(label, valueEnum);
					if (changeCheck.changed)
					{
						value = System.Convert.ToInt32(valueEnum);
						foreach (Material t in targets)
						{
							t.SetInt(propertyName, value);
						}
					}
				}
			});

			RemoveUsedProperty(propertyName);
		}
	}
}