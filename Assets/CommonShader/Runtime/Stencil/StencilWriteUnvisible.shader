Shader "Custom/Stencil/Write Unvisible"
{
    Properties
    {
		_Color ("Color", Color) = (0, 0, 0, 0)
        _MainTex ("Texture", 2D) = "white" {}

		_StencilRef("Stencil Ref", Int) = 0
		_StencilReadMask("Stencil ReadMask", Int) = 255
		_StencilWriteMask("Stencil WriteMask", Int) = 255
		[Enum(UnityEngine.Rendering.CompareFunction)]_StencilComp ("Stencil Comp", Float) = 8
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilPass("Stencil Pass", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilFail("Stencil Fail", Float) = 0
		[Enum(UnityEngine.Rendering.StencilOp)]_StencilZFail("Stencil ZFail", Float) = 0
	}

	SubShader
	{
		Pass
		{
			Tags
			{
				"Queue" = "Geometry-1"
			}

			Blend Zero One
			ZWrite Off

			Stencil
			{
				Ref[_StencilRef]
				ReadMask[_StencilReadMask]
				WriteMask[_StencilWriteMask]
				Comp[_StencilComp]
				Pass[_StencilPass]
				Fail[_StencilFail]
				ZFail[_StencilZFail]
			}
		}
	}
}
