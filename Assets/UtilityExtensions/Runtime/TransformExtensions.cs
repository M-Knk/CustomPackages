using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomPackage.UtililtyExtensions
{
	public static class TransformExtensions
	{
		public static void Initiailze(this Transform transform)
		{
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
		}
	}
}