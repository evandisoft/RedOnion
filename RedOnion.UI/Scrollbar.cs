using RedOnion.Attributes;
using System;
using System.ComponentModel;
using UnityEngine;
using UUI = UnityEngine.UI;

namespace RedOnion.UI
{
	[WorkInProgress, Description("Scroll bar / slider.")]
	public class Scrollbar : Element
	{
		[Unsafe, Description("Game object of the sliding area.")]
		public GameObject SliderObject { get; }
		[Unsafe, Description("Game object of the handle.")]
		public GameObject HandleObject { get; }

		[Unsafe, Description("UnityEngine.UI.Image of the scrollbar.")]
		public UUI.Image MainImage { get; }
		[Unsafe, Description("UnityEngine.UI.Image of the handle.")]
		public UUI.Image HandleImage { get; }
		[Unsafe, Description("UnityEngine.UI.Scrollbar component.")]
		public UUI.Scrollbar Component { get; }

		[Description("Create horizontal (default) or vertical scroll bar.")]
		public Scrollbar(bool vertical = false)
			: this(vertical ? UUI.Scrollbar.Direction.TopToBottom : UUI.Scrollbar.Direction.LeftToRight)
		{ }
		public Scrollbar(UUI.Scrollbar.Direction direction)
		{
			SliderObject = new GameObject() { layer = UILayer };
			var sliderRect = SliderObject.AddComponent<RectTransform>();
			sliderRect.sizeDelta = new Vector2(-20f, -20f);
			sliderRect.anchorMin = Vector2.zero;
			sliderRect.anchorMax = Vector2.one;

			HandleObject = new GameObject() { layer = UILayer };
			var handleRect = HandleObject.AddComponent<RectTransform>();
			handleRect.sizeDelta = new Vector2(20f, 20f);

			SliderObject.transform.SetParent(RootObject.transform, false);
			HandleObject.transform.SetParent(SliderObject.transform, false);

			var img = RootObject.AddComponent<ScrollbarImage>();
			img.sprite = Skin.horizontalSlider.normal.background;
			img.type = UUI.Image.Type.Sliced;
			MainImage = img;

			HandleImage = HandleObject.AddComponent<Components.BackgroundImage>();
			HandleImage.sprite = Skin.horizontalSliderThumb.normal.background;
			HandleImage.type = UUI.Image.Type.Sliced;

			Component = RootObject.AddComponent<UUI.Scrollbar>();
			Component.handleRect = handleRect;
			Component.targetGraphic = HandleImage;
			Component.direction = direction;
			img.Scrollbar = Component;
		}

		class ScrollbarImage : UUI.Image
		{
			public UUI.Scrollbar Scrollbar;
			public bool vertical => Scrollbar != null
				&& (Scrollbar.direction == UUI.Scrollbar.Direction.BottomToTop
				|| Scrollbar.direction == UUI.Scrollbar.Direction.TopToBottom);
			public bool horizontal => Scrollbar != null
				&& (Scrollbar.direction == UUI.Scrollbar.Direction.LeftToRight
				|| Scrollbar.direction == UUI.Scrollbar.Direction.RightToLeft);
			public override float preferredWidth => vertical ? 80f : 16f;
			public override float preferredHeight => horizontal ? 80f : 16f;
		}
	}
}
