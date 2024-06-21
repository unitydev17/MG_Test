using CargoMover;
using UnityEngine;
using VContainer;

public class Gameplay : MonoBehaviour
{
    [SerializeField] private PlayArea _exampleArea;
    [SerializeField] private PlayArea _playArea;
    [SerializeField] private Transform _storePoint;


    private int _level;
    private LevelBuilder _levelBuilder;

    [Inject]
    public void Construct(LevelBuilder levelBuilder)
    {
        _levelBuilder = levelBuilder;
    }

    private void Start()
    {
        _level = LevelGenerator.GenerateLevel();
        _levelBuilder.BuildLevel(_level, _exampleArea, _storePoint);
    }
}