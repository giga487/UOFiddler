using Newtonsoft.Json.Linq;
using SeaHatsExternal.Crypto;
using SeaHatsExternal.Quests;

var originalDirectory = Directory.GetCurrentDirectory();
var temporaryDirectory = Path.Combine(Path.GetTempPath(), "fiddler-quest-catalog-" + Guid.NewGuid().ToString("N"));
Directory.CreateDirectory(temporaryDirectory);
var checks = 0;
try
{
    Directory.SetCurrentDirectory(temporaryDirectory);
    var source = JObject.Parse("""
        {
          "CatalogVersion": 12, "FutureRoot": { "Value": null, "Enabled": false },
          "Quests": { "1001": {
            "ID": 1001, "QuestName": "An island errand", "Group": "Island Errands", "Priority": 1,
            "World": { "MapId": 0, "Region": "Custom Islet", "Bounds": { "X": 10, "Y": 20, "Width": 30, "Height": 40 },
              "Spawn": { "X": 15, "Y": 25, "Z": 0 }, "FutureSpawn": { "Species": "Rat" } },
            "FutureQuest": [1, "two", null],
            "Steps": { "1": {
              "Step": 1, "StepName": "First steps", "Text": "Original text", "NpcGumpText": "Original dialogue",
              "StepObjective": ["Catch a fish", "Return to Julius"], "StepDataContainers": [],
              "Triggers": [
                { "Name": "catch", "Type": "FishingCatch", "Target": "Fish", "RequiredCount": 2,
                  "MapId": 0, "Region": "Custom Islet", "RegionType": "BaseRegion", "MinimumValue": 20,
                  "After": [], "FutureTrigger": { "Tags": ["a", "b"] } },
                { "Name": "talk", "Type": "NpcTalk", "Target": "Julius", "After": ["catch"] }
              ],
              "MapMarkers": [{ "Name": "Harbour", "MapId": 0, "X": 15, "Y": 25, "FutureMarker": true }],
              "FutureStep": { "Optional": null, "Amount": 0 }
            } }
          } }
        }
        """);
    var input = Path.Combine(temporaryDirectory, "input.json");
    var output = Path.Combine(temporaryDirectory, "output.json");
    File.WriteAllText(input, source.ToString());
    var manager = new QuestSeaHatsManager();
    Check(manager.LoadQuestJson(input), "JSON import succeeds");
    var quest = manager.Data.Quests[1001];
    var edited = new QuestDataStep(quest.Steps[1]) { Text = "Edited in Fiddler", NpcGumpText = "Edited dialogue" };
    manager.UpdateStep(1001, edited);
    Check(manager.SaveJsonQuest(output), "Saving an edited step succeeds");
    var saved = JObject.Parse(File.ReadAllText(output));
    AssertExtensions(source, saved);
    Check(saved["Quests"]!["1001"]!["Steps"]!["1"]!["Text"]!.Value<string>() == "Edited in Fiddler", "Ordinary editor changes are saved");
    AssertExtensions(source, JObject.Parse(manager.ExtractDataJson()));

    Check(manager.CloneQuest(1001), "Quest duplication succeeds");
    var clone = manager.Data.Quests[1002];
    Check(JToken.DeepEquals(quest.ExtensionData["World"], clone.ExtensionData["World"]), "Duplicate retains the world definition");
    Check(JToken.DeepEquals(edited.ExtensionData["Triggers"], clone.Steps[1].ExtensionData["Triggers"]), "Duplicate retains trigger dependencies and future fields");
    clone.ExtensionData["World"]["MapId"] = 7;
    clone.Steps[1].ExtensionData["Triggers"][0]!["RequiredCount"] = 99;
    clone.Steps[1].ExtensionData["MapMarkers"][0]!["X"] = 999;
    Check(quest.ExtensionData["World"]["MapId"]!.Value<int>() == 0, "Duplicate world changes do not affect the original");
    Check(edited.ExtensionData["Triggers"][0]!["RequiredCount"]!.Value<int>() == 2, "Duplicate trigger changes do not affect the original");
    Check(edited.ExtensionData["MapMarkers"][0]!["X"]!.Value<int>() == 15, "Duplicate marker changes do not affect the original");
    var copiedStep = new QuestDataStep(edited);
    copiedStep.ExtensionData["FutureStep"]["Amount"] = 50;
    Check(edited.ExtensionData["FutureStep"]["Amount"]!.Value<int>() == 0, "Step copies isolate future extension objects");
    var copiedCatalog = new QuestData(manager.Data);
    copiedCatalog.ExtensionData["FutureRoot"]["Enabled"] = true;
    Check(!manager.Data.ExtensionData["FutureRoot"]["Enabled"]!.Value<bool>(), "Catalog copies isolate root extension objects");

    var encrypted = Path.Combine(temporaryDirectory, "encrypted.quest");
    Check(SeaHatsFileManager.EncryptStringToFile(manager.ExtractDataJson(), encrypted, "test-only-password"), "Encrypted export succeeds");
    var restored = new QuestSeaHatsManager();
    Check(restored.LoadQuestJsonCrypted(encrypted, "test-only-password"), "Encrypted import succeeds");
    AssertExtensions(source, JObject.Parse(restored.ExtractDataJson()));
    foreach (var id in new ushort[] { 1001, 1002 })
    {
        Check(restored.Data.Quests[id].Steps[1].Own == restored.Data.Quests[id], "Restored steps retain their correct parent");
    }
    if (args.Length > 0)
    {
        var actualPath = Path.GetFullPath(args[0], originalDirectory);
        var actualSource = JObject.Parse(File.ReadAllText(actualPath));
        Check(manager.LoadQuestJson(actualPath), "Current gameplay catalog loads");
        foreach (var actualQuest in manager.Data.Quests.Values)
        {
            foreach (var step in actualQuest.Steps.Values.ToArray())
            {
                manager.UpdateStep(actualQuest.ID, new QuestDataStep(step) { Notes = step.Notes + " Test edit." });
            }
        }
        Check(manager.SaveJsonQuest(output), "Current gameplay catalog survives editor step replacement");
        var actualSaved = JObject.Parse(File.ReadAllText(output));
        foreach (var property in actualSource["Quests"]!.Children<JProperty>())
        {
            var before = property.Value;
            var after = actualSaved["Quests"]![property.Name]!;
            Check(JToken.DeepEquals(before["World"], after["World"]), "Current world definition survives saving");
            foreach (var step in before["Steps"]!.Children<JProperty>())
            {
                var savedStep = after["Steps"]![step.Name]!;
                Check(JToken.DeepEquals(step.Value["Triggers"], savedStep["Triggers"]), "Current triggers survive saving");
                Check(JToken.DeepEquals(step.Value["MapMarkers"], savedStep["MapMarkers"]), "Current markers survive saving");
                Check(JToken.DeepEquals(step.Value["StepObjective"], savedStep["StepObjective"]), "Current journal objectives survive saving");
            }
        }
    }
    Console.WriteLine($"PASS: {checks} quest catalog preservation checks.");
}
finally
{
    Directory.SetCurrentDirectory(originalDirectory);
    // This exact temporary directory is created above solely for this process's fixtures.
    var resolvedTemporaryDirectory = Path.GetFullPath(temporaryDirectory);
    var temporaryRoot = Path.GetFullPath(Path.GetTempPath()).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
    if (!resolvedTemporaryDirectory.StartsWith(temporaryRoot, StringComparison.OrdinalIgnoreCase) ||
        !Path.GetFileName(resolvedTemporaryDirectory).StartsWith("fiddler-quest-catalog-", StringComparison.Ordinal))
    {
        throw new InvalidOperationException("Test cleanup escaped its temporary fixture directory.");
    }
    Directory.Delete(resolvedTemporaryDirectory, true);
}

void Check(bool condition, string description)
{
    if (!condition)
    {
        throw new InvalidOperationException(description);
    }
    checks++;
}

void AssertExtensions(JObject before, JObject after)
{
    foreach (var name in new[] { "CatalogVersion", "FutureRoot" })
    {
        Check(JToken.DeepEquals(before[name], after[name]), "Root extension survives saving");
    }
    var beforeQuest = before["Quests"]!["1001"]!;
    var afterQuest = after["Quests"]!["1001"]!;
    foreach (var name in new[] { "World", "FutureQuest" })
    {
        Check(JToken.DeepEquals(beforeQuest[name], afterQuest[name]), "Quest extension survives saving");
    }
    foreach (var name in new[] { "Triggers", "MapMarkers", "FutureStep", "StepObjective" })
    {
        Check(JToken.DeepEquals(beforeQuest["Steps"]!["1"]![name], afterQuest["Steps"]!["1"]![name]), "Step extension survives saving");
    }
}
