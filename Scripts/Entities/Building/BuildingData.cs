using Godot;
using System;

public partial class BuildingData : Resource
{

    public ConcavePolygonShape2D obstacle_shape;
    public RectangleShape2D interaction_shape;
    public Texture2D sprite_texture;
    public Vector2 sprite_offset;
    public Vector2 sprite_size;
    public Vector2 interaction_offset;

    [Export]
    public String type;
    [Export]
    public String name;
    [Export]
    public int HP;

    private int level;

    public void initialize() {
        switch(type) {
            case "Resource":
                break;
            case "Tower":
                GD.Print("Initializing tower");
                level = 1;
                obstacle_shape = (ConcavePolygonShape2D)GD.Load("res://Resources/Buildings/Towers/Fighters/obstacle_shape.tres");
                interaction_shape = (RectangleShape2D)GD.Load("res://Resources/Buildings/Towers/Fighters/interaction_shape.tres");
                switch (name) {
                    case "Fighters":
                        sprite_texture = (Texture2D)GD.Load("res://Assets/Buildings/torreA.png");
                        sprite_offset = new Vector2(0, -120);
                        sprite_size = new Vector2(1, 1);
                        interaction_offset = new Vector2(0, -120);
                        break;
                }
                break;
            case "GreatCommune":
                GD.Print("Initializing GreatCommune");
                level = 1;
                sprite_texture = (Texture2D)GD.Load("res://Assets/Buildings/building_01_tavern.png");
                sprite_offset = new Vector2(-52, -224);
                sprite_size = new Vector2(0.3f, 0.3f);
                obstacle_shape = (ConcavePolygonShape2D)GD.Load("res://Resources/Buildings/GreatCommune/obstacle_shape.tres");
                interaction_shape = (RectangleShape2D)GD.Load("res://Resources/Buildings/GreatCommune/interaction_shape.tres");
                interaction_offset = new Vector2(-25, -50);
                break;
        }
    }
}
