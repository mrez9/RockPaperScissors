using UnityEngine;
using DG.Tweening;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [SerializeField]
    private GameObject playerHandGameObject;
    [SerializeField]
    private GameObject botHandGameObject;
    [SerializeField]
    private Animator playerHandAnimator;
    [SerializeField]
    private Animator botHandAnimator;

    private const string PlayRockAnim = "Playing Rock";
    private const string PlayPaperAnim = "Playing Paper";
    private const string PlayScissorsAnim = "Playing Scissors";

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
    /// Playing animation of player's hand
    /// </summary>
    /// <param name="playCardsEnum">player's card choice</param>
    public void PlayerHandPlay(PlayCardsEnum playCardsEnum)
    {
        string animState = GetAnimState(playCardsEnum);

        // because playing rock not showing fingers, it must be closer to middle
        float endValue = playCardsEnum == PlayCardsEnum.Rock ? -120 : -175;

        playerHandAnimator.Play(animState, 0, 0f);
        playerHandGameObject.SetActive(true);
        playerHandGameObject.transform.localPosition =
            new Vector3(playerHandGameObject.transform.localPosition.x, -800);
        playerHandGameObject.transform.DOLocalMoveY(endValue, 1f);
    }

    /// <summary>
    /// Playing animation of bot's hand
    /// </summary>
    /// <param name="playCardsEnum">bot's card choice</param>
    public void BotHandPlay(PlayCardsEnum playCardsEnum)
    {
        string animState = GetAnimState(playCardsEnum);

        //float endValue = playCardsEnum == PlayCardsEnum.Rock ? 120 : 175;
        float endValue = 175;
        Debug.Log("AAAA:" + animState);

        botHandAnimator.Play(animState, 0, 0f);
        botHandGameObject.SetActive(true);
        botHandGameObject.transform.localPosition =
            new Vector3(botHandGameObject.transform.localPosition.x, 800);
        botHandGameObject.transform.DOLocalMoveY(endValue, 1f).OnComplete(() =>
        {
            GameManager.instance.RoundPlayed();
        });
    }

    #region Animation State

    private string GetAnimState(PlayCardsEnum playCardsEnum)
    {
        if (playCardsEnum == PlayCardsEnum.Rock)
        {
            return PlayRockAnim;
        }

        if (playCardsEnum == PlayCardsEnum.Paper)
        {
            return PlayPaperAnim;
        }

        if (playCardsEnum == PlayCardsEnum.Scissors)
        {
            return PlayScissorsAnim;
        }

        Debug.LogError("Error in play card state");
        return PlayRockAnim;
    }

    #endregion
}