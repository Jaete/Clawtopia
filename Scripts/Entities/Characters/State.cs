using System;
using ClawtopiaCs.Scripts.Systems.GameModes;
using Godot;

public partial class State : Node
{
    [Signal]
    public delegate void StateTransitionEventHandler(State current, String next);

    [ExportGroup("Settings")]
    [Export] public AnimController AnimationController { get; set; }

    public Unit Unit;

    public virtual void Enter() {}

    public virtual void Update(double delta) {}

    public virtual void Exit() {}

    public virtual void NavigationFinished() {}

    public virtual void CommandReceived(Vector2 coords) {}

    /// <summary>
    /// Inicializacao basica para todos os estados de qualquer unidade.
    /// Cada estado chama esta funcao em sua própria <c>_Ready()</c> e após isso inicializa dados especificos
    /// daquela unidade e estado. <br></br><br></br>&#10;
    /// </summary>
    public void InitializeUnit()
    {
        Unit = GetParent().GetParent<Unit>();
        Unit.Navigation.VelocityComputed += VelocityComputed;
    }


    /// <summary>
    /// <param name="next">Para qual estado ir</param>
    /// Emite o sinal de <c>StateTransition</c> para trocar de um estado para o outro. 
    /// </summary>
    public void ChangeState(string next)
    {
        EmitSignal("StateTransition", this, next);
    }


    /// <summary>
    /// <param name="safeVelocity">Velocidade pós calculo de avoidance.</param>
    /// Recebe a velocidade, multiplica pela velocidade de movimento da unidade e modifica a velocidade do CharacterBody, após isso chamando <c>MoveAndSlide</c>
    /// </summary>
    public void VelocityComputed(Vector2 safeVelocity)
    {
        Unit.Velocity = safeVelocity * Unit.Attributes.MoveSpeed;
        Unit.MoveAndSlide();
    }

    /// <summary>
    /// Pega a próxima posição de navegação, calcula a distância e modifica a velocidade do navigationAgent.<br></br><br></br>
    /// Caso avoidance esteja ligado, ele aguarda pelo signal <c>VelocityComputed</c> para receber a nova velocidade após o calculo de avoidance.<br></br><br></br>
    /// Caso esteja desligado, chama a função de mover o aliado com a velocidade desconsiderando avoidance.
    /// </summary>
    public void Move()
    {
        var nextPathPos = Unit.Navigation.GetNextPathPosition();
        var newVelocity = Unit.GlobalPosition.DirectionTo(nextPathPos);

        if (Unit.Navigation.AvoidanceEnabled)
        {
            Unit.Navigation.Velocity = newVelocity;
        }
        else
        {
            VelocityComputed(newVelocity);
        }
    }
}