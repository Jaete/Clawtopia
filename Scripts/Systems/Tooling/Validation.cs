using Godot;
 
namespace ClawtopiaCs.Scripts.Systems.Tooling
{
    public partial class Validation : Resource
    {
        public static bool ValidateResource(Resource item)
        {
            if (item is null)
            {
                GD.PushWarning("O resource não foi adicionado ao node.");
                return false;
            }

            return true;
        }

        public static bool ValidateNewBuilding(string BuildingName, BuildingData NewBuildingData)
        {
            if (!ValidateResource(NewBuildingData))
            {
                EditorUI.PopupAccept("New building Data is not set. Add a new Building Data before adding the building.");
                return false;
            }

            if (!ValidateResource(NewBuildingData.Structure))
            {
                EditorUI.PopupAccept("New building Structure is not set. Add a new Building Structure inside the new Building Data before adding the building.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(BuildingName))
            {
                EditorUI.PopupAccept("Building name is not set. Set a name before adding the building.");
                return false;
            }

            if (!ValidateResource(NewBuildingData.Structure.PreviewTexture))
            {
                EditorUI.PopupAccept("New building preview texture is not set. Set a preview texture before adding the building.");
                return false;
            }

            if (!ValidateResource(NewBuildingData.Structure.RotatedPreviewTexture))
            {
                EditorUI.PopupAccept("New building rotated preview texture is not set. Set a placed texture before adding the building.");
                return false;
            }

            return true;
        }

        public static bool ValidateNewCollectable(Collectable collectable)
        {
            if (collectable.Name == null)
            {
                EditorUI.PopupAccept("New Collectable name is not set. Set before saving.");
                return false;
            }

            return true;
        }
    }


}
