using System;
using Data;
using echo17.Signaler.Core;
using UI.Summary;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class StartScreenController : MonoBehaviour, ISubscriber, IBroadcaster
    {
        [SerializeField] private GameObject startScreenLayout;
        [SerializeField] private Button startGameButton;

        private void Awake()
        {
            Signaler.Instance.Subscribe<BackToTitle>(this, OnBackToTitle);
        }

        private bool OnBackToTitle(BackToTitle signal)
        {
            startScreenLayout.SetActive(true);
            SetupButtonEvents();
            return true;
        }
        
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
