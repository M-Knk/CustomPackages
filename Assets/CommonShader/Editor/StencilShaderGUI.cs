using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace CustomPackage.CommonShader
{
	public class StencilShaderGUI : ShaderGUI
	{
		MaterialPropertyContainer propertyContainer = null;

		override public void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			if (propertyContainer == null)
			{
				propertyContainer = new MaterialPropertyContainer(properties);
				propertyContainer.FoldoutSub("Stencil", sub =>
				{
					sub.IntSlider("Ref", "_StencilRef", 0, 255);
					sub.IntSlider("Read Mask", "_StencilReadMask", 0, 255);
					sub.IntSlider("Write Mask", "_StencilWriteMask", 0, 255);
					sub.EnumPopup<UnityEngine.Rendering.CompareFunction>("Comp", "_StencilComp");
					sub.EnumPopup<UnityEngine.Rendering.StencilOp>("Pass", "_StencilPass");
					sub.EnumPopup<UnityEngine.Rendering.StencilOp>("Fail", "_StencilFail");
					sub.EnumPopup<UnityEngine.Rendering.StencilOp>("Z Fail", "_StencilZFail");
				});
			}

			base.OnGUI(materialEditor, propertyContainer.RestProperties);

			propertyContainer.Draw(materialEditor);
		}
	}
}