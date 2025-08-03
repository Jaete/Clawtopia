using Godot;

public partial class Constants : Node
{
    public const string NOT_DEFINED = "Not Defined";


    public const string ECONOMIC = "Economic";
    public const string MILITARY = "Military";
    public const string SALMON = "Salmon";
    public const string CATNIP = "Catnip";
    public const string SAND = "Sand";
    public const string COMMUNE = "Commune";
    public const string RESOURCE = "Resource";
    public const string TOWER = "Tower";
    public const string HOUSE = "House";
    public const string FIGHTERS = "Fighters";
    public const string COMMUNIST = "Communist";

    public const string BUILDING_MENU = "BuildingMenu";
    public const string COMMUNIST_MENU = "CommunistMenu";
    public const string PURRLAMENT_MENU = "PurrLamentMenu";
    public const string HOUSE_MENU = "HouseMenu";
    public const string BASE_MENU = "BaseMenu";

    public const string MENU_LIST = BASE_MENU +
                                    COMMUNIST_MENU +
                                    BUILDING_MENU;

    public const string RESOURCE_LIST = SALMON + ","
                                               + CATNIP + ","
                                               + SAND;

    public const string BUILDING_LIST = TOWER + ","
                                              + RESOURCE + ","
                                              + COMMUNE + ","
                                              + HOUSE;

    public const string RESOURCES_QTY_LEVEL = "Full, Mid, Low, None";

    public const string TOWER_LIST = FIGHTERS; /*MODIFICAR COM NOVOS TIPOS DE TORRE SEGUINDO MODELO ACIMA*/
    public const string ALLY_CATEGORY_LIST = ECONOMIC + "," + MILITARY;

    public const string HOUSE_EXTERNAL_NAME = "Casa da Comuna";
    public const string COMMUNE_EXTERNAL_NAME = "Purrlamento";
    public const string FIGHTERS_TOWER_EXTERNAL_NAME = "Torre de Lutadores";
    public const string FISHERMAN_HOUSE_EXTERNAL_NAME = "Cabana do Pescador";
    public const string DISTILLERY_EXTERNAL_NAME = "Destilaria";
    public const string SAND_MINE_EXTERNAL_NAME = "Mina de Areia";

    public const string BUILDING_EXTERNAL_NAME_LIST =
        HOUSE_EXTERNAL_NAME + "," +
        COMMUNE_EXTERNAL_NAME + "," +
        FIGHTERS_TOWER_EXTERNAL_NAME + "," +
        FISHERMAN_HOUSE_EXTERNAL_NAME + "," +
        DISTILLERY_EXTERNAL_NAME + "," +
        SAND_MINE_EXTERNAL_NAME;

    public const string BUILDING_PATH = "res://TSCN/Entities/Buildings/Building.tscn";
    public const string MODE_MANAGER_PATH = "/root/Game/ModeManager";
    public const string BUILD_MODE_PATH = "/root/Game/ModeManager/BuildMode";
    public const string SIMULATION_MODE_PATH = "/root/Game/ModeManager/SimulationMode";
    public const string LEVEL_MANAGER_PATH = "/root/Game/LevelManager";
}