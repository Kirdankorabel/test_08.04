using System;
using Game2048.Gameplay.GameFlow;

namespace Game2048.Gameplay.UI
{
    public class UIController : IGameStateListener
    {
        private readonly MainMenuUI _mainMenuUI;
        private readonly GameUI _gameUI;
        private readonly GameOverUI _gameOverUI;

        public event Action OnPlayClicked;
        public event Action OnRestartClicked;
        public event Action OnExitClicked;
        public event Action OnAutoMergeClicked;

        public UIController(
            MainMenuUI mainMenuUI,
            GameUI gameUI,
            GameOverUI gameOverUI)
        {
            _mainMenuUI = mainMenuUI;
            _gameUI = gameUI;
            _gameOverUI = gameOverUI;

            _mainMenuUI.OnPlayClicked += HandlePlay;
            _gameUI.OnAutoMergeClicked += () => OnAutoMergeClicked?.Invoke();
            _gameUI.OnExitClicked += HandleExit;
            _gameOverUI.OnRestartClicked += HandleRestart;
            _gameOverUI.OnExitClicked += HandleExit;
        }

        public void OnGameStateChanged(GameState state) { }

        public void ShowMainMenu(int bestScore)
        {
            _mainMenuUI.Show(bestScore);
            _gameUI.gameObject.SetActive(false);
            _gameOverUI.Hide();
        }

        public void ShowGameOver(int score)
        {
            _gameOverUI.Show(score);
        }

        public void UpdateScore(int score)
        {
            _gameUI.UpdateScore(score);
        }

        public void SetAutoMergeInteractable(bool interactable)
        {
            _gameUI.SetAutoMergeInteractable(interactable);
        }

        private void HandlePlay()
        {
            _mainMenuUI.Hide();
            _gameUI.gameObject.SetActive(true);
            _gameUI.UpdateScore(0);
            _gameUI.SetAutoMergeInteractable(false);
            _gameOverUI.Hide();

            OnPlayClicked?.Invoke();
        }

        private void HandleRestart()
        {
            _gameOverUI.Hide();
            _gameUI.UpdateScore(0);
            _gameUI.SetAutoMergeInteractable(false);

            OnRestartClicked?.Invoke();
        }

        private void HandleExit()
        {
            OnExitClicked?.Invoke();
        }
    }
}
