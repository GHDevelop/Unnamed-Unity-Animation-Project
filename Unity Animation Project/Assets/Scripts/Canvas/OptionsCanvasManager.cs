using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsCanvasManager : MonoBehaviour
{
    public static OptionsCanvasManager settings;
    [HideInInspector] public Canvas myCanvas;

    [Header("PlayerPrefs value names")]
    [SerializeField] private string masterVolume = "Master Volume";
    [SerializeField] private string musicVolume = "Music Volume";
    [SerializeField] private string soundVolume = "Sound Volume";
    [SerializeField] private static string resolution = "Resolution";
    [SerializeField] private static string quality = "Quality";
    [SerializeField] private static string fullscreen = "Fullscreen";

    [Header("Menu Inputs")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundVolumeSlider;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] public ButtonFunctions back;
    [SerializeField] private Button apply;

    [Header("Audio Manager")]
    [SerializeField] private AudioMixer audioManager;

    [Header("Audio Manager Variable Names")]
    [SerializeField] private string masterVolumeAM = "MasterVolume";
    [SerializeField] private string musicVolumeAM = "MusicVolume";
    [SerializeField] private string soundVolumeAM = "SoundVolume";

    [Header("Formats")]
    [SerializeField] private string resolutionFormat = "{0} x {1}: {2}:{3}";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void SetResolutionAndQualityOnProgramStart()
    {
        int resolution = PlayerPrefs.GetInt(OptionsCanvasManager.resolution);
        bool fullscreen = PlayerPrefs.GetInt(OptionsCanvasManager.fullscreen, 0) > 0 ? true : false;
        Screen.SetResolution(Screen.resolutions[resolution].width, Screen.resolutions[resolution].height, fullscreen);

        int quality = PlayerPrefs.GetInt(OptionsCanvasManager.quality, QualitySettings.names.Length - 1);
        QualitySettings.SetQualityLevel(quality);
    }

    private void Awake()
    {
        if (settings == null)
        {
            settings = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        myCanvas = GetComponent<Canvas>();

        PopulateResolutionDropdown();
        PopulateQualityDropdown();

        masterVolumeSlider.onValueChanged.AddListener(delegate { EnableApply(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { EnableApply(); });
        soundVolumeSlider.onValueChanged.AddListener(delegate { EnableApply(); });
        resolutionDropdown.onValueChanged.AddListener(delegate { EnableApply(); });
        qualityDropdown.onValueChanged.AddListener(delegate { EnableApply(); });
        fullscreenToggle.onValueChanged.AddListener(delegate { EnableApply(); });
    }

    //Does not work in Awake, so running in Start instead
    private void Start()
    {
        ApplyAudioLevels(PlayerPrefs.GetFloat(this.masterVolume), PlayerPrefs.GetFloat(this.musicVolume), PlayerPrefs.GetFloat(this.soundVolumeAM));
    }

    private void OnEnable()
    {
        masterVolumeSlider.value = PlayerPrefs.GetFloat(this.masterVolume, masterVolumeSlider.maxValue);
        musicVolumeSlider.value = PlayerPrefs.GetFloat(this.musicVolume, musicVolumeSlider.maxValue);
        soundVolumeSlider.value = PlayerPrefs.GetFloat(this.soundVolume, soundVolumeSlider.maxValue);

        for (int index = 0; index < Screen.resolutions.Length; index++)
        {
            if (Screen.currentResolution.width == Screen.resolutions[index].width
                && Screen.currentResolution.height == Screen.resolutions[index].height)
            {
                resolutionDropdown.value = index;
            }
        }

        qualityDropdown.value = QualitySettings.GetQualityLevel();
        fullscreenToggle.isOn = Screen.fullScreen;
        apply.interactable = false;
    }

    private void PopulateQualityDropdown()
    {
        if (qualityDropdown)
        {
            qualityDropdown.ClearOptions();

            List<string> qualityLevels = new List<string>();
            foreach (string name in QualitySettings.names)
            {
                qualityLevels.Add(name);
            }

            qualityDropdown.AddOptions(qualityLevels);
        }
    }

    #region resolution_dropdown
    private void PopulateResolutionDropdown()
    {
        if (resolutionDropdown)
        {
            resolutionDropdown.ClearOptions();
            List<string> resolutions = new List<string>();

            for (int index = 0; index < Screen.resolutions.Length; index++)
            {
                int widthRatio;
                int heightRatio;

                CalculateRatioOfResolution(Screen.resolutions[index].width, Screen.resolutions[index].height, out widthRatio, out heightRatio);
                resolutions.Add(string.Format(resolutionFormat, Screen.resolutions[index].width, Screen.resolutions[index].height, widthRatio, heightRatio));
            }

            resolutionDropdown.AddOptions(resolutions);
        }
    }

    private void CalculateRatioOfResolution(int width, int height, out int widthRatio, out int heightRatio)
    {
        int greatestCommonDenominator = GreatestCommonDenominator(width, height);

        widthRatio = width / greatestCommonDenominator;
        heightRatio = height / greatestCommonDenominator;
    }

    private int GreatestCommonDenominator(int paramModBy, int paramMod)
    {
        if (paramModBy == 0)
        {
            return paramMod;
        }
        return GreatestCommonDenominator(paramMod % paramModBy, paramModBy);
    }
    #endregion

    private void EnableApply()
    {
        apply.interactable = true;
    }

    public void Apply()
    {
        ApplyAudioLevels(masterVolumeSlider.value, musicVolumeSlider.value, soundVolumeSlider.value);
        PlayerPrefs.SetFloat(this.masterVolume, masterVolumeSlider.value);
        PlayerPrefs.SetFloat(this.musicVolume, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(this.soundVolume, soundVolumeSlider.value);

        Screen.SetResolution(Screen.resolutions[resolutionDropdown.value].width, Screen.resolutions[resolutionDropdown.value].height, fullscreenToggle.isOn);
        PlayerPrefs.SetInt(OptionsCanvasManager.resolution, resolutionDropdown.value);
        PlayerPrefs.SetInt(OptionsCanvasManager.fullscreen, fullscreenToggle.isOn ? 1 : 0);

        QualitySettings.SetQualityLevel(qualityDropdown.value);
        PlayerPrefs.SetInt(OptionsCanvasManager.quality, qualityDropdown.value);

        apply.interactable = false;
    }

    private void ApplyAudioLevels(float maVolume, float muVolume, float soVolume)
    {
        if (audioManager)
        {
            audioManager.SetFloat(this.masterVolumeAM, maVolume * 80 - 80);
            audioManager.SetFloat(this.musicVolumeAM, muVolume * 80 - 80);
            audioManager.SetFloat(this.soundVolumeAM, soVolume * 80 - 80);
        }
    }
}
