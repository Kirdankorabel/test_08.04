using System;
using Game2048.Infrastructure.Core;
using UnityEngine;
using Zenject;

namespace Game2048.Gameplay.Scores
{
    public class ScoreService : IDisposable
    {
        private const string BestScoreKey = "BestScore";

        private readonly SignalBus _signalBus;
        private int _currentScore;

        public event Action<int> OnScoreChanged;

        [Inject]
        public ScoreService(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<MergeCompletedSignal>(OnMergeCompleted);
        }

        public int CurrentScore => _currentScore;
        public int BestScore => PlayerPrefs.GetInt(BestScoreKey, 0);

        public void Dispose()
        {
            _signalBus.TryUnsubscribe<MergeCompletedSignal>(OnMergeCompleted);
        }

        public void AddScore(int amount)
        {
            _currentScore += amount;
            OnScoreChanged?.Invoke(_currentScore);
        }

        public void SaveBestScore()
        {
            if (_currentScore > BestScore)
            {
                PlayerPrefs.SetInt(BestScoreKey, _currentScore);
                PlayerPrefs.Save();
            }
        }

        public void Reset()
        {
            _currentScore = 0;
            OnScoreChanged?.Invoke(0);
        }

        private void OnMergeCompleted(MergeCompletedSignal signal)
        {
            AddScore(signal.ScoreReward);
        }
    }
}
