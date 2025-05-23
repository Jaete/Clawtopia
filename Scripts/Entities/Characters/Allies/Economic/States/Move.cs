using System.Drawing;
using ClawtopiaCs.Scripts.Systems;
using Godot;
using Godot.Collections;

public partial class Move : EconomicState
{
    public override void Enter() {}

    public override void Update(double delta)
    {
        Move();
    }

    public override void Exit() {}

    public override void CommandReceived(Vector2 coords)
    {
        if (!Ally.CurrentlySelected) { return; }

        ChooseNextTargetPosition(coords);
    }

    public override void NavigationFinished()
    {
        if (Ally.AllyIsBuilding)
        {
            ChangeState("Building");
            return;
        }

        if (Ally.InteractedWithBuilding)
        {
            if (!Ally.InteractedBuilding.IsBuilt)
            {
                Ally.ConstructionToBuild = Ally.InteractedBuilding;
                ChangeState("Building");
                return;
            }

            switch (Ally.InteractedBuilding.Data.Type)
            {
                case Constants.TOWER:
                    ChangeState("Taking_shelter");
                    return;
                case Constants.RESOURCE:
                    EconomicBehaviour.SeekResource(Ally, Ally.InteractedBuilding.Data.ResourceType);
                    return;
            }
        }

        if (Ally.InteractedResource != null && !Ally.Delivering)
        {
            ChangeState("Collecting");
            return;
        }

        if (Ally.Delivering)
        {
            EconomicBehaviour.DeliverResource(Ally);
            return;
        }

        ChangeState("Idle");
    }
}