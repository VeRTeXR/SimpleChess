using echo17.Signaler.Core;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StartScreenController : MonoBehaviour, ISubscriber, IBroadcaster
    {
        [SerializeField] private GameObject startScreenLayout;
        [SerializeField] private Button startGameButton;   
        
        private void Start()
        {
            startScreenLayout.SetActive(true);
            SetupButtonEvents();
        }

        private void SetupButtonEvents()
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            startScreenLayout.SetActive(false);
            Signaler.Instance.Broadcast(this, new InitializeGameSession());
        }
    }
}
