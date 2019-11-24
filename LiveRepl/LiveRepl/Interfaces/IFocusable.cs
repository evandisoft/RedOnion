namespace LiveRepl.Interfaces 
{
    public interface IFocusable 
    {
		string ControlName { get; }
		bool HasFocus();
		void GrabFocus();
	}
}