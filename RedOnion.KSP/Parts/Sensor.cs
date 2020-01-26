using System;
using System.ComponentModel;
using RedOnion.KSP.API;

namespace RedOnion.KSP.Parts
{
	[Description("Part that is also a sensor. (Has `ModuleEnviroSensor`.)")]
	public class Sensor : PartBase
	{
		protected readonly ModuleEnviroSensor module;
		protected internal Sensor(Ship ship, Part part, PartBase parent, DecouplerBase decoupler, ModuleEnviroSensor module)
			: base(PartType.Sensor, ship, part, parent, decoupler)
			=> this.module = module;

		[Description("Accepts `sensor`. (Case insensitive)")]
		public override bool istype(string name)
			=> name.Equals("sensor", StringComparison.OrdinalIgnoreCase);

		[Description("State of the sensor.")]
		public bool active => module.sensorActive;
		[Description("Toggle the state.")]
		public void toggle() => module.Toggle();
		[Description("Sensor type.")]
		public ModuleEnviroSensor.SensorType Type => module.sensorType;
		[Description("Sensor read-out.")]
		public string display => module.readoutInfo;
		[Description("Sensor electric consumption.")]
		public double consumption => module.resHandler?.GetAverageInput() ?? 0.0;		
	}
}
