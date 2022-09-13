using Microsoft.Xna.Framework.Input;
using Nez.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neztris.Shared
{
	internal sealed class InputActions
	{
		[JsonInclude]
		public InputAction MoveBlockDown { get; set; } = new InputAction("Move Block Down", Keys.Down);
		[JsonInclude]
		public InputAction MoveBlockLeft { get; set; } = new InputAction("Move Block Left", Keys.Left);
		[JsonInclude]
		public InputAction MoveBlockRight { get; set; } = new InputAction("Move Block Right", Keys.Right);
		[JsonInclude]
		public InputAction RotateBlockCounterClockwise { get; set; } = new InputAction("Rotate Block CCW", Keys.Q);
		[JsonInclude]
		public InputAction RotateBlockClockwise { get; set; } = new InputAction("Rotate Block CW", Keys.E);

		public InputAction[] GetAvailableActions() 
		{
			return new []
			{
				MoveBlockDown,
				MoveBlockLeft,
				MoveBlockRight,
				RotateBlockCounterClockwise,
				RotateBlockClockwise
			};
		}

	}
}
