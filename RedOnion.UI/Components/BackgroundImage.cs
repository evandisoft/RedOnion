using UUI = UnityEngine.UI;

namespace RedOnion.UI.Components
{
	public class BackgroundImage : UUI.Image
	{
		public override float preferredWidth => -1f;
		public override float preferredHeight => -1f;
	}
}
