using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// manages loading and starting new game/progress
/// </summary>
public class GameState : MonoBehaviour
{
    IGameProgress gameProgress;
    int currentGameSlot = 0;
    Savable[] savables;

    bool gameIsFinished;

    public IGameProgress Progress => gameProgress;

    private void Awake()
    {
        EnsureGameProgressIsSet();
    }

    void Start()
    {
        savables = FindObjectsOfType<Savable>(true);
        gameProgress.LoadOrStartNew(savables);
    }

    /// <summary>
    /// Create fresh new game
    /// </summary>
    public void Reset()
    {
        EnsureGameProgressIsSet();
        gameProgress.ResetAndSave();
        if (Application.isPlaying)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void EnsureGameProgressIsSet()
    {
        if (gameProgress == null)
            gameProgress = new GameProgress(new FileGameProgressStorageService());
    }

    void OnApplicationQuit()
    {
        Debug.LogWarning($"Application is exiting. Need to update progress, and save '{savables.Count()}' elements");
        gameProgress.Save(savables);
    }

    private void Update()
    {
        if (gameIsFinished && Input.anyKey)
        {
            Reset();
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
            Reset();
    }

    public void GameIsFinished()
    {
        gameIsFinished = true;
    }
}
