using System;

namespace receiverApp
{
	public class SData
	{
		public string sensor_id { get; set; }
		public DateTime send_time { get; set; }
		public double sensor_data { get; set; }

		public SData(string sensor_id, DateTime send_time, double sensor_data)
		{
			this.sensor_id = sensor_id;
			this.send_time = send_time;
			this.sensor_data = sensor_data;
		}

		public override String ToString()
		{
			return String.Format(" {0} | {1} | {2} ", sensor_id, send_time, sensor_data);
		}
	}
}

