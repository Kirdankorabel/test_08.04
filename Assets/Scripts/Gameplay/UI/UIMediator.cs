using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game2048.Gameplay.Cubes;
using Game2048.Gameplay.GameFlow;
using Game2048.Gameplay.Scores;
using Zenject;

namespace Game2048.Gameplay.UI
{
    public class UIMediator : IDisposable
    {
        private readonly UIController _uiController;
        private readonly GameController _gameController;
        private readonly ScoreService _scoreService;
        private readonly CubeController _cubeController;

        private CancellationTokenSource _autoMergeCts;

        [Inject]
        public UIMediator(
            UIController uiController,
            GameController gameController,
            ScoreService scoreService,
            CubeController cubeController)
        {
            _uiController = uiController;
            _gameController = gameController;
            _scoreService = scoreService;
            _cubeController = cubeController;

            _uiController.OnPlayClicked += OnPlay;
            _uiController.OnRestartClicked += OnRestart;
            _uiController.OnExitClicked += OnExit;
            _uiController.OnAutoMergeClicked += OnAutoMerge;
            _scoreService.OnScoreChanged += OnScoreChanged;

            _uiController.ShowMainMenu(_scoreService.BestScore);
        }

        public void Dispose()
        {
            _uiController.OnPlayClicked -= OnPlay;
            _uiController.OnRestartClicked -= OnRestart;
            _uiController.OnExitClicked -= OnExit;
            _uiController.OnAutoMergeClicked -= OnAutoMerge;
            _scoreService.OnScoreChanged -= OnScoreChanged;

            _autoMergeCts?.Cancel();
            _autoMergeCts?.Dispose();
        }

        private void OnPlay()
        {
            _gameController.StartGame();
        }

        private void OnRestart()
        {
            _autoMergeCts?.Cancel();
            _gameController.StartGame();
        }

        private void OnExit()
        {
            _autoMergeCts?.Cancel();
            _scoreService.SaveBestScore();
            _gameController.StopGame();
            _uiController.ShowMainMenu(_scoreService.BestScore);
        }

        private void OnAutoMerge()
        {
            _gameController.StartAutoMerge();

            _autoMergeCts?.Cancel();
            _autoMergeCts = new CancellationTokenSource();
            RunAutoMergeAsync(_autoMergeCts.Token).Forget();
        }

        private async UniTaskVoid RunAutoMergeAsync(CancellationToken ct)
        {
            await _cubeController.ExecuteAutoMergeAsync(ct);
            _gameController.FinishAutoMerge();
        }

        private void OnScoreChanged(int score)
        {
            _uiController.UpdateScore(score);
        }
    }
}
