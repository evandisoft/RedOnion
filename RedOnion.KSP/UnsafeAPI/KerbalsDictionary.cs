using System;
using System.ComponentModel;
using RedOnion.KSP.Utilities;

namespace RedOnion.KSP.UnsafeAPI
{
	/// <summary>
	/// Not sure if I want to add this.
	/// </summary>
	[Description("A dictionary of all the kerbals who are in the current crew.")]
	public class KerbalsDictionary : ScriptStringKeyedConstDictionary<ProtoCrewMember>
	{
		public KerbalsDictionary()
		{
		}

		public static KerbalsDictionary Instance
		{
			get
			{
				var kerbals = new KerbalsDictionary();
				foreach(var member in HighLogic.fetch.currentGame.CrewRoster.Crew)
				{
					if (member.rosterStatus==ProtoCrewMember.RosterStatus.Assigned || member.rosterStatus==ProtoCrewMember.RosterStatus.Available)
					{
						kerbals.Add(member.name.Replace(' ', '_').Replace('-', '_'), member);
					}
				}
				return kerbals;
			}
		}
	}
}
