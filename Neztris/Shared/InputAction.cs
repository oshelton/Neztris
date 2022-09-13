using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neztris.Shared
{
	internal sealed class InputAction
	{
		private InputAction() { }

		public InputAction(string title, Keys defaultKey)
		{
#if DEBUG
			if (string.IsNullOrWhiteSpace(title))
				throw new ArgumentNullException("Title cannot be null.");
#endif
			Title = title;
			DefaultKey = defaultKey;
		}

		[JsonInclude]
		public string Title { get; private set; }
		[JsonInclude]
		public Keys DefaultKey { get; private set; }
		[JsonInclude]
		public Keys? OverrideKey { get; set; }
		public Keys ActualKey {
			get => OverrideKey ?? DefaultKey;
			set => OverrideKey = value; 
		}

		public bool IsActionPressed() => Input.IsKeyPressed(ActualKey);
		public bool IsActionDown() => Input.IsKeyDown(ActualKey);
		public bool IsActionReleased() => Input.IsKeyReleased(ActualKey);
	}
}
