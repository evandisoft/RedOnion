using System;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	public class Sensor : PartBase
	{
		protected readonly ModuleEnviroSensor module;
		protected internal Sensor(Ship ship, Part part, PartBase parent, Decoupler decoupler, ModuleEnviroSensor module)
			: base(ship, part, parent, decoupler)
			=> this.module = module;

		public bool Active => module.sensorActive;
		public void Toggle() => module.Toggle();
		public ModuleEnviroSensor.SensorType Type => module.sensorType;
		public string Display => module.readoutInfo;
		public double Consumption => module.resHandler?.GetAverageInput() ?? 0.0;		
	}
}
