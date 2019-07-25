using RedOnion.ROS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace RedOnion.KSP.API
{
	public partial class VectorCreator
	{
		public class Draw
		{
			protected IProcessor processor;
			protected ISpaceObject _reference;
			protected Vector3d _origin, _direction;

			protected GameObject bodyObject;
			protected GameObject headObject;
			protected LineRenderer bodyRender;
			protected LineRenderer headRender;
		}
	}
}
