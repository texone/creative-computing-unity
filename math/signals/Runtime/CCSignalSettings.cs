using System;

namespace cc.creativecomputing.math.signal
{
	using UnityEngine;

	[Serializable]
	public class CCSignalSettings
	{

		[Range(0,10)]
		public float scale = 1;
		[Range(1, 10)]
		public float octaves = 1;
		[Range(0, 1)]
		public float gain = 0.5f;
		[Range(0, 10)]
		public float lacunarity = 2;
	
		public bool normed = true;

		

		
	}
}