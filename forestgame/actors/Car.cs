using Godot;
using System;

public partial class Car : RigidBody2D
{
    public string state = "idle";

    [Export]
    public Node2D exitPoint;

    public bool entityIsStandingNextToDoor = false;
    private player entityNextToDoor;

    private player driver;

    [Export] public double maxSpeed = 200;
    [Export] public double acceleration = 200;
    [Export] public double deceleration = 50;
    [Export] public double friction = 1.5;
    [Export] public double rotationSpeed = 2;

    private double velocity = 0;

    public override void _Process(double deltaTime)
    {
        if(state == "driving")
        {
             carMovement(deltaTime);
            driver.Position = Position;

            if (Input.IsKeyPressed(Key.R))
            {
                driver.Position = exitPoint.GlobalPosition;
                state = "idle";
                driver.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
                driver.state = "free";

                driver = null;
            }
        }

        if (entityIsStandingNextToDoor)
        {
            if (Input.IsKeyPressed(Key.E))
            {
                if (entityNextToDoor != null)
                {
                    entityNextToDoor.state = "driving";
                    state = "driving";
                    driver = entityNextToDoor;
                    driver.GetNode<CollisionShape2D>("CollisionShape2D").Disabled = true;
                }
            }
        }
    }

    private void carMovement(double deltaTime)
    {
        
        // Calculate the direction based on the car's rotation
        Vector2 forwardDirection = new Vector2(0, -1).Rotated((float)Rotation);

        // Accelerate or decelerate based on input
        if (Input.IsActionPressed("drive_forward"))
        {
            velocity += acceleration * deltaTime;
        }
        else if (Input.IsActionPressed("drive_backwards"))
        {
            velocity -= acceleration * deltaTime;
        }
        else
        {
            // Apply deceleration gradually if no input is given
            if (velocity > 0)
                velocity -= deceleration * deltaTime;
            else if (velocity < 0)
                velocity += deceleration * deltaTime;

            // Apply friction gradually when velocity is small (i.e. car should slow down to a stop)
            if (Math.Abs(velocity) < 1) // When the car is close to stopping
            {
                velocity = 0;
            }
            else
            {
                // Apply friction over time to gradually slow down
                velocity *= (1 - friction * deltaTime); // Gradual reduction based on friction
            }
        }

        // Clamp velocity to max speed (ensure velocity stays within bounds)
        velocity = Mathf.Clamp((float)velocity, -maxSpeed, maxSpeed);

        // Apply the velocity in the direction the car is facing
        Vector2 velocityVector = forwardDirection * (float)velocity;

        // Move the car (use the velocity to move via physics)
        LinearVelocity = velocityVector;

        // Calculate rotation (angular velocity)
        float targetRotation = 0f;

        // Only rotate when moving
        if (Math.Abs(velocity) > 1) // Only rotate if moving
        {
            if (Input.IsActionPressed("steer_left"))
            {
                // Apply rotation speed based on velocity and direction
                targetRotation = -(float)rotationSpeed * Math.Clamp((float)velocity, -100, 100) * (float)deltaTime;
            }

            if (Input.IsActionPressed("steer_right"))
            {
                // Apply rotation speed based on velocity and direction
                targetRotation = (float)rotationSpeed * Math.Clamp((float)velocity, -100, 100) * (float)deltaTime;
            }
        }

        // Set the angular velocity (rotation speed)
        AngularVelocity = targetRotation;
    }


    private void entityIsNextToDoor(Node2D node)
    {
        if(node is player)
        {
            entityIsStandingNextToDoor = true;
            entityNextToDoor = node as player;
        }

    }
    private void entityExitedDoorArea(Node2D node)
    {
        if (node is player)
        {
            entityIsStandingNextToDoor = false;
            entityNextToDoor = null;
        }
    }


    private void onCrash(Node2D node)
    {
        velocity = 0f;
    }
}