using Godot;
using System;
using System.Collections.Generic;

public partial class Constants : Node
{
	public const string ECONOMIC = "Economic";
	public const string MILITARY = "Military";
	public const string SALMON = "Salmon";
	public const string CATNIP = "Catnip";
	public const string SAND = "Sand";
	public const string COMMUNE = "Commune";
	public const string RESOURCE = "Resource";
	public const string TOWER = "Tower";
	public const string HOUSE = "House";
	public const string BUILDING_PATH = "res://TSCN/Entities/Buildings/Building.tscn";
	public const string FIGHTERS = "Fighters";
	public const string COMMUNIST = "Communist";
	public const string BUILDING_MENU = "BuildingMenu";
	public const string COMMUNIST_MENU = "CommunistMenu";
	public const string PURRLAMENT_MENU = "PurrLamentMenu";
	public const string BASE_MENU = "BaseMenu";
	public const string MENU_LIST = BUILDING_MENU + "," 
								  + COMMUNIST_MENU + ","
								  + BASE_MENU;
	public const string RESOURCE_LIST = SALMON + ","
									  + CATNIP + ","
									  + SAND;
	public const string BUILDING_LIST = TOWER + ","
									+ RESOURCE + ","
									+ COMMUNE + ","
									+ HOUSE;
	public const string TOWER_LIST = FIGHTERS; /*MODIFICAR COM NOVOS TIPOS DE TORRE SEGUINDO MODELO ACIMA*/

	public const string HOUSE_EXTERNAL_NAME = "Casa da Comuna";
	public const string COMMUNE_EXTERNAL_NAME = "Purrlamento";
	public const string FIGHTERS_TOWER_EXTERNAL_NAME = "Torre de Lutadores";
	public const string FISHERMAN_HOUSE_EXTERNAL_NAME = "Cabana do Pescador";
}
