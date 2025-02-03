using UnityEngine;

// Scriptable Object that holds all settings and parameters for boid behavior
[CreateAssetMenu(fileName = "BoidSettings", menuName = "ScriptableObjects/BoidSettings", order = 1)]
public class BoidSettings : ScriptableObject
{
    // Core settings
    public int boidCount;
    public GameObject boidPrefab;

    // Flocking behavior weights (0-1 range)
    [Range(0.01f, 1f)] public float separationStrength;  // How strongly boids avoid each other
    [Range(0.01f, 1f)] public float alignmentStrength;   // How strongly boids match velocities
    [Range(0.01f, 1f)] public float cohesionStrength;    // How strongly boids group together

    // Physics parameters
    [Range(0.1f, 5f)] public float mass;                 // Mass affects force calculations
    [Range(0.01f, 3f)] public float speed;               // Base movement speed
    [Range(0.01f, 5f)] public float maxAccel;            // Maximum acceleration force
    [Range(0.01f, 2f)] public float perceptionRange;     // How far boids can see neighbors

    // Visual settings
    public bool boundsOn = true;                         // Show boundary sphere
    public bool drawDebugLines = false;                  // Show debug force vectors
    private GameObject bounds;

    // Default values
    private const int initBoidCount = 250;
    private const int maxBoidCount = 500;
    private const float initSeparStr = 0.65f;
    private const float initAlignStr = 0.55f;
    private const float initCohesStr = 0.4f;
    private const float initMass = 1;
    private const float initSpeed = 2.5f;
    private const float initMaxForce = 0.4f;
    private const float initPerceptRange = 1.0f;

    // Methods to safely change parameter values
    public void ChangeCount(float count) => boidCount = Mathf.Clamp((int)count, 1, maxBoidCount);
    public void ChangeSeparation(float separation) => separationStrength = Mathf.Clamp(separation, 0, 1);
    public void ChangeAlignment(float alignment) => alignmentStrength = Mathf.Clamp(alignment, 0, 1);
    public void ChangeCohesion(float cohesion) => cohesionStrength = Mathf.Clamp(cohesion, 0, 1);
    public void ChangeSpeed(float spd) => speed = Mathf.Clamp(spd, 0, 8);
    public void ChangeMaxForce(float mxForce) => maxAccel = Mathf.Clamp(mxForce, 0, 1);
    public void ChangePerception(float perception) => perceptionRange = Mathf.Clamp(perception, 0, 2);

    // Reset all values to defaults
    public void ResetSettings()
    {
        boidCount = initBoidCount;
        separationStrength = initSeparStr;
        alignmentStrength = initAlignStr;
        cohesionStrength = initCohesStr;
        mass = initMass;
        speed = initSpeed;
        maxAccel = initMaxForce;
        perceptionRange = initPerceptRange;
        boundsOn = true;
        drawDebugLines = false;
        UIManager.Instance.RefreshUI();
    }

    public void Reset() => ResetSettings();

    // Toggle debug visualization settings
    public void ToggleDebugLines() => drawDebugLines = !drawDebugLines;

    public void ToggleBounds()
    {
        boundsOn = !boundsOn;
        if (!bounds)
            bounds = GameObject.Find("Bounds");
        bounds.SetActive(boundsOn);
    }
}