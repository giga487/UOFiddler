# Editing SeaHats quests in JSON and Fiddler

Quests.Json is the editable catalog. The current stories are examples; their names, journal text, dialogue, steps, objectives, markers and gameplay conditions can be rewritten for the official map. Fiddler preserves extra JSON fields at catalog, quest and step level, including Triggers, MapMarkers, World and future fields. Editing a normal text field, replacing a step, duplicating a quest, exporting or saving the catalog retains these fields. Duplicated extension objects are independent from the original.

The existing visual controls edit the original fields. Edit gameplay extensions directly in JSON, including with an AI, and reopen the file in Fiddler. Unknown fields are preserved, but the game only executes fields supported by its quest runtime.

## Instructions for an author or AI

- Preserve the existing catalog structure: Quests is an object keyed by numeric quest ID; Steps is an object keyed by numeric step ID. ID and Step must agree with their object keys.
- Quest 1 is currently assigned automatically to new players; completing it unlocks quest 2. Keep these IDs when replacing the example main story. Changing the automatic starting/unlocked IDs requires changing that policy in SeaHats.
- Step 0 is the offer/introduction; 32767 is the conclusion. Playable steps use increasing positive IDs. Keep IDs stable once characters have saved progress.
- Write journal text in Text, NPC dialogue in NpcGumpText, and ordered objective strings in StepObjective. Group is the left-column heading and Priority is the quest category (0 primary, 1 secondary).
- Use StepDataContainers: [] for steps driven by the newer Triggers. Do not replace Triggers with the legacy container types.
- Each trigger has a unique Name, a supported Type, a Target, and a positive RequiredCount (default 1). Names in After refer to trigger Names within that same step; all prerequisites must be complete before a later event can count. A single event cannot finish successive tasks of the same type.
- MinimumValue is the minimum Base skill for SkillLearned/CombatPractice. MapId limits the map. Region and RegionType limit the action location; as requested, a missing/unregistered region filter imposes no region restriction. Check that expected regions actually exist on the official map.
- MapMarkers is a per-step array of Name, MapId, X and Y. Markers follow the active step. Remove them from later steps when the destination should disappear.
- ObjectiveIndex optionally binds a trigger to a zero-based StepObjective row. With matching array lengths, triggers bind by position; otherwise unassigned triggers bind to the last row. The journal displays current/required counters and marks completed or locked requirements automatically. Several triggers on one row sum their displayed amounts, and all must finish for the row to complete.
- In Text or StepObjective, use `{progress:trigger_name}`, `{current:trigger_name}`, `{required:trigger_name}` and `{remaining:trigger_name}` for live text. At 2 of 10, these become 2/10, 2, 10 and 8. Trigger names are case-sensitive. The server and client must both include the progress update; these fields and placeholders survive Fiddler saves.
- World currently describes a rat islet: MapId, Region, Bounds (X, Y, Width, Height), Spawn (X, Y, Z). It creates its region and QuestIsletRat spawner. Another mob's spawn needs a corresponding world/spawner implementation.

## Event names and targets

| Type | Target / producer |
|---|---|
| FishingCatch | Fish, after a successful catch reaches the backpack |
| FoodCooked | Concrete food type, e.g. FishSteak, after successful cooking reaches the backpack |
| SkillLearned | Combat, from an NPC lesson or valid practice with an already learned discipline |
| CombatPractice | Combat, after valid weapon/spell/dummy/butte practice |
| NpcTalk | Julius, after a valid conversation; other NPCs need to emit their own event |
| VendorPurchase | PersonalCrewFlag, after Marlowe's actual sale |
| CrewFounded | Crew, after successful foundation (Target can be omitted) |
| ShipCrafted | Ship, after a paid Crew Shipyard recipe |
| ItemDelivery | Board, Cloth or IronIngot, after successful shipyard material consumption |
| ShipDeparted | Existing harbour region name, e.g. StartCity; physical crew vessel movement across its boundary with member aboard |
| EnterArea | Registered region name, after the player enters it |
| KillMob | Concrete creature class name; Rat includes the quest rat subclass |

## Example step

```json
"2": {
  "Step": 2,
  "StepName": "Bread from the Blue",
  "Text": "Catch and prepare your first meal, then return to Julius.",
  "NpcGumpText": "The sea can feed you if you learn to ask.",
  "StepObjective": ["Catch 1 fish.", "Cook 1 fish steak.", "Return to Julius."],
  "StepDataContainers": [],
  "Triggers": [
    { "Name": "catch", "Type": "FishingCatch", "Target": "Fish", "RequiredCount": 1, "MapId": 0 },
    { "Name": "cook", "Type": "FoodCooked", "Target": "FishSteak", "After": ["catch"], "MapId": 0 },
    { "Name": "return", "Type": "NpcTalk", "Target": "Julius", "After": ["catch", "cook"], "MapId": 0 }
  ],
  "MapMarkers": [{ "Name": "Julius", "MapId": 0, "X": 3566, "Y": 1371 }]
}
```

The server validates trigger names, types, thresholds, quantities and dependencies when loading the catalog. Its gameplay rules remain authoritative. After publishing catalog changes, reload/restart server and client using the same file. Fiddler's encrypted export also preserves extension data, but the current gameplay extensions must be consumed from the unencrypted runtime catalog.

## Verification

Run from the UOFiddler repository:

```text
dotnet run --project tests/QuestCatalog.Tests -- path/to/Quests.Json
```

The test uses its own temporary files: edit/save/export, quest and step copying, clone isolation, encrypted round trip, and preservation of every actual quest's triggers, markers, objectives and world definition. It does not write to the supplied catalog.
