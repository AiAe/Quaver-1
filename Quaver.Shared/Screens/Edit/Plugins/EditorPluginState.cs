using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using MoonSharp.Interpreter;
using MoonSharp.Interpreter.Interop;
using Quaver.API.Enums;
using Quaver.API.Maps.Structures;
using Quaver.Shared.Scripting;
using Wobble.Graphics.ImGUI;

namespace Quaver.Shared.Screens.Edit.Plugins
{
    [MoonSharpUserData]
    public class EditorPluginState : LuaPluginState
    {
        /// <summary>
        ///     The current time in the song
        /// </summary>
        public double SongTime { get; [MoonSharpVisible(false)] set; }

        /// <summary>
        ///     The objects that are currently selected by the user
        /// </summary>
        public List<HitObjectInfo> SelectedHitObjects { get; [MoonSharpVisible(false)] set; }

        /// <summary>
        ///     The current timing point in the map
        /// </summary>
        public TimingPointInfo CurrentTimingPoint { get; [MoonSharpVisible(false)] set; }

        /// <summary>
        ///     The currently selected editor layer
        /// </summary>
        public EditorLayerInfo CurrentLayer { get; [MoonSharpVisible(false)] set; }

        /// <summary>
        ///     The currently selected beat snap
        /// </summary>
        public int CurrentSnap { get; [MoonSharpVisible(false)] set; }

        /// <summary>
        ///     ImGui options used to set styles/fonts for the window
        /// </summary>
        [MoonSharpVisible(false)]
        private ImGuiOptions Options { get; }

        [MoonSharpVisible(false)]
        public EditorPluginState(ImGuiOptions options) => Options = options;

        /// <summary>
        ///     Pushes all styles to the current imgui context
        /// </summary>
        public void PushImguiStyle() => ImGui.PushFont(Options.Fonts.First().Context);
    }
}
