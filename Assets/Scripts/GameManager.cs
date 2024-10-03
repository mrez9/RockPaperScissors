using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int playerScore, botScore;
    private PlayCardsEnum playerPlayedCard, botPlayedCard;

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

    /// <summary>
    /// Reset previous game and start new game
    /// </summary>
    public void StartGame()
    {
        ResetGame();
        GameSceneUI.instance.InitStartGame();
    }

    /// <summary>
    /// Player's card choice to play
    /// </summary>
    /// <param name="playCardsEnum">player's choice</param>
    public void PlayerPlayedCard(PlayCardsEnum playCardsEnum)
    {
        playerPlayedCard = playCardsEnum;
        GameSceneUI.instance.DisablePlayCards();
        AnimationManager.instance.PlayerHandPlay(playCardsEnum);
        PlayBotRandomCard();
    }

    /// <summary>
    /// Playing a random choice for bot
    /// </summary>
    private void PlayBotRandomCard()
    {
        PlayCardsEnum botPlayCard = (PlayCardsEnum)Random.Range(0, 3);
        botPlayedCard = botPlayCard;
        AnimationManager.instance.BotHandPlay(botPlayCard);
    }

    /// <summary>
    /// Get result of played round and reset variables 
    /// </summary>
    public void RoundPlayed()
    {
        int result = GetRoundResult(playerPlayedCard, botPlayedCard);
        if (result == 1)
        {
            playerScore++;
            GameSceneUI.instance.SetPlayerScore(playerScore);
        }
        else if (result == -1)
        {
            botScore++;
            GameSceneUI.instance.SetBotScore(botScore);
        }

        //checking game ends
        if (playerScore == 5 || botScore == 5)
        {
            bool isWin = playerScore == 5;
            GameSceneUI.instance.ShowResultMenu(isWin, playerScore, botScore);
        }
        else
        {
            GameSceneUI.instance.EnablePlayCards();
        }
    }

    /// <summary>
    /// Reset last ended game stats
    /// </summary>
    private void ResetGame()
    {
        playerScore = 0;
        botScore = 0;
        GameSceneUI.instance.SetPlayerScore(0);
        GameSceneUI.instance.SetBotScore(0);
        GameSceneUI.instance.EnablePlayCards();
    }

    #region Result

    /// <summary>
    /// Calculate played round's result
    /// </summary>
    /// <param name="playerCard">player's choice</param>
    /// <param name="botCard">bot's choice</param>
    /// <returns></returns>
    private int GetRoundResult(PlayCardsEnum playerCard, PlayCardsEnum botCard)
    {
        if (playerCard == botCard)
        {
            return 0;
        }

        switch (playerCard)
        {
            case PlayCardsEnum.Rock:
                return botCard == PlayCardsEnum.Scissors ? 1 : -1;
            case PlayCardsEnum.Paper:
                return botCard == PlayCardsEnum.Rock ? 1 : -1;
            case PlayCardsEnum.Scissors:
                return botCard == PlayCardsEnum.Paper ? 1 : -1;
            default:
                Debug.LogError($"Error in get result of round, playerCard:{playerCard}, botCard:{botCard}");
                return 0;
        }
    }

    #endregion
}