using ClawtopiaCs.Scripts.Entities;
using ClawtopiaCs.Scripts.Entities.Building;
using ClawtopiaCs.Scripts.Systems;
using Godot;
using Godot.Collections;
using ClawtopiaCs.Scripts.Systems.GameModes;
using System.Linq;
using System;

[GlobalClass, Tool]
public partial class CollectPoint : StaticBody2D
{
    [Signal] public delegate void ResourceCollectedEventHandler(int quantity);

    private int _resourceQuantity = 10;
    public int SelfIndex;

    [ExportGroup("Settings")]
    private CollectableList _collectables;

    [Export(PropertyHint.File, "*.tres")]
    public CollectableList Collectables
    {
        get => _collectables;
        set
        {
            _collectables = value;
            if (Engine.IsEditorHint())
            {
                CallDeferred(MethodName.NotifyPropertyListChanged);
            }
        }
    }

    [ExportGroup("Node Refs")]
    [Export] public Area2D Interaction;
    [Export] public CollisionPolygon2D InteractionShape;
    [Export] public CollisionPolygon2D BodyShape;

    [ExportGroup("Data")]
    private string _collectable;

    [Export(PropertyHint.Enum)]
    public string Collectable
    {
        get => _collectable;
        set
        {
            if (_collectable == value) return;
            _collectable = value;

            if (Engine.IsEditorHint())
            {
                if (string.IsNullOrEmpty(value))
                {
                    Resource = null;
                    CollectableEditor.Reset(this);
                    return;
                }
                if (Resource != null && Resource.Name == value) return;
                Resource = CollectableEditor.LoadCollectable(value, Collectables);
                
                if (!IsNodeReady()) { return; }
                CollectableEditor.ReloadCollectPoint(this);
            }
        }
    }
    [Export] public int MaxResourceQuantity = 10;

    [Export] public Collectable Resource;
    public Sprite2D StaticSprite = new();
    public AnimatedSprite2D AnimatedSprite = new();

    [Export]
    public int ResourceQuantity
    {
        get => _resourceQuantity;
        set
        {
            _resourceQuantity = value;
            if (_resourceQuantity - value < 0)
            {
                _resourceQuantity = 0;
            }
            CallDeferred(MethodName.ChangeSpriteOnBreakpoint);
        }
    }

    public override void _Ready()
    {
        CallDeferred(MethodName.Initialize);
    }

    public void Initialize()
    {
        if (Resource == null) return;
        CallDeferred(MethodName.ChangeSpriteOnBreakpoint);
        CallDeferred(MethodName.SetCollisionShapes);

        if (Engine.IsEditorHint()) return;
        
        InputPickable = true;
        LevelManager.Singleton.CollectPoints.Add(this);
        SelfIndex = LevelManager.Singleton.CollectPoints.Count;
        Interaction.MouseEntered += OnHover;
        Interaction.MouseExited += OnUnhover;
        ResourceCollected += OnResourceCollected;
        ResourceQuantity = MaxResourceQuantity;
    }

    private void OnResourceCollected(int quantity)
    {
        ResourceQuantity -= quantity;
    }

    public virtual void OnHover()
    {
        Modulation.AssignState(this, InteractionStates.HOVER);
        if (SimulationMode.Singleton.SelectedAllies.Count > 0)
        {
            CustomCursor.Instance.SetCursor(Resource.HoverCursor);
        }
    }
    
    public virtual void OnUnhover()
    {
       CustomCursor.Instance.ResetCursor();
       Modulation.AssignState(this, InteractionStates.UNHOVER);
    }

    private void SetCollisionShapes()
    {
        BodyShape.Polygon = Resource?.BodyShape?.Segments ?? null;
        InteractionShape.Polygon = Resource?.Interaction?.Segments ?? null;
    }

    private void SetStaticSprite(ProgressStructure progressStructure, ProgressStructure.States state)
    {
        ClearExistingSprites();

        StaticSprite = new Sprite2D
        {
            Texture = progressStructure.StaticTextures[(int) state],
            Name = "StaticSprite"
        };


        AddChild(StaticSprite);
        StaticSprite.Owner = this;
        QueueRedraw();
    }

    private void SetAnimatedSprite(ProgressStructure progressStructure, ProgressStructure.States state)
    {
        ClearExistingSprites();

        AnimatedSprite = new AnimatedSprite2D
        {
            SpriteFrames = progressStructure.AnimatedTextures,
            Name = "AnimatedSprite",
            Animation = ProgressStructure.Animations[(int) state]
        };

        AddChild(AnimatedSprite);
        AnimatedSprite.Owner = this;
        QueueRedraw();
        AnimatedSprite.Play();
    }

    private void ClearExistingSprites()
    {
        if (IsInstanceValid(StaticSprite))
        {
            RemoveExistingSprites(StaticSprite.GetClass());
        }

        if (IsInstanceValid(AnimatedSprite))
        {
            RemoveExistingSprites(AnimatedSprite.GetClass());
        }
    }

    private void RemoveExistingSprites(string sprite)
    {
        var sprites = new Array<Node>(GetChildren().Where(c => c.IsClass(sprite)));
        if (sprites != null)
        {
            foreach (var item in sprites)
            {
                item.QueueFree();
                item.Dispose();
            }
        }
    }

    public void ChangeSpriteOnBreakpoint() {

        switch (Resource.ProgressStructure.Type)
        {
            case ProgressStructure.TextureType.Static:
                SetStaticSprite(Resource.ProgressStructure, GetResourceState());
                break;
            case ProgressStructure.TextureType.Animated:
                SetAnimatedSprite(Resource.ProgressStructure, GetResourceState());
                break;
        }
    }

    private ProgressStructure.States GetResourceState()
    {
        if (ResourceQuantity > MaxResourceQuantity * 0.75)
        {
            return ProgressStructure.States.Full;
        }
        else if (ResourceQuantity > MaxResourceQuantity * 0.5)
        {
            return ProgressStructure.States.Medium;
        }
        else if (ResourceQuantity > MaxResourceQuantity * 0.25)
        {
            return ProgressStructure.States.Low;
        }
        else
        {
            return ProgressStructure.States.Empty;
        }
    }

    public override void _ValidateProperty(Dictionary property)
    {
        if (!Engine.IsEditorHint())
        {
            base._ValidateProperty(property);
            return;
        };

        if (property["name"].AsStringName() == PropertyName.Collectable)
        {
            var usage = property["usage"].As<PropertyUsageFlags>() | PropertyUsageFlags.ReadOnly;
            property["hint_string"] = string.Join(",", CollectableList.GetNames(Collectables.Resources));
        }

        base._ValidateProperty(property);
    }
}

