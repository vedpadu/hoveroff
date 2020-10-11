using System.Collections.Generic;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
public class playfabLogin : MonoBehaviour
{
    private string userEmail;
    private string userPassword;
    private string username;
    public GameObject loginPanel;
    bool hasStatisticsInitialized;
    public List<string> statsToCheck;
    private levelManagerScript lMS;
    public void Start()
    {
        lMS = GameObject.FindGameObjectWithTag("levelSelect").GetComponent<levelManagerScript>();
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)){
            /*
            Please change the titleId below to your own titleId from PlayFab Game Manager.
            If you have already set the value in the Editor Extensions, this can be skipped.
            */
            PlayFabSettings.staticSettings.TitleId = "8DA36";
        }
        PlayerPrefs.DeleteAll();
        DoLogin();
    }

    void DoLogin()
    {
        if (PlayerPrefs.HasKey("EMAIL"))
        {
            userEmail = PlayerPrefs.GetString("EMAIL");
            userPassword = PlayerPrefs.GetString("PASSWORD");
            var request = new LoginWithEmailAddressRequest {Email = userEmail, Password = userPassword};
            PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
        }
        else
        {
            loginPanel.SetActive(true);
        }
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");
        initStats();
        if (loginPanel.activeSelf)
        {
            lMS.LoadNextLevel(1,0);
        }
        else
        {
            lMS.LoadNextLevel(1,1);
        }
      
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        PlayerPrefs.SetString("PlayFabId", result.PlayFabId);
        //loginPanel.SetActive(false);
    }

    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("Register Success");
        PlayerPrefs.SetString("EMAIL", userEmail);
        PlayerPrefs.SetString("PASSWORD", userPassword);
        PlayFabClientAPI.UpdateUserTitleDisplayName(new UpdateUserTitleDisplayNameRequest {DisplayName = username}, OnDisplayNameSuccess, OnDisplayNameFailure);
        DoLogin();
    }

    void OnDisplayNameSuccess(UpdateUserTitleDisplayNameResult result)
    {
        print("Display name: " + result);
    }

    void OnDisplayNameFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnLoginFailure(PlayFabError error)
    {
        var registerRequest = new RegisterPlayFabUserRequest {Email = userEmail, Password = userPassword, Username = username};
        PlayFabClientAPI.RegisterPlayFabUser(registerRequest, OnRegisterSuccess, OnRegisterFailure);
    }

    public void GetUserEmail(string emailIn)
    {
        userEmail = emailIn;
    }

    public void GetUserPassword(string passwordIn)
    {
        userPassword = passwordIn;
    }

    public void GetUserName(string usernameIn)
    {
        username = usernameIn;
    }

    public void OnClickLogin()
    {
        var request = new LoginWithEmailAddressRequest {Email = userEmail, Password = userPassword};
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);
    }

    private void initStats()
    {
        GetStatisticsForCheckInit();

    }
    
    void GetStatisticsForCheckInit()
    {
        PlayFabClientAPI.GetPlayerStatistics(
            new GetPlayerStatisticsRequest(),
            CheckStatsInit,
            error => Debug.LogError(error.GenerateErrorReport())
        );
    }

    void CheckStatsInit(GetPlayerStatisticsResult result)
    {
        List<string> stats = new List<string>();
        for (var i = 0; i < statsToCheck.Count; i++)
        {
            stats.Add(statsToCheck[i]);
        }

        foreach (var eachStat in result.Statistics)
        {
            if (stats.Contains(eachStat.StatisticName))
            {
                stats.Remove(eachStat.StatisticName);
            }
        }
        if (stats.Count == 0)
        {
            hasStatisticsInitialized = true;
        }
        else
        {
            hasStatisticsInitialized = false;
        }
        
       /* PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest {
                // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
                Statistics = new List<StatisticUpdate> {
                    new StatisticUpdate { StatisticName = "laserDroneTime", Value = -1000000000 },
                }
            },
            result1 => { Debug.Log("User statistics updated"); },
            error => { Debug.LogError(error.GenerateErrorReport()); });*/
        if (!hasStatisticsInitialized)
        {
            PlayFabClientAPI.UpdatePlayerStatistics( new UpdatePlayerStatisticsRequest {
                    // request.Statistics is a list, so multiple StatisticUpdate objects can be defined if required.
                    Statistics = new List<StatisticUpdate> {
                        new StatisticUpdate { StatisticName = "level", Value = 0 },
                    }
                },
                result1 => { Debug.Log("User statistics updated"); },
                error => { Debug.LogError(error.GenerateErrorReport()); });
        }
        
    }
}