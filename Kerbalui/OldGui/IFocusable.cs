namespace Kerbalui.Obsolete {
    public interface IFocusable {
		string ControlName { get; }
		bool HasFocus();
		void GrabFocus();
	}
}