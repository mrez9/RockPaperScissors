using FishNet.Connection;
using UnityEngine;
using FishNet.Object;

public class GameManager : NetworkBehaviour
{
    public static GameManager instance;

    private int player1Score, player2Score;
    private PlayCardsEnum player1PlayedCard, player2PlayedCard;
    private bool player1Played, player2Played;

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
        RPCInitGame();
    }

    [ObserversRpc]
    private void RPCInitGame()
    {
        GameSceneUI.instance.InitStartGame();
    }

    /// <summary>
    /// Player's card choice to play
    /// </summary>
    /// <param name="playedCard">player's choice</param>
    public void PlayerPlayedCard(PlayCardsEnum playedCard)
    {
        RPCPlayerPlayedCard(playedCard);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RPCPlayerPlayedCard(PlayCardsEnum playedCard, NetworkConnection conn = null)
    {
        if (conn.ClientId == 0)
        {
            player1PlayedCard = playedCard;
            player1Played = true;
            Debug.Log($"player1 played :{playedCard}");
        }
        else
        {
            player2PlayedCard = playedCard;
            player2Played = true;
            Debug.Log($"player2 played :{playedCard}");
        }

        if (player1Played && player2Played)
        {
            RoundPlayed();
        }
    }

    /// <summary>
    /// Get result of played round and reset variables 
    /// </summary>
    private void RoundPlayed()
    {
        int result = GetRoundResult(player1PlayedCard, player2PlayedCard);
        if (result == 1)
        {
            player1Score++;
        }
        else if (result == 2)
        {
            player2Score++;
        }

        bool isWin = player1Score == 5 || player2Score == 5;

        RPCSendRound(player1PlayedCard, player2PlayedCard, player1Score, player2Score, isWin);
        player1Played = false;
        player2Played = false;
    }

    [ObserversRpc]
    private void RPCSendRound(PlayCardsEnum p1PlayedCard, PlayCardsEnum p2PlayedCard, int p1Score, int p2Score,
        bool isWin)
    {
        if (NetworkGameManager.Instance._isHost)
        {
            GameSceneUI.instance.PlayRound(p1PlayedCard, p1Score, p2PlayedCard, p2Score, isWin);
        }
        else
        {
            GameSceneUI.instance.PlayRound(p2PlayedCard, p2Score, p1PlayedCard, p1Score, isWin);
        }
    }

    /// <summary>
    /// Reset last ended game stats
    /// </summary>
    private void ResetGame()
    {
        player1Score = 0;
        player2Score = 0;
    }

    /// <summary>
    /// Calculate played round's result
    /// </summary>
    /// <param name="player1Choice">player 1 choice card</param>
    /// <param name="player2Choice">player 2 choice card</param>
    /// <returns>player id of winner, zero for draw</returns>
    private int GetRoundResult(PlayCardsEnum player1Choice, PlayCardsEnum player2Choice)
    {
        if (player1Choice == player2Choice)
        {
            return 0;
        }

        switch (player1Choice)
        {
            case PlayCardsEnum.Rock:
                return player2Choice == PlayCardsEnum.Scissors ? 1 : 2;
            case PlayCardsEnum.Paper:
                return player2Choice == PlayCardsEnum.Rock ? 1 : 2;
            case PlayCardsEnum.Scissors:
                return player2Choice == PlayCardsEnum.Paper ? 1 : 2;
            default:
                Debug.LogError(
                    $"Error in get result of round, playerCard:{player1Choice}, opponentCard:{player2Choice}");
                return 0;
        }
    }
}