using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Handles the spawning and management of boids in the scene.
/// Boids are autonomous agents that simulate flocking behavior like fish or birds.
/// </summary>
public class BoidSpawner : MonoBehaviour
{
    // Settings for boid behavior (speed, flocking parameters, etc)
    [Header("Boid Settings")]
    [Tooltip("Contains all parameters that control boid behavior")]
    [SerializeField] private BoidSettings boidSettings;

    // Where boids will spawn from
    [Header("Spawn Settings")] 
    [Tooltip("Where boids will spawn. If not set, uses this object's position")]
    [SerializeField] private Transform spawnLocation;
    
    // Camera references for switching views
    [Header("Camera Settings")]
    [Tooltip("Main camera that shows overall view")]
    [SerializeField] private CinemachineVirtualCamera mainVCam;
    
    [Tooltip("Camera that follows individual boids")]
    [SerializeField] private CinemachineVirtualCamera fishVCam;

    // Spawn area parameters
    private float spawnAreaRadius = 10;
    private Vector3 boidLocation;

    // Constants for spawn area calculations
    private const float SPAWN_AREA_MARGIN = 0.9f;
    private const float DEFAULT_BOUNDARY_RADIUS = 10f;

    // Initialize spawn location and area
    private void Awake()
    {
        boidSettings.boundsOn = true;

        if (!spawnLocation)
            spawnLocation = this.transform;
        else
            spawnAreaRadius = spawnLocation.localScale.x / 2;
    }

    // Reset settings and spawn initial boids
    private void Start()
    {
        boidSettings.ResetSettings();
        SpawnBoids();

        if (mainVCam)
            WatchMainCamera();
        Debug.Log("Total boids: " + Boid.boidList.Count);
    }

    // Create all boids at start of simulation
    public void SpawnBoids()
    {
        // Validate required components exist
        if (boidSettings == null)
        {
            Debug.LogError("No BoidSettings assigned to BoidSpawner!");
            return;
        }

        if (boidSettings.boidPrefab == null)
        {
            Debug.LogError("No boid prefab assigned in BoidSettings!");
            return;
        }

        // Initialize or clear the list of all boids
        if (Boid.boidList == null)
            Boid.boidList = new List<Boid>();
        else
            Boid.boidList.Clear();

        // Create each boid at a random position within spawn area
        Boid newBoid;
        for (int i = 0; i < boidSettings.boidCount; i++)
        {
            boidLocation =
                Random.insideUnitSphere.normalized * Random.Range(0, spawnAreaRadius * 0.9f);
            newBoid = Instantiate(
                    boidSettings.boidPrefab,
                    boidLocation,
                    Quaternion.identity,
                    this.transform
                )
                .GetComponent<Boid>();
            ConfigureBoid(newBoid);
        }
    }

    // Destroy all boids and create new ones
    public void RespawnBoids()
    {
        KillBoids();
        SpawnBoids();
        UIManager.Instance.RefreshUI();
    }

    // Remove all boids from the scene
    public void KillBoids()
    {
        foreach (Boid boid in Boid.boidList)
        {
            Destroy(boid.gameObject);
        }
        Boid.boidList.Clear();
    }

    /// <summary>
    /// Switches the active camera between the main overview camera and a fish-following camera
    /// </summary>
    public void ToggleCamera()
    {
        if (mainVCam.gameObject.activeInHierarchy)
            WatchFishCamera();
        else
            WatchMainCamera();
    }

    // Switch to main overview camera
    public void WatchMainCamera()
    {
        mainVCam.gameObject.SetActive(true);
        fishVCam.gameObject.SetActive(false);
    }

    // Switch to camera that follows a random boid
    public void WatchFishCamera()
    {
        if (fishVCam && Boid.boidList != null && Boid.boidList.Count > 0)
        {
            mainVCam.gameObject.SetActive(false);
            fishVCam.gameObject.SetActive(true);
            int randomBoid = Random.Range(0, boidSettings.boidCount);
            fishVCam.Follow = fishVCam.LookAt = Boid.boidList[randomBoid].transform;
        }
    }

    // Create a single boid at specified position
    private void SpawnSingleBoid(Vector3 position)
    {
        Boid newBoid = Instantiate(
            boidSettings.boidPrefab,
            position,
            Quaternion.identity,
            this.transform
        ).GetComponent<Boid>();
        
        ConfigureBoid(newBoid);
    }

    // Set up a boid with required settings and boundaries
    private void ConfigureBoid(Boid boid)
    {
        boid.boidSettings = boidSettings;
        boid.SetBoundarySphere(spawnLocation.position, spawnAreaRadius);
    }
}
