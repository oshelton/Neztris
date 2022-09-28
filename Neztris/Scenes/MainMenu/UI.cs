using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Nez;
using Nez.UI;
using Neztris.Shared;
using Neztris.Utils;
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Neztris.Scenes.MainMenu
{
	internal static class UI
	{
		public static UICanvas AddMenus(this Entity entity, Action onPlay, Action onExit)
		{
			var canvas = new UICanvas();
			canvas.IsFullScreen = false;
			canvas.RenderLayer = c_screenSpaceRenderLayer;
			entity.AddComponent(canvas);

			var uiState = new UIState();
			uiState.MainMenu = CreateMainMenu(canvas, uiState, onPlay, onExit);
			uiState.OptionsMenu = CreateOptionsMenu(canvas, uiState);

			entity.AddComponent(uiState);

			return canvas;
		}

		private static Table CreateMainMenu(UICanvas canvas, UIState state, Action onPlay, Action onExit)
		{
			var table = new Table();
			table.Pad(16);
			table.SetFillParent(true);
			table.Center();
			canvas.Stage.AddElement(table);

			table.Row().SetFillX().Pad(0.0f, 0.0f, 32.0f, 0.0f);

			var titleLabel = new Label("Neztris");
			titleLabel.SetFontScale(3.0f);
			titleLabel.SetFontColor(Color.CadetBlue);
			table.Add(titleLabel);

			TextButton CreateAndAddMainMenuButton(string title, string tooltipText, Action<Button> onClickHandler)
			{
				table.Row().SetFillX().Pad(0.0f, 16.0f, 8.0f, 16.0f);

				var button = new TextButton(title, s_defaultSkin);
				button.OnClicked += onClickHandler;
				table.Add(button);

				var tooltip = new TextTooltip(tooltipText, button, s_defaultSkin);
				table.AddElement(tooltip);

				return button;
			}

			var playButton = CreateAndAddMainMenuButton("Play", "Start the game", button => onPlay());
			var highScoresButton = CreateAndAddMainMenuButton("High Scores", "View the recent high scores for the game", button => Console.WriteLine("High Scores!"));
			var optionsButton = CreateAndAddMainMenuButton("Options", "Open the options menu", button => state.CurentMenuState = ExclusiveMenuState.Options);
			var exitButton = CreateAndAddMainMenuButton("Exit", "Exit the game", button => onExit());

			playButton.EnableExplicitFocusableControl(exitButton, highScoresButton, null, null);
			highScoresButton.EnableExplicitFocusableControl(playButton, optionsButton, null, null);
			optionsButton.EnableExplicitFocusableControl(highScoresButton, exitButton, null, null);
			exitButton.EnableExplicitFocusableControl(optionsButton, playButton, null, null);

			canvas.Stage.SetGamepadFocusElement(playButton);
			state.PlayButton = playButton;
			state.OptionsButton = optionsButton;

			return table;
		}

		private static Dialog CreateOptionsMenu(UICanvas canvas, UIState state)
		{
			var dialog = new Dialog("Options", s_defaultSkin);
			dialog.Pad(16);
			dialog.SetMovable(false);
			dialog.SetIsVisible(false);
			dialog.SetWidth(250 + 32);
			canvas.Stage.AddElement(dialog);

			var scrollingContent = new Table();
			scrollingContent.Defaults().SetSpaceBottom(8.0f);
			scrollingContent.Defaults().SetFillX();
			scrollingContent.Top();

			void AddSectionLabel(Table container, string caption)
			{
				var label = new Label(caption);
				label.SetAlignment(Align.Center);
				container.Add(label).SetColspan(3).SetPadTop(8).SetPadBottom(8);

				container.Row();
			}

			Slider AddSliderOption(Table container, string label, float initialValue, Action<float> updateAction)
			{
				container.Add(label).SetAlign(Align.Left);

				container.Add().SetExpandX();

				var slider = new Slider(s_defaultSkin, null, 0, 1, 0.1f);
				slider.SetValue(initialValue);
				slider.OnChanged += updateAction;
				container.Add(slider).SetAlign(Align.Right).SetPadRight(8);

				container.Row();

				return slider;
			}

			void AddInputActionOption(Table container, UIState state, InputAction action)
			{
				container.Add(action.Title).SetAlign(Align.Left);

				container.Add().SetExpandX();

				var keyContainer = new Table();
				keyContainer.Right();
				keyContainer.Defaults().SetSpaceRight(4);

				var keyLabel = new Label(action.ActualKey.ToString());
				keyLabel.SetAlignment(Align.Right);
				keyContainer.Add(keyLabel);

				var editButton = new TextButton("Edit", s_defaultSkin);
				editButton.OnClicked += button =>
				{
					state.OptionsMenu.SetIsVisible(false);

					var reassignDialog = new Dialog("Reassign Action", s_defaultSkin);
					reassignDialog.SetWidth(250);

					var captionLabel = new Label($"Press any key to use for \"{action.Title}\".\nOr press Escape to cancel.");
					reassignDialog.GetContentTable().Add(captionLabel).Pad(8);

					reassignDialog.SetPosition(
						Game.ViewportWidth / 2 - reassignDialog.width / 2,
						Game.ViewportHeight / 2 - reassignDialog.height / 2
					);
					container.GetStage().AddElement(reassignDialog);

					IEnumerator RebindControl(InputAction action, Label keyLabel, Dialog reassignDialog, UIState state)
					{
						while (true)
						{
							var pressedKeys = Input.CurrentKeyboardState.GetPressedKeys();
							if (pressedKeys.Length != 0)
							{
								if (!pressedKeys.Contains(Keys.Escape))
								{
									action.OverrideKey = pressedKeys[0];
									keyLabel.SetText(action.ActualKey.ToString());
								}
								reassignDialog.Remove();
								state.OptionsMenu.SetIsVisible(true);
								break;
							}
							else
							{
								yield return null;
							}
						}
					}

					Core.StartCoroutine(RebindControl(action, keyLabel, reassignDialog, state));
				};
				keyContainer.Add(editButton);

				var resetButton = new TextButton("Reset", s_defaultSkin);
				resetButton.OnClicked += button =>
				{
					action.OverrideKey = null;
					keyLabel.SetText(action.ActualKey.ToString());
				};
				keyContainer.Add(resetButton);

				container.Add(keyContainer).SetFillX().SetAlign(Align.Right);

				container.Row();
			}

			AddSectionLabel(scrollingContent, "Audio");
			var musicSlider = AddSliderOption(scrollingContent, "Music", GameOptions.Instance.MusicVolume, val => state.MusicVolume = val);
			var soundSlider = AddSliderOption(scrollingContent, "Sound Effects", GameOptions.Instance.SoundVolume, val => state.SoundVolume = val);
			AddSectionLabel(scrollingContent, "Controls");
			foreach (var action in GameOptions.Instance.InputActions.GetAvailableActions())
			{
				AddInputActionOption(scrollingContent, state, action);
			}

			var scrollContainer = new ScrollPane(scrollingContent, s_defaultSkin);
			scrollContainer.FillParent = true;
			dialog.GetContentTable().Add(scrollContainer);

			var saveButton = new TextButton("Save", s_defaultSkin);
			saveButton.OnClicked += button =>
			{
				state.UpdateOptions();
				state.CurentMenuState = ExclusiveMenuState.Main;
			};
			dialog.GetButtonTable().Add(saveButton);

			var backButton = new TextButton("Back", s_defaultSkin);
			backButton.OnClicked += button => state.CurentMenuState = ExclusiveMenuState.Main;
			dialog.GetButtonTable().Add(backButton);

			musicSlider.EnableExplicitFocusableControl(backButton, soundSlider, null, null);
			soundSlider.EnableExplicitFocusableControl(musicSlider, saveButton, null, null);
			saveButton.EnableExplicitFocusableControl(soundSlider, musicSlider, null, backButton);
			backButton.EnableExplicitFocusableControl(soundSlider, musicSlider, saveButton, null);

			dialog.SetPosition(
				Game.ViewportWidth / 2 - dialog.width / 2,
				Game.ViewportHeight / 2 - dialog.height / 2
			);
			state.MusicSlider = musicSlider;
			state.SoundSlider = soundSlider;

			return dialog;
		}

		private sealed class UIState: Component
		{
			public UIState()
			{
				var options = GameOptions.Instance;
				MusicVolume = options.MusicVolume;
				SoundVolume = options.SoundVolume;
			}

			public Table MainMenu { get; set; }
			public Dialog OptionsMenu { get; set; }

			public TextButton PlayButton { private get; set; }
			public TextButton OptionsButton { get; set; }

			public Slider MusicSlider { get; set; }
			public Slider SoundSlider { get; set; }

			public ExclusiveMenuState CurentMenuState
			{
				get => m_currentMenuState;
				set
				{
					if (value != m_currentMenuState)
					{
						var stage = MainMenu.GetStage();
						MainMenu.SetIsVisible(false);
						OptionsMenu.SetIsVisible(false);

						TooltipManager.GetInstance().HideAll();

						m_currentMenuState = value;
						switch (m_currentMenuState)
						{
							case ExclusiveMenuState.Main:
								MainMenu.SetIsVisible(true);
								stage.SetGamepadFocusElement(PlayButton);
								break;
							case ExclusiveMenuState.Options:
								OptionsMenu.SetIsVisible(true);
								stage.SetGamepadFocusElement(MusicSlider);
								break;
						};
					}
				}
			}

			public void UpdateOptions()
			{
				var options = GameOptions.Instance;
				options.MusicVolume = MusicVolume;
				options.SoundVolume = SoundVolume;
				options.Save();
			}

			public float MusicVolume { get; internal set; } = 0.5f;
			public float SoundVolume { get; internal set; } = 0.5f;

			private ExclusiveMenuState m_currentMenuState = ExclusiveMenuState.Main; 
		};

		private static readonly Skin s_defaultSkin = Skin.CreateDefaultSkin();

		private const int c_screenSpaceRenderLayer = 999;

		private enum ExclusiveMenuState
		{
			Main,
			Options
		}
	}
}
