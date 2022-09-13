using Microsoft.Xna.Framework;
using Nez;
using Nez.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neztris.Scenes.MainMenu
{
	internal sealed class MainMenuScene: Scene
	{
		public override void Initialize()
		{
			base.Initialize();

			ClearColor = Color.Black;
			AddRenderer(new DefaultRenderer());

			m_mainMenu = CreateEntity("MainMenu");
			m_mainMenu.AddMenus(
				() => Debug.Log("Starting Game..."), 
				() => Core.Exit()
			);
		}

		private Entity m_mainMenu;
	}
}
