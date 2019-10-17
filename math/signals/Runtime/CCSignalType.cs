using System;
using System.Collections.Generic;

namespace cc.creativecomputing.math.signal
{
	[Serializable]
	public sealed class CCSignalType
	{
		public static readonly CCSignalType SIMPLEX = new CCSignalType("SIMPLEX", InnerEnum.SIMPLEX, new CCSimplexNoise());
		public static readonly CCSignalType LINEAR_RANDOM = new CCSignalType("LINEAR_RANDOM", InnerEnum.LINEAR_RANDOM, new CCLinearRandom());
		public static readonly CCSignalType SINUS = new CCSignalType("SINUS", InnerEnum.SINUS, new CCSinSignal());
		public static readonly CCSignalType SQUARE = new CCSignalType("SQUARE", InnerEnum.SQUARE, new CCSquareSignal());
		public static readonly CCSignalType TRI = new CCSignalType("TRI", InnerEnum.TRI, new CCTriSignal());
		public static readonly CCSignalType SLOPED_TRI = new CCSignalType("SLOPED_TRI", InnerEnum.SLOPED_TRI, new CCSlopedTriSignal());
		public static readonly CCSignalType SAW = new CCSignalType("SAW", InnerEnum.SAW, new CCSawSignal());

		private static readonly IList<CCSignalType> valueList = new List<CCSignalType>();

		static CCSignalType()
		{
			valueList.Add(SIMPLEX);
			valueList.Add(LINEAR_RANDOM);
			valueList.Add(SINUS);
			valueList.Add(SQUARE);
			valueList.Add(TRI);
			valueList.Add(SLOPED_TRI);
			valueList.Add(SAW);
		}

		public enum InnerEnum
		{
			SIMPLEX,
			LINEAR_RANDOM,
			WORLEY,
			SINUS,
			SQUARE,
			TRI,
			SLOPED_TRI,
			SAW
		}

		public readonly InnerEnum innerEnumValue;
		private readonly string nameValue;
		private readonly int ordinalValue;
		private static int nextOrdinal = 0;

		private CCSignal _mySignal;

		private CCSignalType(string name, InnerEnum innerEnum, CCSignal theSignal)
		{
			_mySignal = theSignal;

			nameValue = name;
			ordinalValue = nextOrdinal++;
			innerEnumValue = innerEnum;
		}

		public CCSignal signal()
		{
			return _mySignal;
		}


		public static IList<CCSignalType> values()
		{
			return valueList;
		}

		public int ordinal()
		{
			return ordinalValue;
		}

		public override string ToString()
		{
			return nameValue;
		}

		public static CCSignalType valueOf(string name)
		{
			foreach (CCSignalType enumInstance in CCSignalType.valueList)
			{
				if (enumInstance.nameValue == name)
				{
					return enumInstance;
				}
			}
			throw new System.ArgumentException(name);
		}
	}

}