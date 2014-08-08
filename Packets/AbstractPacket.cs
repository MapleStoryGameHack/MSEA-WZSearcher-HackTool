﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MSEAHackUtility
{
	public abstract class AbstractPacket
	{
		protected MemoryStream _buffer;

		public byte[] ToArray()
		{
			return _buffer.ToArray();
		}
	}
}