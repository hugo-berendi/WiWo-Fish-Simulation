using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    // Static list containing all boids in the scene
    public static List<Boid> boidList;

    public BoidSettings boidSettings;

    // Boundary variables
    private Vector3 boundaryCenter;
    private float boundaryRadius = 10;
    private bool turningAround = false;
    private Quaternion targetRotation;

    // Movement variables
    private Vector3 velocity;
    private Vector3 acceleration;
    
    // Flocking force variables
    private Vector3 separationForce;
    private Vector3 alignmentForce; 
    private Vector3 cohesionForce;

    // Cached variables for neighbor calculations
    private Dictionary<Boid, (Vector3, Vector3, Vector3, float)> neighbors;
    private Vector3 vectorBetween;
    private Vector3 velocityOther;
    private Vector3 targetPosition;
    private float sqrPerceptionRange;
    private float sqrMagnitudeTemp;

    private void Awake()
    {
        // Initialize collections if needed
        if (boidList == null)
            boidList = new List<Boid>();
        boidList.Add(this);
        
        if (neighbors == null)
            neighbors = new Dictionary<Boid, (Vector3, Vector3, Vector3, float)>();
    }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        // Set random initial direction and velocity
        transform.forward = Random.insideUnitSphere.normalized;
        velocity = transform.forward * boidSettings.speed;
        acceleration = Vector3.zero;
    }

    public void SetBoundarySphere(Vector3 center, float radius)
    {
        boundaryCenter = center;
        boundaryRadius = radius;
    }

    private void Update()
    {
        Flocking();     // Calculate flocking forces
        TurnAtBounds(); // Keep boids within boundaries
        Move();         // Apply forces and move
        ResetForces();  // Clear forces for next frame
    }

    private void ApplyForce(Vector3 force)
    {
        acceleration += (force / boidSettings.mass);
    }

    private void ResetForces()
    {
        acceleration = separationForce = alignmentForce = cohesionForce = Vector3.zero;
    }

    private void Move()
    {
        // Limit acceleration and update velocity
        acceleration = Vector3.ClampMagnitude(acceleration, boidSettings.maxAccel);
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, boidSettings.speed);

        // Ensure minimum velocity
        if (velocity.sqrMagnitude <= .1f)
            velocity = transform.forward * boidSettings.speed;

        // Update position and rotation if moving
        if (velocity != Vector3.zero)
        {
            transform.position += velocity * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(velocity);
        }
    }

    private void TurnAtBounds()
    {
        if (!boidSettings.boundsOn)
            return;

        // Check if boid is outside boundary
        if ((transform.position - boundaryCenter).sqrMagnitude > (boundaryRadius * boundaryRadius))
        {
            if (!turningAround)
            {
                // Set target position on opposite side of boundary
                targetPosition = boundaryCenter + (boundaryCenter - transform.position);
                turningAround = true;
            }

            // Smoothly turn towards target
            targetRotation = Quaternion.LookRotation(targetPosition);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * boidSettings.speed);
            velocity = Vector3.Slerp(velocity, targetPosition - transform.position, Time.deltaTime * boidSettings.speed);
        }
        // Stop turning when aligned with target
        else if (Quaternion.Angle(transform.rotation, targetRotation) <= .01f)
            turningAround = false;
    }

    private void Flocking()
    {
        // Find nearby boids once and cache results
        FindNeighbors();

        // Calculate flocking forces
        Separation();
        Alignment();
        Cohesion();

        // Apply all forces
        ApplyForce(separationForce);
        ApplyForce(alignmentForce);
        ApplyForce(cohesionForce);
    }

    private void FindNeighbors()
    {
        neighbors.Clear();
        sqrPerceptionRange = boidSettings.perceptionRange * boidSettings.perceptionRange;

        // Find all boids within perception range
        foreach (Boid other in boidList)
        {
            if (other == this) continue;

            velocityOther = other.velocity;
            vectorBetween = other.transform.position - transform.position;
            sqrMagnitudeTemp = vectorBetween.sqrMagnitude;

            if (sqrMagnitudeTemp < sqrPerceptionRange)
            {
                neighbors.Add(other, (other.transform.position, velocityOther, vectorBetween, sqrMagnitudeTemp));
            }
        }
    }

    private void Separation()
    {
        if (boidSettings.separationStrength <= 0 || neighbors.Count == 0)
            return;

        // Move away from nearby boids
        foreach (var item in neighbors)
        {
            if (item.Value.Item4 < boidSettings.perceptionRange * boidSettings.separationStrength)
            {
                separationForce -= item.Value.Item3;
            }
        }

        // Apply strength and limit force
        separationForce *= boidSettings.separationStrength;
        separationForce = Vector3.ClampMagnitude(separationForce, boidSettings.maxAccel / 2);

        if (boidSettings.drawDebugLines)
        {
            Debug.DrawLine(transform.position, transform.position + separationForce, Color.red);
        }
    }

    private void Alignment()
    {
        if (boidSettings.alignmentStrength <= 0 || neighbors.Count == 0)
            return;

        // Match velocity with nearby boids
        foreach (var item in neighbors)
        {
            alignmentForce += item.Value.Item2;
        }

        // Calculate average and apply strength
        alignmentForce /= neighbors.Count;
        alignmentForce *= boidSettings.alignmentStrength;
        alignmentForce = Vector3.ClampMagnitude(alignmentForce, boidSettings.maxAccel);

        if (boidSettings.drawDebugLines)
        {
            Debug.DrawLine(transform.position, transform.position + alignmentForce, Color.green);
        }
    }

    private void Cohesion()
    {
        if (boidSettings.cohesionStrength <= 0 || neighbors.Count == 0)
            return;

        // Move toward center of nearby boids
        foreach (var item in neighbors)
        {
            cohesionForce += item.Value.Item1;
        }

        // Calculate center point and direction
        cohesionForce /= neighbors.Count;
        cohesionForce -= transform.position;
        
        // Apply strength and limit force
        cohesionForce *= boidSettings.cohesionStrength;
        cohesionForce = Vector3.ClampMagnitude(cohesionForce, boidSettings.maxAccel);

        if (boidSettings.drawDebugLines)
        {
            Debug.DrawLine(transform.position, transform.position + cohesionForce, Color.blue);
        }
    }
}
