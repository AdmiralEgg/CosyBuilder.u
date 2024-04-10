using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TitleManager : MonoBehaviour
{
    [SerializeField, Required, Tooltip("Title Screen UI")]
    private UIDocument _titleUI;

    [SerializeField, Required, Tooltip("Game UI")]
    private UIDocument _gameUI;

    [SerializeField, Required, Tooltip("Camera to activate when Title Screen is shown")]
    private CinemachineVirtualCamera _titleCamera;
    
    [SerializeField]
    private bool _skipTitleScreen = false;

    void Start()
    {
        //case GameState.Title:
        //    s_buildModeUI.gameObject.SetActive(false);
        //    s_titleUI.gameObject.SetActive(true);
        //    s_titleCamera.gameObject.SetActive(true);
        //    RegisterTitleButtonCallbacks();
        //    SwitchInputMap("Title");
        //    EnableInputMap("FocusCamera", false);
        //    EnableInputMap("BuildCamera", false);
        //    EnableInputMap("UI", true);
        //    break;

        if (_skipTitleScreen == false)
        {
            ConfigureTitleScreen();
            RegisterTitleButtonCallbacks();
        }
        else
        {
            DisableTitleScreen();
        }
    }

    private void ConfigureTitleScreen()
    {
        //_gameUI.enabled = false;
        

        // disable inventory UI
        _titleUI.enabled = true;
        _titleCamera.enabled = true;
    }

    private void DisableTitleScreen()
    {
        // disable inventory UI
        _titleUI.enabled = false;
        _titleCamera.enabled = false;

        StartCoroutine(WaitForBlend());
    }

    private IEnumerator WaitForBlend()
    {
        yield return new WaitForSeconds(2);
        
        //_gameUI.enabled = true;
    }

    private void RegisterTitleButtonCallbacks()
    {
        // TODO: Move this to the TitleScreenUI GameObject so we can use OnActivate
        // Register the Start and Options buttons

        VisualElement rootElement = _titleUI.rootVisualElement;
        Button startButton = rootElement.Q<Button>("StartButton");
        Button optionsButton = rootElement.Q<Button>("OptionsButton");

        //startButton.clicked += OnClickStart;

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
    }
}
