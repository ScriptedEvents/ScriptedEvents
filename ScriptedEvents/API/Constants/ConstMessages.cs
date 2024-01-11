namespace ScriptedEvents.API.Constants
{
    public static class ConstMessages
    {
        public const string RoomInput = @"The following inputs can be used to target rooms:
- '*'/'ALL' - Targets ALL rooms
- 'LightContainment' - Targets light containment rooms
- 'HeavyContainment' - Targets heavy containment rooms
- 'Entrance' - Targets entrance rooms
- The ID of a room. All valid room IDs can be found on Exiled's documentation at: https://exiled-team.github.io/EXILED/api/Exiled.API.Enums.RoomType.html";

        public const string GotoInput = @"The following keywords can be used in place of a label:
- START - Moves to the start of the script.
- STOP - Immediately stops the script.
- NEXT - Moves to the next line.";
    }
}
