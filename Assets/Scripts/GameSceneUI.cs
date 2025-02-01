using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
    public static GameSceneUI instance;

    [Header("Main Menu")]
    [SerializeField]
    private GameObject mainMenuGo;
    [SerializeField]
    private Button connectHostButton;
    [SerializeField]
    private Button connectClientButton;
    [SerializeField]
    private TMP_Text connectionInfoText;

    [Header("Game Frame")]
    [SerializeField]
    private GameObject gameFrameGo;
    [SerializeField]
    private TMP_Text playerScoreText;
    [SerializeField]
    private TMP_Text opponentScoreText;
    [SerializeField]
    private Button[] playCardButtons;
    [SerializeField]
    private GameObject playerHandGo;
    [SerializeField]
    private GameObject opponentHandGo;

    [Header("Result Popup")]
    [SerializeField]
    private GameObject resultPopupGo;
    [SerializeField]
    private GameObject resultHeaderWinGo;
    [SerializeField]
    private GameObject resultHeaderLoseGo;
    [SerializeField]
    private TMP_Text resultScoreText;
    [SerializeField]
    private Image resultPlayerInfoImage;
    [SerializeField]
    private Image resultOpponentInfoImage;
    [SerializeField]
    private Sprite[] resultPlayersInfoSprites;

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

    public void ShowConnectionInfo(bool isHost)
    {
        connectionInfoText.text = isHost ? "Connected as Host, wait for client" : "Connected as client, wait to start";
        connectionInfoText.gameObject.SetActive(true);

        connectHostButton.interactable = false;
        connectClientButton.interactable = false;
    }

    /// <summary>
    /// Initialize to start game, switch to game state in canvas
    /// </summary>
    public void InitStartGame()
    {
        SetScores(0, 0);
        EnablePlayCards();
        HandsView(false);
        mainMenuGo.SetActive(false);
        gameFrameGo.SetActive(true);
    }

    private void HandsView(bool isView)
    {
        playerHandGo.SetActive(isView);
        opponentHandGo.SetActive(isView);
    }

    private void SetScores(int playerScore, int opponentScore)
    {
        playerScoreText.text = playerScore.ToString();
        opponentScoreText.text = opponentScore.ToString();
    }

    private void EnablePlayCards()
    {
        foreach (Button playCardButton in playCardButtons)
        {
            playCardButton.interactable = true;
        }
    }

    private void DisablePlayCards()
    {
        foreach (Button playCardButton in playCardButtons)
        {
            playCardButton.interactable = false;
        }
    }

    public void OnPlayCardButtonClicked(int playCard)
    {
        GameManager.instance.PlayerPlayedCard((PlayCardsEnum)playCard);
        DisablePlayCards();
    }

    /// <summary>
    /// Play round by given data from game manager, then update scoreboard and show the win frame if game ends
    /// </summary>
    /// <param name="playerPlayedCard">player's card choice</param>
    /// <param name="playerScore">player's score after this round</param>
    /// <param name="opponentPlayedCard">opponent's card choice</param>
    /// <param name="opponentScore">opponent's score after this round</param>
    /// <param name="isWin">is game ends after this round</param>
    public void PlayRound(PlayCardsEnum playerPlayedCard, int playerScore, PlayCardsEnum opponentPlayedCard,
        int opponentScore, bool isWin)
    {
        StartCoroutine(PlayRoundCoroutine(playerPlayedCard, playerScore, opponentPlayedCard, opponentScore, isWin));
    }

    // wait until showing animations, then update scores
    private IEnumerator PlayRoundCoroutine(PlayCardsEnum playerPlayedCard, int playerScore,
        PlayCardsEnum opponentPlayedCard, int opponentScore, bool isWin)
    {
        AnimationManager.instance.PlayHandsAnimations(playerPlayedCard, opponentPlayedCard);
        yield return new WaitForSeconds(1f);
        SetScores(playerScore, opponentScore);

        if (isWin)
        {
            ShowResultMenu(playerScore, opponentScore);
        }
        else
        {
            EnablePlayCards();
        }
    }

    private void ShowResultMenu(int playerScore, int opponentScore)
    {
        bool isPlayerWin = playerScore > opponentScore;

        playerHandGo.SetActive(false);
        opponentHandGo.SetActive(false);

        resultHeaderWinGo.SetActive(isPlayerWin);
        resultHeaderLoseGo.SetActive(!isPlayerWin);

        resultScoreText.text = playerScore + " - " + opponentScore;
        resultPlayerInfoImage.sprite = NetworkGameManager.Instance._isHost
            ? resultPlayersInfoSprites[0]
            : resultPlayersInfoSprites[1];
        resultOpponentInfoImage.sprite = NetworkGameManager.Instance._isHost
            ? resultPlayersInfoSprites[1]
            : resultPlayersInfoSprites[0];

        resultPopupGo.SetActive(true);
    }
}