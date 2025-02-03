using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utilities;
using UnityEngine.UI;
using TMPro;

// Manages all UI elements and interactions
public class UIManager : Singleton<UIManager>
{
    // References to settings and canvases
    [SerializeField] private BoidSettings boidsSettings;
    [SerializeField] private Canvas simulationCanvas;
    [SerializeField] private Canvas optionsMenuCanvas;

    // HUD UI elements
    [SerializeField] private Button debugLinesButton;
    [SerializeField] private Slider sepSlider;
    [SerializeField] private Text sepValue;
    [SerializeField] private Slider alignSlider;
    [SerializeField] private Text alignValue;
    [SerializeField] private Slider cohSlider;
    [SerializeField] private Text cohValue;
    [SerializeField] private Slider spdSlider;
    [SerializeField] private Text spdValue;
    [SerializeField] private Slider forceSlider;
    [SerializeField] private Text forceValue;
    [SerializeField] private Slider perceptSlider;
    [SerializeField] private Text perceptValue;

    // Options menu UI elements
    [SerializeField] private TextMeshProUGUI boidCount;
    [SerializeField] private Slider countSlider;
    [SerializeField] private Text countSliderValue;
    [SerializeField] private TMP_Dropdown methodDropdown;

    // Internal tracking
    private List<Canvas> canvasList;
    private Canvas currentCanvas;
    private bool isOptionsUIVisible = false;

    // Initialize UI elements and set up listeners
    private void Start()
    {
        // Set up canvas list
        if (canvasList == null)
            canvasList = new List<Canvas>();
        if (simulationCanvas) canvasList.Add(simulationCanvas);
        if (optionsMenuCanvas) canvasList.Add(optionsMenuCanvas);
        
        // Show simulation canvas by default in boids scene
        if (SceneManager.GetActiveScene().name == "Boids")
            ActivateCanvas(simulationCanvas);

        // Add value change listeners to update text displays
        SetupSliderListeners();
    }

    // Refresh UI on enable
    private void OnEnable()
    {
        RefreshUI();
        
        // Only show debug button in editor
        #if UNITY_EDITOR
            debugLinesButton.gameObject.SetActive(true);
        #else
            debugLinesButton.gameObject.SetActive(false);
        #endif
    }

    // Set up all slider value change listeners
    private void SetupSliderListeners()
    {
        // HUD sliders
        sepSlider.onValueChanged.AddListener((v) => { sepValue.text = v.ToString("0.00"); });
        alignSlider.onValueChanged.AddListener((v) => { alignValue.text = v.ToString("0.00"); });
        cohSlider.onValueChanged.AddListener((v) => { cohValue.text = v.ToString("0.00"); });
        spdSlider.onValueChanged.AddListener((v) => { spdValue.text = v.ToString("0.00"); });
        forceSlider.onValueChanged.AddListener((v) => { forceValue.text = v.ToString("0.00"); });
        perceptSlider.onValueChanged.AddListener((v) => { perceptValue.text = v.ToString("0.00"); });
        
        // Options menu slider
        countSlider.onValueChanged.AddListener((v) => { countSliderValue.text = v.ToString(); });
    }

    // Show simulation canvas
    public void LoadGame()
    {
        ActivateCanvas(simulationCanvas);
    }

    // Helper to show UI after delay
    private IEnumerator ShowUIWithDelay(object[] parameters)
    {
        yield return new WaitForSeconds((float)parameters[1]);
        ActivateCanvas((Canvas)parameters[0]);
        yield return null;
    }

    // Toggle options menu visibility
    public void ToggleOptionsMenu()
    {
        RefreshUI();
        isOptionsUIVisible = !isOptionsUIVisible;
        optionsMenuCanvas.gameObject.SetActive(isOptionsUIVisible);
        currentCanvas.gameObject.SetActive(!isOptionsUIVisible);
    }

    // Hide all canvases
    private void DisableAllCanvases()
    {
        foreach (Canvas canvas in canvasList)
        {
            canvas.gameObject.SetActive(false);
        }
    }

    // Show specified canvas
    private void ActivateCanvas(Canvas canvas)
    {
        DisableAllCanvases();
        currentCanvas = canvas;
        currentCanvas.gameObject.SetActive(true);
    }

    // Reload current scene
    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    // Exit application
    public void ExitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    // Update all UI elements to match current settings
    public void RefreshUI()
    {
        // Update HUD values
        sepSlider.value = boidsSettings.separationStrength;
        sepValue.text = boidsSettings.separationStrength.ToString("0.00");
        alignSlider.value = boidsSettings.alignmentStrength;
        alignValue.text = boidsSettings.alignmentStrength.ToString("0.00");
        cohSlider.value = boidsSettings.cohesionStrength;
        cohValue.text = boidsSettings.cohesionStrength.ToString("0.00");
        spdSlider.value = boidsSettings.speed;
        spdValue.text = boidsSettings.speed.ToString("0.00");
        forceSlider.value = boidsSettings.maxAccel;
        forceValue.text = boidsSettings.maxAccel.ToString("0.00");
        perceptSlider.value = boidsSettings.perceptionRange;
        perceptValue.text = boidsSettings.perceptionRange.ToString("0.00");

        // Update options menu values
        boidCount.text = boidsSettings.boidCount.ToString();
        countSlider.value = boidsSettings.boidCount;
        countSliderValue.text = boidsSettings.boidCount.ToString("0.00");
    }
}