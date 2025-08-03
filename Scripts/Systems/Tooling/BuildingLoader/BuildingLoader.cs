using Godot;
using Godot.Collections;

namespace ClawtopiaCs.Scripts.Systems.Tooling
{
    [GlobalClass, Tool]
    public partial class BuildingLoader : Node
    {
        public static BuildingLoader Singleton { get; set; }

        [ExportCategory("Loader config")]
        [Export] public BuildingLoaderSettings Settings;

        [ExportCategory("Buildings")]
        [Export]
        public BuildingList Buildings { get; set; }

        public override void _EnterTree()
        {
            Singleton = this;
            if (Engine.IsEditorHint())
            {
                if (Settings == null || string.IsNullOrEmpty(Settings.ResourceDirectory)) { return; }

                string buildingListPath = $"{Settings.ResourceDirectory}/Buildings/BuildingList.tres";
                if (ResourceLoader.Exists(buildingListPath))
                {
                    Buildings = ResourceLoader.Load<BuildingList>(buildingListPath);
                }
                else
                {
                    Buildings = new BuildingList();
                    ResourceSaver.Save(Buildings, buildingListPath);
                }
            }
        }

        public static Array<string> GetBuildingNames(BuildingList buildings)
        {
            if (buildings == null)
            {
                GD.PushError("Building List is not set.");
                return [];
            }

            if (buildings.List == null || buildings.List.Count == 0)
            {
                GD.PushError("Building List is empty.");
                return [];
            }

            Array<string> buildingNames = [];

            foreach (var building in buildings.List)
            {
                if (building != null && !string.IsNullOrEmpty(building.Name))
                {
                    buildingNames.Add(building.Name);
                }
            }

            return buildingNames;
        }

        public override void _Ready()
        {
            Singleton = this;
        }

        public Building LoadBuilding(BuildingData buildingData)
        {
            if (buildingData == null) return null;
            Building building = Settings.BaseBuilding.Instantiate<Building>();
            building.Data = buildingData;
            building.Initialize();
            GD.Print("DATA Load: ", building.Data);
            return building;
        }
    }


}
