using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BoidSpawner : MonoBehaviour
{
    [SerializeField]
    private BoidSettings boidSettings;

    [SerializeField]
    private BoidManager boidManager;

    [SerializeField]
    private Transform spawnLocation;

    [SerializeField]
    private CinemachineVirtualCamera mainVCam;

    [SerializeField]
    private CinemachineVirtualCamera fishVCam;
    private float boundaryRadius = 10;
    private Vector3 boidLocation;

    private void Awake()
    {
        boidSettings.boundsOn = true;

        if (!spawnLocation)
            spawnLocation = this.transform;
        else
            boundaryRadius = spawnLocation.localScale.x / 2;
    }

    private void Start()
    {
        boidSettings.ResetSettings();
        SpawnBoids();

        if (mainVCam)
            WatchMainCamera();
        Debug.Log("Total boids: " + Boid.boidList.Count);
    }

    public void SpawnBoids()
    {
        // Create or clear BoidList
        if (Boid.boidList == null)
            Boid.boidList = new List<Boid>();
        else
            Boid.boidList.Clear();
        // Spawn Boids
        Boid newBoid;
        for (int i = 0; i < boidSettings.boidCount; i++)
        {
            boidLocation =
                Random.insideUnitSphere.normalized * Random.Range(0, boundaryRadius * 0.9f);
            newBoid = Instantiate(
                    boidSettings.boidPrefab,
                    boidLocation,
                    Quaternion.identity,
                    this.transform
                )
                .GetComponent<Boid>();
            newBoid.boidSettings = boidSettings;
            newBoid.SetBoundarySphere(spawnLocation.position, boundaryRadius);
        }
    }

    public void RespawnBoids()
    {
        KillBoids();
        SpawnBoids();
        UIManager.Instance.RefreshUI();
    }

    public void KillBoids()
    {
        foreach (Boid boid in Boid.boidList)
        {
            Destroy(boid.gameObject);
        }
        Boid.boidList.Clear();
    }

    public void ToggleCamera()
    {
        if (mainVCam.gameObject.activeInHierarchy)
            WatchFishCamera();
        else
            WatchMainCamera();
    }

    public void WatchMainCamera()
    {
        mainVCam.gameObject.SetActive(true);
        fishVCam.gameObject.SetActive(false);
    }

    public void WatchFishCamera()
    {
        if (fishVCam)
        {
            mainVCam.gameObject.SetActive(false);
            fishVCam.gameObject.SetActive(true);
            int randomBoid = Random.Range(0, boidSettings.boidCount);
            fishVCam.Follow = fishVCam.LookAt = Boid.boidList[randomBoid].transform;
        }
    }
}

