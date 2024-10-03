using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    public static GameSceneUI instance;

    [Header("Main Menu")]
    [SerializeField]
    private GameObject mainMenuGameObject;

    [Header("Game Menu")]
    [SerializeField]
    private GameObject gameMenuGameObject;
    [SerializeField]
    private TMP_Text playerScoreText;
    [SerializeField]
    private TMP_Text botScoreText;
    [SerializeField]
    private Button playCardRockButton;
    [SerializeField]
    private Button playCardPaperButton;
    [SerializeField]
    private Button playCardScissorsButton;
    [SerializeField]
    private GameObject playerHandGameObject;
    [SerializeField]
    private GameObject botHandGameObject;

    [Header("Result Menu")]
    [SerializeField]
    private GameObject resultMenuGameObject;
    [SerializeField]
    private GameObject resultMenuHeaderWinGameObject;
    [SerializeField]
    private GameObject resultMenuHeaderLoseGameObject;
    [SerializeField]
    private TMP_Text resultMenuScoreText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }
    }

    #region Main Menu

    public void ShowMainMenu()
    {
        mainMenuGameObject.SetActive(true);
    }

    public void StartButtonClicked()
    {
        mainMenuGameObject.SetActive(false);
        GameManager.instance.StartGame();
    }

    #endregion

    #region Game Menu

    public void InitStartGame()
    {
        resultMenuGameObject.SetActive(false);
        playerHandGameObject.SetActive(false);
        botHandGameObject.SetActive(false);
        
        gameMenuGameObject.SetActive(true);
    }

    #endregion

    #region PlayCards

    public void EnablePlayCards()
    {
        playCardRockButton.interactable = true;
        playCardPaperButton.interactable = true;
        playCardScissorsButton.interactable = true;
    }

    public void DisablePlayCards()
    {
        playCardRockButton.interactable = false;
        playCardPaperButton.interactable = false;
        playCardScissorsButton.interactable = false;
    }

    public void PlayCardRockClicked()
    {
        GameManager.instance.PlayerPlayedCard(PlayCardsEnum.Rock);
    }

    public void PlayCardPaperClicked()
    {
        GameManager.instance.PlayerPlayedCard(PlayCardsEnum.Paper);
    }

    public void PlayCardScissorsClicked()
    {
        GameManager.instance.PlayerPlayedCard(PlayCardsEnum.Scissors);
    }

    #endregion

    #region Scoreboard

    public void SetPlayerScore(int score)
    {
        playerScoreText.text = score.ToString();

    }

    public void SetBotScore(int score)
    {
        botScoreText.text = score.ToString();
    }
    #endregion

    #region Result Menu

    public void ShowResultMenu(bool isWin, int playerScore, int botScore)
    {
        playerHandGameObject.SetActive(false);
        botHandGameObject.SetActive(false);

        resultMenuHeaderWinGameObject.SetActive(isWin);
        resultMenuHeaderLoseGameObject.SetActive(!isWin);

        resultMenuScoreText.text = playerScore + " - " + botScore;

        resultMenuGameObject.SetActive(true);
    }

    public void CancelButtonClicked()
    {
        resultMenuGameObject.SetActive(false);

        playerHandGameObject.SetActive(true);
        botHandGameObject.SetActive(true);

        gameMenuGameObject.SetActive(false);
        ShowMainMenu();
    }

    public void PlayAgainButtonClicked()
    {
        resultMenuGameObject.SetActive(false);
        GameManager.instance.StartGame();
    }

    #endregion
}