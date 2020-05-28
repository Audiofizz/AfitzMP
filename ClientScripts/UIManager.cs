using UnityEngine;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject allMenus;

    #region StartMenu

    public GameObject startMenu;
    public InputField usernameField;
    public InputField ipAddressField;

    private bool atStart = false;

    #endregion

    public GameObject gameMenu;

    public GameObject leaderboard;

    public GameObject connectingUI;

    [SerializeField] private SpriteFade hitMarker;

    [SerializeField] private SpriteFade bloodSplat;

    public Slider localHpbar;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists");
            Destroy(this);
        }
    }

    private void Start()
    {
        if (!allMenus.activeSelf)
            allMenus.SetActive(true);

        if (!Client.instance.Connected() && !atStart)
        {
            DisconnectedUI();
        }
    }

    public void ConnectToServer()
    {
        MainMenu(false);
        Client.instance.ConnectToServer();
        atStart = false;
    }

    #region Menu Functions

    public void MainMenu(bool active)
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
            startMenu.SetActive(active);
            connectingUI.SetActive(!active);
            usernameField.interactable = active;
        });
    }
    
    public void InGameMenu(bool active)
    {
        if (gameMenu != null)
            gameMenu.SetActive(active);
    }

    public void ShowLeaderboard(bool active)
    {
        if (instance.leaderboard != null)
        {
            Leaderboard.instance.Active(active);
            if (active)
                Leaderboard.instance.UpdateScore();
        }
    }

    #endregion

    public void DisconnectedUI()
    {
        MainMenu(true);
        InGameMenu(false);
        atStart = true;
    }

    public void DisconnectFromServer()
    {
        Client.DisconnectFromServer();
    }

    public void DisableAllMenuUI()
    {
        ThreadManager.ExecuteOnMainThread(() =>
        {
            startMenu.SetActive(false);
            usernameField.interactable = false;
            connectingUI.SetActive(false);
        });
    }

    public void HitMarker()
    {
        hitMarker.Activate();
    }

    public void GotKill()
    {

    }

    public void TakeDamage()
    {
        bloodSplat.Activate();
    }
}
