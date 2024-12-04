using Godot;
using System;

public partial class player : RigidBody2D
{
    public string state;
    [Export] public float walkingSpeed = 200f;

    public override void _Ready()
    {
        state = "free";
    }

    public override void _Process(double delta)
    {
        if (state == "free")
        {
            Visible = true;
            movement();
        }
        if (state == "driving")
        {
            Visible = false;
        }
    }

    private void movement()
    {
        Vector2 velocity = Vector2.Zero;

        // Bewegungseingaben abfragen (WASD oder Pfeiltasten)
        if (Input.IsActionPressed("ui_up"))
            velocity.Y -= 1;
        if (Input.IsActionPressed("ui_down"))
            velocity.Y += 1;
        if (Input.IsActionPressed("ui_left"))
            velocity.X -= 1;
        if (Input.IsActionPressed("ui_right"))
            velocity.X += 1;

        // Normale Richtung beibehalten, um diagonale Bewegung nicht schneller zu machen
        velocity = velocity.Normalized() * walkingSpeed;

        // Bewegung anwenden
        LinearVelocity = velocity;
    }
}
