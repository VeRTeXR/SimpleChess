using echo17.Signaler.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Summary
{
    public class SummaryScreenController : MonoBehaviour, ISubscriber, IBroadcaster
    {
        [SerializeField] private GameObject layoutObject;
        [SerializeField] private TextMeshProUGUI winnerText; 
        [SerializeField] private Button restartButton;
        [SerializeField] private Button exitButton;

        private void Awake()
        {
            Signaler.Instance.Subscribe<GameOver>(this, OnGameOver);
        }

        private bool OnGameOver(GameOver signal)
        {
            SetupButtonEvents();
            layoutObject.SetActive(true);
            
            if (signal.WonPlayerData != null)
            {
                winnerText.text = signal.WonPlayerData.Name.ToUpper() + " WIN!";
            }
            return true;
        }

        private void SetupButtonEvents()
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
            exitButton.onClick.RemoveAllListeners();
            exitButton.onClick.AddListener(ExitGame);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

        private void RestartGame()
        {
            layoutObject.SetActive(false);
            Signaler.Instance.Broadcast(this, new RestartSession());
        }
    }
}
