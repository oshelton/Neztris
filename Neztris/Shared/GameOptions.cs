using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nez.Persistence;

namespace Neztris.Shared
{
	internal sealed class GameOptions
	{
		public static GameOptions Load()
		{
			if (Instance is not null)
				throw new InvalidOperationException("Options have already been loaded.");

			FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Game.Name, "options.json");

			if (!File.Exists(FilePath))
			{
				Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Game.Name));
				Instance = new GameOptions()
				{
					MusicVolume = 0.5f,
					SoundVolume = 0.5f,
					InputActions = new InputActions(),
				};
			}
			else
			{
				Instance = Json.FromJson<GameOptions>(File.ReadAllText(FilePath));
			}
			return Instance;
		}

		public static GameOptions Instance { get; private set; }

		internal GameOptions() { }

		[JsonInclude]
		public float MusicVolume { get; set; }
		[JsonInclude]
		public float SoundVolume { get; set; }
		[JsonInclude]
		public InputActions InputActions { get; set; }

		public Task Save()
		{
			var json = Json.ToJson(this);

			return Task.Run(() =>
			{
				File.WriteAllText(FilePath, json);
			});
		}

		private static string FilePath { get; set; }
	}
}
