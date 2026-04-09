using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Gameplay.UI
{
    public class GameOverUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _exitButton;

        public event Action OnRestartClicked;
        public event Action OnExitClicked;

        public void Show(int score)
        {
            _scoreText.text = score.ToString();
            _panel.SetActive(true);
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }

        private void Start()
        {
            _restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
            _exitButton.onClick.AddListener(() => OnExitClicked?.Invoke());
        }
    }
}
