using System.Collections.Generic;
using System.Linq;
using Quaver.Shared.Assets;
using Quaver.Shared.Config;
using Quaver.Shared.Screens.Options.Content;
using Quaver.Shared.Screens.Options.Items;
using Quaver.Shared.Screens.Options.Items.Custom;
using Quaver.Shared.Screens.Options.Sections;
using Quaver.Shared.Screens.Options.Sidebar;
using Wobble.Bindables;
using Wobble.Graphics;
using Wobble.Graphics.Sprites;

namespace Quaver.Shared.Screens.Options
{
    public class OptionsMenu : Sprite
    {
        /// <summary>
        /// </summary>
        private OptionsHeader Header { get; set; }

        /// <summary>
        /// </summary>
        private OptionsSidebar Sidebar { get; set; }

        /// <summary>
        /// </summary>
        private OptionsContent Content { get; set; }

        /// <summary>
        /// </summary>
        private List<OptionsSection> Sections { get; set; }

        /// <summary>
        /// </summary>
        public Bindable<OptionsSection> SelectedSection { get; private set; }

        /// <summary>
        /// </summary>
        private Dictionary<OptionsSection, OptionsContentContainer> ContentContainers { get; set; }

        /// <summary>
        /// </summary>
        public OptionsMenu()
        {
            Size = new ScalableVector2(1366, 768);
            Alpha = 0;

            CreateSections();
            CreateSidebar();
            CreateContainer();
            CreateContentContainers();
            CreateHeader();

            SetActiveContentContainer();
            SelectedSection.ValueChanged += OnSectionChanged;
        }

        private void OnSectionChanged(object sender, BindableValueChangedEventArgs<OptionsSection> e)
            => ScheduleUpdate(SetActiveContentContainer);

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public override void Destroy()
        {
            // ReSharper disable once DelegateSubtraction
            SelectedSection.ValueChanged -= OnSectionChanged;
            SelectedSection?.Dispose();

            base.Destroy();
        }

