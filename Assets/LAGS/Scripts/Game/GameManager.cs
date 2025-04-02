using SombraStudios.Shared.Patterns.Creational.Singleton;
using UnityEngine;
using UnityEngine.Events;

namespace LAGS
{
    public class GameManager : PersistentSingleton<GameManager>
    {
        [Header("References")]
        [SerializeField] private GameManagerData _data;

        [Header("Events")]
        public UnityEvent LevelStarted = new();
        public UnityEvent LevelFinished = new();

        [Header("Debug")]
        [SerializeField] private LevelProgression _levelProgression;

        public GameManagerData Data { get => _data; }
        public LevelProgression LevelProgression { get => _levelProgression; }
    }

    public enum LevelProgression
    {
        NotStarted,
        InProgress,
        Finished
    }
}
