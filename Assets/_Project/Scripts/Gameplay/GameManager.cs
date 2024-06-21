using CargoMover;
using UnityEngine;
using VContainer;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayArea _exampleArea;
    [SerializeField] private Transform _storePoint;


    private int _level;
    private LevelBuilder _levelBuilder;

    [Inject]
    public void Construct(LevelBuilder levelBuilder)
    {
        _levelBuilder = levelBuilder;
    }

    private void OnEnable()
    {
        NetworkUI.OnStart += StartLevel;
    }

    private void OnDisable()
    {
        NetworkUI.OnStart -= StartLevel;
    }

    private void StartLevel()
    {
        _level = LevelGenerator.GenerateLevel();
        _levelBuilder.BuildLevel(_level, _exampleArea, _storePoint);
    }
}