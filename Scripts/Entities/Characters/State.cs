using Godot;
using System;


public partial class State : Node
{
    [Signal]
    public delegate void StateTransitionEventHandler(State current, String next);
    
    // Referências recorrentes
    public Unit Unit;
    public ClawtopiaCs.Scripts.Systems.GameModes.SimulationMode SimulationMode;
    public BuildMode BuildMode;
    public ModeManager ModeManager;
    public Controller Controller;
    
    // Funçôes recorrentes na maquina de estado
    public virtual void Enter(){}
    public virtual void Update(double delta){}
    public virtual void Exit(){}
    public virtual void MouseRightClicked(Vector2 coords){}
    public virtual void NavigationFinished(){}

    
    /// <summary>
    /// Inicializacao basica para todos os estados de qualquer unidade.
    /// Cada estado chama esta funcao em sua própria <c>_Ready()</c> e após isso inicializa dados especificos
    /// daquela unidade e estado. <br></br><br></br>&#10;
    /// </summary>
    public void InitializeUnit(){
        Unit = GetParent().GetParent<Unit>();
        ModeManager = GetNode<ModeManager>("/root/Game/ModeManager");
        SimulationMode = ModeManager.GetNode<ClawtopiaCs.Scripts.Systems.GameModes.SimulationMode>("SimulationMode");
        Unit.Navigation.VelocityComputed += VelocityComputed;
    }

    
    /// <summary>
    /// <param name="next">Para qual estado ir</param>
    /// Emite o sinal de <c>StateTransition</c> para trocar de um estado para o outro. 
    /// </summary>
    public void ChangeState(string next){
        EmitSignal("StateTransition", this, next);
    }
    
    
    /// <summary>
    /// <param name="safeVelocity">Velocidade pós calculo de avoidance.</param>
    /// Recebe a velocidade, multiplica pela velocidade de movimento da unidade e modifica a velocidade do CharacterBody, após isso chamando <c>MoveAndSlide</c>
    /// </summary>
    public void VelocityComputed(Vector2 safeVelocity) {
        Unit.Velocity = safeVelocity * Unit.Attributes.MoveSpeed;
        Unit.MoveAndSlide();
    }
    
    
    /// <summary>
    /// Recalcula as coordenadas baseado na posicao do aliado e da construçao selecionada.
    /// Pressupoe-se que a este ponto o clique direito foi efetuado em cima de uma construcao.
    /// </summary>
    /// <param name="allyPosition">A posicao global do aliado.</param>
    /// <param name="buildingPosition">A posicao global da construcao.</param>
    /// <returns> <c>Vector2</c> coordenadas globais.</returns>
    public Vector2 RecalculateCoords(Vector2 allyPosition, Vector2 buildingPosition){
        var buildMode = ModeManager.GetNode<BuildMode>("BuildMode");
        var x = buildingPosition.X - allyPosition.X > 0? -1 : 1;
        var y = buildingPosition.Y - allyPosition.Y > 0? 1 : -1;
        var newCoords = new Vector2(
            buildingPosition.X + (buildMode.TileSizeX * x),
            buildingPosition.Y + (buildMode.TileSizeY * y)
        );
        return newCoords;
    }
    
    
    /// <summary>
    /// Pega a próxima posição de navegação, calcula a distância e modifica a velocidade do navigationAgent.<br></br><br></br>
    /// Caso avoidance esteja ligado, ele aguarda pelo signal <c>VelocityComputed</c> para receber a nova velocidade após o calculo de avoidance.<br></br><br></br>
    /// Caso esteja desligado, chama a função de mover o aliado com a velocidade desconsiderando avoidance.
    /// </summary>
    public void Move(){
        var nextPathPos = Unit.Navigation.GetNextPathPosition();
        var newVelocity = Unit.GlobalPosition.DirectionTo(nextPathPos);
        if (Unit.Navigation.AvoidanceEnabled){
            Unit.Navigation.Velocity = newVelocity;
        }
        else{
            VelocityComputed(newVelocity);
        }
    }
}
