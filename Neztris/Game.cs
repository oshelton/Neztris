using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nez;
using Neztris.Shared;
using static Nez.Scene;

namespace Neztris
{
	internal sealed class Game : Core
	{
		public const string Name = "Neztris";

		public static int ViewportWidth = 352;
		public static int ViewportHeight = 320;

		public Game(): base(isFullScreen: false, windowTitle: Name, contentDirectory: @"..\..\Content") { }

		protected override void Initialize()
		{
			base.Initialize();

			GameOptions.Load();

			Core.ExitOnEscapeKeypress = false;

			Window.AllowUserResizing = true;
			SetDefaultDesignResolution(ViewportWidth, ViewportHeight, SceneResolutionPolicy.ShowAllPixelPerfect);
			Screen.SetSize(ViewportWidth * 2, ViewportHeight * 2);
			Scene = new Scenes.MainMenu.MainMenuScene();
		}
	}
}