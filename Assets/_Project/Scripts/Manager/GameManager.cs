using Cinemachine;
using FMOD.Studio;
using FMODUnity;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public enum GameState { Title, Play }
    
    [SerializeField, Required, Tooltip("Title Screen UI")]
    private UIDocument _titleUI;

    [SerializeField, Required, Tooltip("Game UI")]
    private UIDocument _gameUI;

    [SerializeField, Required, Tooltip("Options UI")]
    private UIDocument _optionsModalUI;

    [SerializeField, Required, Tooltip("Quit UI")]
    private UIDocument _quitModalUI;

    [SerializeField, Required, Tooltip("Camera to activate when Title Screen is shown")]
    private CinemachineVirtualCamera _titleCamera;

    [SerializeField]
    private GameState _currentGameState = GameState.Title;

    [SerializeField]
    private bool _skipTitleScreen = false;

    [SerializeField]
    private EventReference _titleMusic, _uiClick;

    private EventInstance _titleMusicInstance;

    public static Action<GameManager.GameState> GameStateChange;

    private void Awake()
    {
        // Configure quit button
        ConfigureQuitModal();

        PlayerInput.GetPlayerByIndex(0).actions["Quit"].performed += (callback) =>
        {
            ToggleQuitModal();
        };

        _quitModalUI.rootVisualElement.AddToClassList("hidden");

        // Toggle options button
        ConfigureOptionsModal();
        
        PlayerInput.GetPlayerByIndex(0).actions["Settings"].performed += (callback) =>
        {
            OptionsMenuToggle();
        };

        _optionsModalUI.rootVisualElement.AddToClassList("hidden");

        ConfigureTitleScreenSound();

        // Toggle title screen
        if (_skipTitleScreen == false)
        {
            // Disable player input?
            SetGameState(GameState.Title);
        }
        else
        {
            SetGameState(GameState.Play);
        }
    }

    private void OnDestroy()
    {
        _titleMusicInstance.release();
    }

    private void ConfigureTitleScreenSound()
    {
        _titleMusicInstance = FMODUnity.RuntimeManager.CreateInstance(_titleMusic);
        _titleMusicInstance.setVolume(0.5f);
    }

    private void SetGameState(GameState state)
    {
        switch (state) 
        {
            case GameState.Title:
                ConfigureTitleScreen();
                RegisterTitleScreenCallbacks();

                // Play the title music
                _titleMusicInstance.start();
                break;
            case GameState.Play:
                DisableTitleScreen();
                
                // Start the 'play' music
                _titleMusicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
        }

        _currentGameState = state;
        GameStateChange?.Invoke(state);
    }

    private void ConfigureQuitModal()
    {
        VisualElement rootElement = _quitModalUI.rootVisualElement;
        Button confirmButton = rootElement.Q<Button>("Confirm");
        Button cancelButton = rootElement.Q<Button>("Cancel");

        confirmButton.clicked += () =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        };

        cancelButton.clicked += () =>
        {
            ToggleQuitModal();
            
        };
    }

    private void ToggleQuitModal()
    {
        if (_quitModalUI.rootVisualElement.ClassListContains("hidden"))
        {
            _quitModalUI.rootVisualElement.RemoveFromClassList("hidden");
        }
        else
        {
            _quitModalUI.rootVisualElement.AddToClassList("hidden");
        }

        // Hide the Options Menu (if it's visible)
        if (_optionsModalUI.rootVisualElement.ClassListContains("hidden") == false)
        {
            _optionsModalUI.rootVisualElement.AddToClassList("hidden");
        }

        RuntimeManager.PlayOneShot(_uiClick);
    }

    private void ConfigureOptionsModal()
    {
        VisualElement rootElement = _optionsModalUI.rootVisualElement;
        Button backButton = rootElement.Q<Button>("Back");

        backButton.clicked += () =>
        {
            OptionsMenuToggle();
        };

        // Get all the available quality options and put them into the fields
        string[] allSettings = QualitySettings.names;
        
        DropdownField qualityDropdown = rootElement.Q<DropdownField>("QualityDropdown");
        qualityDropdown.choices.Clear();
        
        foreach (var setting in allSettings)
        {
            qualityDropdown.choices.Add(setting);
            qualityDropdown.value = allSettings[QualitySettings.GetQualityLevel()];
        }

        // subscribe to changes
        qualityDropdown.RegisterValueChangedCallback(newValue =>
        {
            QualitySettings.SetQualityLevel(qualityDropdown.index);
            RuntimeManager.PlayOneShot(_uiClick);
        });
    }

    private void OptionsMenuToggle()
    {
        Debug.Log("Clicked options toggle");
        
        if (_optionsModalUI.rootVisualElement.ClassListContains("hidden"))
        {
            _optionsModalUI.rootVisualElement.RemoveFromClassList("hidden");
        }
        else
        {
            _optionsModalUI.rootVisualElement.AddToClassList("hidden");
        }

        RuntimeManager.PlayOneShot(_uiClick);
    }

    private void ConfigureTitleScreen()
    {
        _gameUI.rootVisualElement.AddToClassList("hidden");
        _titleUI.rootVisualElement.RemoveFromClassList("hidden");

        _titleCamera.enabled = true;
    }

    private void DisableTitleScreen()
    {
        _titleUI.rootVisualElement.AddToClassList("hidden");
        _titleCamera.enabled = false;

        StartCoroutine(WaitForBlend());
    }

    private IEnumerator WaitForBlend()
    {
        yield return new WaitForSeconds(2);

        _gameUI.rootVisualElement.RemoveFromClassList("hidden");
    }

    private void RegisterTitleScreenCallbacks()
    {
        // TODO: Move this to the TitleScreenUI GameObject so we can use OnActivate
        // Register the Start and Options buttons

        VisualElement rootElement = _titleUI.rootVisualElement;
        Button startButton = rootElement.Q<Button>("StartButton");

        startButton.clicked += () =>
        {
            Debug.Log("Start button clicked down!");
            DisableTitleScreen();
            SetGameState(GameState.Play);
            RuntimeManager.PlayOneShot(_uiClick);
        };

        startButton.RegisterCallback<MouseOverEvent>(e =>
        {
            Debug.Log("Mouse over start button!");
        });

        // Options Button
        Button optionsButton = rootElement.Q<Button>("OptionsButton");

        optionsButton.clicked += () =>
        {
            Debug.Log("Options button clicked down!");
            OptionsMenuToggle();
            RuntimeManager.PlayOneShot(_uiClick);
        };
    }
}
