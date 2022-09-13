using Nez.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neztris.Utils
{
	internal static class ControlHelpers
	{
		public static Table Pad(this Table table, float left, float top, float right, float bottom)
		{
			return table.PadLeft(left)
				.PadTop(top)
				.PadRight(right)
				.PadBottom(bottom);
		}

		public static Table Pad(this Table table, float all)
		{
			return table.PadLeft(all)
				.PadTop(all)
				.PadRight(all)
				.PadBottom(all);
		}
	}
}
