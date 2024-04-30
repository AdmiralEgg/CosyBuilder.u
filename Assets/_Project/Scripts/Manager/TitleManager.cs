using Cinemachine;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TitleManager : MonoBehaviour
{
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
    private bool _skipTitleScreen = false;

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

        // Toggle title screen
        if (_skipTitleScreen == false)
        {
            // Disable player input?
            
            ConfigureTitleScreen();
            RegisterTitleScreenCallbacks();
        }
        else
        {
            DisableTitleScreen();
        }
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
            Debug.Log("new value " + newValue);
            QualitySettings.SetQualityLevel(qualityDropdown.index);
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

        startButton.RegisterCallback<MouseDownEvent>(e =>
        {
            Debug.Log("Start button clicked down!");
            DisableTitleScreen();
        });

        startButton.clicked += () =>
        {
            Debug.Log("Start button clicked down!");
            DisableTitleScreen();
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
        };
    }
}
