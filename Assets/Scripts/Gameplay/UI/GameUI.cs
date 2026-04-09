using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game2048.Gameplay.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _scoreText;
        [SerializeField] private Button _autoMergeButton;
        [SerializeField] private Button _exitButton;

        public event Action OnAutoMergeClicked;
        public event Action OnExitClicked;

        public void UpdateScore(int score)
        {
            _scoreText.text = score.ToString();
        }

        public void SetAutoMergeInteractable(bool interactable)
        {
            _autoMergeButton.interactable = interactable;
        }

        private void Start()
        {
            _autoMergeButton.onClick.AddListener(() => OnAutoMergeClicked?.Invoke());
            _exitButton.onClick.AddListener(() => OnExitClicked?.Invoke());
        }
    }
}
