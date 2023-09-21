using Humper;
using Humper.Responses;
using PixelFlut.Core;
using System.Numerics;
namespace StickFigureGame;

public class StickFigureBase
{
    public enum FacingDirection { Left, Right };

    public Animator PlayerAnimator { get; private set; }
    public SpriteRenderer PlayerSprite { get; private set; }

    public float MovementSpeed { get; set; } = 10;

    public Vector2 Velocity;

    public FacingDirection Facing { get; set; } = FacingDirection.Right;

    public bool IsGrounded = true;

    public Vector2 Position => box == null ? Vector2.Zero : new Vector2(box.X, box.Y);

    public Vector2 Size { get; set; } = new Vector2(0.65f, 1.25f);

    private StickFigureWorld world;
    public IStickFigureInput Input = new StickFigureInput();
    public enum Player1Or2 { Player1, Player2 }

    public Player1Or2 Player;

    public IBox box { get; private set; }

    public StickFigureBase(StickFigureWorld world, Vector2 spawnLocation)
    {
        this.world = world;
        Input = Player == Player1Or2.Player1 ?
            new StickFigureInput() :
            new StickFigureInput2();

        box = world.BoxWorld.Create(
            spawnLocation.X,
            spawnLocation.Y,
            Size.X,
            Size.Y);
        PlayerAnimator = GetComponentInChildren<Animator>();
        PlayerSprite = GetComponentInChildren<SpriteRenderer>();
        PlayerAnimator.Play("idle");
    }


    public void Teleport(Vector2 to)
    {
        Velocity = Vector2.UnitY;
        box.Move(to.X, to.Y, c => CollisionResponses.None);
    }

    // Update is called once per frame
    void Loop(GameTime time)
    {
        this.Move(time);
    }

    private void Move(GameTime time)
    {
        Vector2 toPosition = this.Position + this.Velocity * (float)time.DeltaTime.TotalSeconds;

        Vector2 prevPositin = new Vector2(box.X, box.Y);

        if (Velocity.Y > 0)
        {
            IsGrounded = false;
        }

        bool hitGround = false;
        box.Move(toPosition.X, toPosition.Y, c =>
        {
            if (c.Hit.Normal.X != 0)
            {
                this.Velocity = new Vector2(0, this.Velocity.Y);
            }
            if (c.Hit.Normal.Y != 0)
            {
                this.Velocity = new Vector2(this.Velocity.X, 0);
            }

            if (c.Hit.Normal.Y > 0)
            {
                hitGround = true;
            }
            return CollisionResponses.Slide;
        });

        IsGrounded = hitGround;
    }
}
