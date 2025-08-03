using ClawtopiaCs.Scripts.Entities;
using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Systems.Interactions
{
    public class StateInteractions
    {
        public static Dictionary<InteractionStates, Color> Colors = new() {
            { InteractionStates.HOVER, StateColors.HoverColor() },
            { InteractionStates.UNHOVER, StateColors.RegularColor() },
            { InteractionStates.ERROR, StateColors.ErrorColor() },
            { InteractionStates.OK, StateColors.OkColor() },
            { InteractionStates.FINISHED, StateColors.RegularColor() },
        };
    }
}
