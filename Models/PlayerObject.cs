using System.Formats.Asn1;
using Silk.NET.Maths;
using TheAdventure;

namespace TheAdventure.Models;

public class PlayerObject : RenderableGameObject
{
    public enum PlayerStateDirection{
        None = 0,
        Down,
        Up,
        Left,
        Right,
    }
    public enum PlayerState{
        None = 0,
        Idle,
        Move,
        Attack,
        GameOver,
        Transform,
        Kamehameha
    }

    public double KamehamehaChargeTime;
    private int _pixelsPerSecond = 192;

    public PlayerStateDirection GetCurrentDirection()
    {
        return State.Direction;
    }

    public bool IsAttacking()
    {
        return State.State == PlayerState.Attack;
    }

    public bool isKamehameha()
    {
        return State.State == PlayerState.Kamehameha;
    }    

  public bool isKamehamehaReady()
    {
        if (KamehamehaChargeTime >= 60)
        {
            KamehamehaChargeTime = 0;
            SetState(PlayerState.Idle, State.Direction);
            return true;
        }
        else
            return false;
    }


    public (PlayerState State, PlayerStateDirection Direction) State{ get; private set; }

    public PlayerObject(SpriteSheet spriteSheet, int x, int y) : base(spriteSheet, (x, y))
    {
        SetState(PlayerState.Idle, PlayerStateDirection.Down);
    }

    public void SetState(PlayerState state, PlayerStateDirection direction)
    {      
        if(State.State == PlayerState.GameOver) return;
        if (State.State == state && State.Direction == direction){
            return;
        }
        else if(state == PlayerState.None && direction == PlayerStateDirection.None){
            SpriteSheet.ActivateAnimation(null);
        }
        else if(state == PlayerState.GameOver){
            SpriteSheet.ActivateAnimation(Enum.GetName(state));
        }
        else if(state == PlayerState.Attack)
        {
            var random = new Random().Next(1, 4);
            var animationName = Enum.GetName<PlayerState>(state) + random + Enum.GetName<PlayerStateDirection>(direction);
            SpriteSheet.ActivateAnimation(animationName);
        }
        else if(state == PlayerState.Transform)
        {
            if (direction == PlayerStateDirection.Up)
            {
                var animationName = Enum.GetName<PlayerState>(state) + "SuperSaiyan";
                SpriteSheet.ActivateAnimation(animationName);
            }
            else
            {
                var animationName = Enum.GetName<PlayerState>(state) + "Kaioken";
                SpriteSheet.ActivateAnimation(animationName);
            }
        }
        else if(state == PlayerState.Kamehameha)
        {
            var animationName = Enum.GetName<PlayerState>(state) + Enum.GetName<PlayerStateDirection>(direction);
            SpriteSheet.ActivateAnimation(animationName);
        }
        else if(state == PlayerState.Move)
        {
            var animationName = Enum.GetName<PlayerState>(state) + Enum.GetName<PlayerStateDirection>(direction);
            SpriteSheet.ActivateAnimation(animationName);
        }
        else if(state == PlayerState.Idle)
        {
            var animationName = Enum.GetName<PlayerState>(state) + Enum.GetName<PlayerStateDirection>(direction);
            SpriteSheet.ActivateAnimation(animationName);
        }
        else{
            var animationName = Enum.GetName<PlayerState>(state) + Enum.GetName<PlayerStateDirection>(direction);
            SpriteSheet.ActivateAnimation(animationName);
        }
        State = (state, direction);
    }

    public void GameOver(){
        SetState(PlayerState.GameOver, PlayerStateDirection.None);
    }

    // Transform the player depending on the direction
    public void Transform(PlayerStateDirection direction)
    {
        SetState(PlayerState.Transform, direction);
    }


    public void Kamehameha(PlayerStateDirection direction)
    {
        KamehamehaChargeTime = 0;
        SetState(PlayerState.Kamehameha,direction);
    }

    public void kamehamehaCharge()
    {
        Console.WriteLine(KamehamehaChargeTime);
        KamehamehaChargeTime += 1;
    }


    public void Attack(bool up, bool down, bool left, bool right)
    {
        if(State.State == PlayerState.GameOver) return;
        var direction = State.Direction;
        if(up){
            direction = PlayerStateDirection.Up;
        }
        else if (down)
        {
            direction = PlayerStateDirection.Down;
        }
        else if (right)
        {
            direction = PlayerStateDirection.Right;
        }
        else if (left){
            direction = PlayerStateDirection.Left;
        }
        SetState(PlayerState.Attack, direction);
    }

    public void UpdatePlayerPosition(double up, double down, double left, double right, int width, int height,
        double time)
    {
        if(State.State == PlayerState.GameOver) return;
        if (up <= double.Epsilon &&
            down <= double.Epsilon &&
            left <= double.Epsilon &&
            right <= double.Epsilon &&
            State.State == PlayerState.Idle){
            return;
        }

        var pixelsToMove = time * _pixelsPerSecond;

        var x = Position.X + (int)(right * pixelsToMove);
        x -= (int)(left * pixelsToMove);

        var y = Position.Y - (int)(up * pixelsToMove);
        y += (int)(down * pixelsToMove);

        if (x < 10)
        {
            x = 10;
        }

        if (y < 24)
        {
            y = 24;
        }

        if (x > width - 10)
        {
            x = width - 10;
        }

        if (y > height - 6)
        {
            y = height - 6;
        }



        if (y < Position.Y){
            SetState(PlayerState.Move, PlayerStateDirection.Up);
        }
        if (y > Position.Y ){
            SetState(PlayerState.Move, PlayerStateDirection.Down);
        }
        if (x > Position.X ){
            SetState(PlayerState.Move, PlayerStateDirection.Right);
        }
        if (x < Position.X){
            SetState(PlayerState.Move, PlayerStateDirection.Left);
        }
        if (x == Position.X &&
            y == Position.Y && !isKamehameha()){
            SetState(PlayerState.Idle, State.Direction);
        }
        if (isKamehameha())
        {
            SetState(PlayerState.Kamehameha, State.Direction);
        }
        Position = (x, y);
    }
}