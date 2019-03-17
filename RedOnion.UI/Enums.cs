using System;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	public enum Layout
	{
		Stack,
		Horizontal,
		Vertical,
		FlowHorizontal,
		FlowVertical
	}
	public enum ImageType
	{
		Simple	= UUI.Image.Type.Simple,
		Sliced	= UUI.Image.Type.Sliced,
		Tiled	= UUI.Image.Type.Tiled,
		Filled	= UUI.Image.Type.Filled
	}
}
