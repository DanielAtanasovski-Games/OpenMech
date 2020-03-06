using Godot;
using System;

public class MMech : KinematicBody2D
{
    private bool canMove;

    // Position
    public Vector2 MovementVector { get; set; }

    // New State
    private int recievedMovement;
    private int recievedTurn;
    private bool recievedFire;

    // Previous State
    private Vector2 lastPosition;
    private float lastRotation;
    private bool canFire = true;

    // Timers
    private Timer attackTimer;

    // Instances
    // TODO: Replace this with bullet associated with weapon / aka JSON file
    [Export]
    public PackedScene Bullet;

    // TODO: Replace this with json of stats
    private int ForwardSpeed = 100;
    private int RotationSpeed = 5;
    private float AttackSpeed = 1F;

    private const int MAX_BULLETS = 50;

    private Bullet[] bullets = new Bullet[MAX_BULLETS];
    private MLevel levelInstance;

    public override void _Ready()
    {
        attackTimer = new Timer();
        AddChild(attackTimer);
        attackTimer.Connect("timeout", this, nameof(AttackReset));
        levelInstance = (MLevel)GetParent().GetParent();

        lastPosition = GlobalPosition;
        lastRotation = Rotation;
    }



    [Remote]
    public void OnRecieveInput(int movement, int turn, bool fire)
    {
        recievedTurn = turn;
        recievedMovement = movement;    
        recievedFire = fire;
    }

    public override void _PhysicsProcess(float delta)
    {
        Move(delta);
        Turn(delta);
        Shoot();
    }

    public void Move(float delta)
    {
        // if (MovementVector == Vector2.Zero)
        //     animation.Playing = false;
        // else
        //     animation.Playing = true;
        // TODO: Check if going backwards
        MovementVector = new Vector2(ForwardSpeed * recievedMovement, 0).Rotated(Rotation);
        MovementVector = MovementVector.Normalized() * ForwardSpeed;

        MovementVector = MoveAndSlide(MovementVector);
        if (lastPosition != Position){
            SetPos(GlobalPosition);
        }
    }

    public void Shoot() {
        if (recievedFire) {
            if (canFire) {
                canFire = false;
                SpawnBullet();
                attackTimer.Start(AttackSpeed);
            }
        }
    }

    public void SpawnBullet() {
        Bullet bullet = (Bullet) Bullet.Instance();
        levelInstance.AddChild(bullet);
        int id = AddBullet(bullet);
        bullet.Start(GlobalPosition, GlobalRotation);
        bullet.Name = "bullet" + Name + id;
        string identify = Name + id;
        Godot.GD.Print(bullet.Name);
        bullet.SetCollisionMaskBit((int)CollisionLayer - 1, false);
        bullet.MechParent = this;
        Rpc("SpawnBullet", Position, Rotation, identify);
    }

    private int AddBullet(Bullet bullet) {
        // TODO: There has to be a better solution to having unique ids for 50 bullets
        for (int i = 0; i < MAX_BULLETS; i++)
        {
            if (bullets[i] == null){
                bullets[i] = bullet;
                return i;
            }
        }
        return -1;
    }

    public void RemoveBullet(Bullet bullet) {
        for (int i = 0; i < MAX_BULLETS; i++)
        {
            if (bullets[i] == null)
                continue;

            if (bullets[i].Name.Equals(bullet.Name)){
                bullets[i] = null;
                return;
            }
        }
    }

    public void DestroyBullets() {
        for (int i = 0; i < MAX_BULLETS; i++)
        {
            if (bullets[i] == null)
                continue;

            bullets[i].Destroy();
        }
    }

    private void AttackReset() {
        canFire = true;
    }

    public void SetPos(Vector2 pos) {
        GlobalPosition = pos;
        Rpc("SetPos", pos);
    }

    public void Turn(float delta)
    {
        Rotation += recievedTurn * RotationSpeed * delta;
        if (lastRotation != Rotation){
            SetRot(Rotation);
        }
    }

    public void SetRot(float rot) {
        Rotation = rot;
        Rpc("SetRot", rot);
    }

    public void Destroy() {
        Rpc("Destroy");
        DestroyBullets();
        QueueFree();
    }
}
