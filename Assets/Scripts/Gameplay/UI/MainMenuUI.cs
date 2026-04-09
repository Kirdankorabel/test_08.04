using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Gameplay.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private GameObject _panel;
        [SerializeField] private TMP_Text _bestScoreText;
        [SerializeField] private Button _playButton;

        public event Action OnPlayClicked;

        public void Show(int bestScore)
        {
            _bestScoreText.text = $"Best: {bestScore}";
            _panel.SetActive(true);
        }

        public void Hide()
        {
            _panel.SetActive(false);
        }

        private void Start()
        {
            _playButton.onClick.AddListener(() => OnPlayClicked?.Invoke());
        }
    }
}