        /// <summary>
        /// </summary>
        private void CreateSections()
        {
            var contentWidth = Width - OptionsSidebar.WIDTH + 2;

            Sections = new List<OptionsSection>
            {
                new OptionsSection("Video", FontAwesome.Get(FontAwesomeIcon.fa_desktop_monitor), new List<OptionsSubcategory>
                {
                    new OptionsSubcategory("Window", new List<OptionsItem>()
                    {
                        new OptionsItemScreenResolution(contentWidth, "Screen Resolution"),
                        new OptionsItemCheckbox(contentWidth, "Enable Fullscreen", ConfigManager.WindowFullScreen),
                        new OptionsItemCheckbox(contentWidth, "Enable Borderless Window", ConfigManager.WindowBorderless)
                    }),
                    new OptionsSubcategory("Frame Time", new List<OptionsItem>()
                    {
                        new OptionsItemFrameLimiter(contentWidth, "Frame Limiter"),
                        new OptionsItemCheckbox(contentWidth, "Display FPS Counter", ConfigManager.FpsCounter)
                    })
                }),
                new OptionsSection("Audio", FontAwesome.Get(FontAwesomeIcon.fa_volume_up_interface_symbol), new List<OptionsSubcategory>
                {
                    new OptionsSubcategory("Volume", new List<OptionsItem>()
                    {
                        new OptionsSlider(contentWidth, "Master Volume", ConfigManager.VolumeGlobal),
                        new OptionsSlider(contentWidth, "Music Volume", ConfigManager.VolumeMusic),
                        new OptionsSlider(contentWidth, "Effect Volume", ConfigManager.VolumeEffect),
                    }),
                    new OptionsSubcategory("Offset", new List<OptionsItem>()
                    {
                        new OptionsItemSliderGlobalOffset(contentWidth, "Global Offset", ConfigManager.GlobalAudioOffset),
                        new OptionsItem(contentWidth, "Calibrate Offset")
                    }),
                    new OptionsSubcategory("Effects", new List<OptionsItem>()
                    {
                       new OptionsItemCheckbox(contentWidth, "Pitch Audio With Playback Rate", ConfigManager.Pitched)
                    }),
                    new OptionsSubcategory("Linux", new List<OptionsItem>()
                    {
                        new OptionsSlider(contentWidth, "Audio Device Period", ConfigManager.DevicePeriod),
                        new OptionsSlider(contentWidth, "Audio Device Buffer Length", ConfigManager.DeviceBufferLengthMultiplier)
                    })
                }),
                new OptionsSection("Gameplay", FontAwesome.Get(FontAwesomeIcon.fa_gamepad_console), new List<OptionsSubcategory>
                {
                    new OptionsSubcategory("Scrolling", new List<OptionsItem>()
                    {
                       new OptionsSlider(contentWidth, "Scroll Speed (4 Keys)", ConfigManager.ScrollSpeed4K),
                       new OptionsSlider(contentWidth, "Scroll Speed (7 Keys)", ConfigManager.ScrollSpeed7K)
                    }),
                    new OptionsSubcategory("Background", new List<OptionsItem>()
                    {
                       new OptionsSlider(contentWidth, "Background Brightness", ConfigManager.BackgroundBrightness),
                       new OptionsItemCheckbox(contentWidth, "Enable Background Blur", ConfigManager.BlurBackgroundInGameplay)
                    }),
                    new OptionsSubcategory("Sound", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Enable Hitsounds", ConfigManager.EnableHitsounds),
                        new OptionsItemCheckbox(contentWidth, "Enable Keysounds", ConfigManager.EnableKeysounds)
                    }),
                    new OptionsSubcategory("Input", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Enable Tap To Pause", ConfigManager.TapToPause),
                        new OptionsItemCheckbox(contentWidth, "Skip Results Screen After Quitting", ConfigManager.SkipResultsScreenAfterQuit)
                    }),
                    new OptionsSubcategory("User Interface", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Show Spectators", ConfigManager.ShowSpectators),
                        new OptionsItemCheckbox(contentWidth, "Display Timing Lines", ConfigManager.DisplayTimingLines),
                        new OptionsItemCheckbox(contentWidth, "Display Judgement Counter", ConfigManager.DisplayJudgementCounter),
                        new OptionsItemCheckbox(contentWidth, "Enable Combo Alerts", ConfigManager.DisplayComboAlerts),
                        new OptionsItemCheckbox(contentWidth, "Enable Accuracy Display Animations", ConfigManager.SmoothAccuracyChanges),
                    }),
                    new OptionsSubcategory("Scoreboard", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Display Scoreboard", ConfigManager.ScoreboardVisible),
                        new OptionsItemCheckbox(contentWidth, "Display Unbeatable Scores", ConfigManager.DisplayUnbeatableScoresDuringGameplay)
                    }),
                    new OptionsSubcategory("Progress Bar", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Display Song Time Progress Bar", ConfigManager.DisplaySongTimeProgress),
                        new OptionsItemCheckbox(contentWidth, "Display Song Time Progress Bar Time Numbers", ConfigManager.DisplaySongTimeProgressNumbers)
                    }),
                    new OptionsSubcategory("Lane Cover", new List<OptionsItem>()
                    {
                       new OptionsItemCheckbox(contentWidth, "Enable Top Lane Cover", ConfigManager.LaneCoverTop),
                       new OptionsSlider(contentWidth, "Top Lane Cover Height", ConfigManager.LaneCoverTopHeight),
                       new OptionsItemCheckbox(contentWidth, "Enable Bottom Lane Cover", ConfigManager.LaneCoverBottom),
                       new OptionsSlider(contentWidth, "Bottom Lane Cover Height", ConfigManager.LaneCoverBottomHeight),
                       new OptionsItemCheckbox(contentWidth, "Display UI Elements Over Lane Covers", ConfigManager.UIElementsOverLaneCover)
                    }),
                    new OptionsSubcategory("Multiplayer", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Enable Battle Royale Alerts", ConfigManager.EnableBattleRoyaleAlerts),
                        new OptionsItemCheckbox(contentWidth, "Enable Battle Royale Background Flashing", ConfigManager.EnableBattleRoyaleBackgroundFlashing)
                    })
                }),
                new OptionsSection("Skin", FontAwesome.Get(FontAwesomeIcon.fa_check), new List<OptionsSubcategory>
                {
                }),
                new OptionsSection("Input", FontAwesome.Get(FontAwesomeIcon.fa_keyboard), new List<OptionsSubcategory>
                {
                }),
                new OptionsSection("Network", FontAwesome.Get(FontAwesomeIcon.fa_earth_globe), new List<OptionsSubcategory>
                {
                    new OptionsSubcategory("Login", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Automatically Log Into The Server", ConfigManager.AutoLoginToServer),
                    }),
                    new OptionsSubcategory("Notifications", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Display Online Friend Notifications", ConfigManager.DisplayFriendOnlineNotifications),
                        new OptionsItemCheckbox(contentWidth, "Display Song Request Notifications", ConfigManager.DisplaySongRequestNotifications)
                    })
                }),
                new OptionsSection("Integration", FontAwesome.Get(FontAwesomeIcon.fa_plus_black_symbol), new List<OptionsSubcategory>
                {
                    new OptionsSubcategory("Games", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Load Songs From Other Installed Games", ConfigManager.AutoLoadOsuBeatmaps)
                    }),
                }),
                new OptionsSection("Miscellaneous", FontAwesome.Get(FontAwesomeIcon.fa_question_sign), new List<OptionsSubcategory>
                {
                    new OptionsSubcategory("Effects", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Display Menu Audio Visualizer", ConfigManager.DisplayMenuAudioVisualizer),
                    }),
                    new OptionsSubcategory("Song Select", new List<OptionsItem>()
                    {
                        new OptionsItemCheckbox(contentWidth, "Display Failed Local Scores", ConfigManager.DisplayFailedLocalScores)
                    })
                }),
            };

            SelectedSection = new Bindable<OptionsSection>(Sections.First()) { Value = Sections.First() };
        }

        /// <summary>
        /// </summary>
        private void CreateHeader() => Header = new OptionsHeader(SelectedSection, Width, Sidebar.Width)
        {
            Parent = this,
            Alignment = Alignment.TopLeft
        };

        /// <summary>
        /// </summary>
        private void CreateSidebar()
        {
            Sidebar = new OptionsSidebar(SelectedSection, Sections, new ScalableVector2(OptionsSidebar.WIDTH,
                Height - OptionsHeader.HEIGHT))
            {
                Parent = this,
                Y = OptionsHeader.HEIGHT
            };
        }

        /// <summary>
        /// </summary>
        private void CreateContainer()
        {
            Content = new OptionsContent(new ScalableVector2(Width - Sidebar.Width + 2,
                Height - OptionsHeader.HEIGHT))
            {
                Parent = this,
                Alignment = Alignment.TopLeft,
                X = Sidebar.Width - 2,
                Y = OptionsHeader.HEIGHT
            };
        }

        /// <summary>
        /// </summary>
        private void CreateContentContainers()
        {
            ContentContainers = new Dictionary<OptionsSection, OptionsContentContainer>();

            foreach (var section in Sections)
                ContentContainers.Add(section, new OptionsContentContainer(section, Content.Size));
        }

        /// <summary>
        /// </summary>
        private void SetActiveContentContainer()
        {
            foreach (var container in ContentContainers)
                container.Value.Parent = SelectedSection.Value == container.Key ? Content : null;
        }
    }
}