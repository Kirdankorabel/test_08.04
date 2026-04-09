using Game2048.Gameplay.Board;
using Game2048.Gameplay.Cubes;
using Game2048.Gameplay.GameFlow.States;
using Game2048.Gameplay.Scores;
using Game2048.Gameplay.UI;
using Game2048.Infrastructure.Core;
using Game2048.Infrastructure.Input;
using Game2048.Infrastructure.Launch;
using Zenject;

namespace Game2048.Gameplay.GameFlow
{
    public class GameController : ITickable, IGameStateListener
    {
        private readonly TouchInputService _inputService;
        private readonly LaunchService _launchService;
        private readonly BoardService _boardService;
        private readonly ICubeController _cubeController;
        private readonly ScoreService _scoreService;
        private readonly UIController _uiController;
        private readonly GameSettings _settings;

        private GameStateMachine _stateMachine;
        private GameOverDetector _gameOverDetector;

        [Inject]
        public GameController(
            TouchInputService inputService,
            LaunchService launchService,
            BoardService boardService,
            ICubeController cubeController,
            ScoreService scoreService,
            UIController uiController,
            GameSettings settings)
        {
            _inputService = inputService;
            _launchService = launchService;
            _boardService = boardService;
            _cubeController = cubeController;
            _scoreService = scoreService;
            _uiController = uiController;
            _settings = settings;
        }

        public void Tick()
        {
            _stateMachine?.Tick();
        }

        public void OnGameStateChanged(GameState state)
        {
            switch (state)
            {
                case GameState.WaitingForInput:
                    var canAutoMerge = _cubeController.TryFindMergeablePair(out _, out _);
                    _uiController.SetAutoMergeInteractable(canAutoMerge);
                    break;

                case GameState.GameOver:
                    _scoreService.SaveBestScore();
                    _uiController.ShowGameOver(_scoreService.CurrentScore);
                    break;

                default:
                    _uiController.SetAutoMergeInteractable(false);
                    break;
            }
        }

        public void StartGame()
        {
            _scoreService.Reset();
            _cubeController.Restart();
            BuildStateMachine();
            _cubeController.Spawn();
            _inputService.ResetState();
            _stateMachine.ChangeState(GameState.WaitingForInput);
        }

        public void StopGame()
        {
            _cubeController.Restart();
            _stateMachine = null;
        }

        public void StartAutoMerge()
        {
            _stateMachine.ChangeState(GameState.AutoMerging);
        }

        public void FinishAutoMerge()
        {
            if (_cubeController.CurrentCube == null)
                _cubeController.Spawn();

            _stateMachine.ChangeState(GameState.WaitingForInput);
        }

        private void BuildStateMachine()
        {
            _gameOverDetector = new GameOverDetector(_cubeController, _boardService);

            _stateMachine = new GameStateMachine();
            _stateMachine.AddListener(this);
            _stateMachine.AddListener(_uiController);

            _stateMachine.AddState(GameState.WaitingForInput,
                new WaitingForInputState(_inputService));
            _stateMachine.AddState(GameState.Dragging,
                new DraggingState(_cubeController, _inputService, _launchService, _boardService, _settings));
            _stateMachine.AddState(GameState.Launched,
                new LaunchedState(_cubeController, _gameOverDetector));
            _stateMachine.AddState(GameState.AutoMerging,
                new AutoMergingState());
            _stateMachine.AddState(GameState.GameOver,
                new GameOverState());
        }
    }
}
