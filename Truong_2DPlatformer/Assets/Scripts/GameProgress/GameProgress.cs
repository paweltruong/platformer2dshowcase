using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GameProgress : IGameProgress
{
    int currentGameSlotIndex = 0;

    IGameProgressStorageService gameProgressStorage;
    GameProgressData current;
    public GameProgressData Current => current;

    public GameProgress(IGameProgressStorageService gameProgressStorage)
    {
        this.gameProgressStorage = gameProgressStorage;
    }


    void Reset()
    {
        current = new GameProgressData();
        current.uid = System.Guid.NewGuid().ToString();
        current.createdDate = System.DateTime.Now.Ticks;
        current.isFreshSave = true;
    }


    public void LoadOrStartNew(IEnumerable<Savable> savables)
    {
        if (gameProgressStorage.IsSlotExists(currentGameSlotIndex))
        {
            current = gameProgressStorage.Load(currentGameSlotIndex);
            if (!current.isFreshSave)
                LoadToScene(savables);
            else

            //if save corrupted(empty progress, reset progress)
            if (current == null)
                Reset();

            current.isFreshSave = false;
        }
        else
        {
            ResetAndSave();
        }
    }
    /// <summary>
    /// reset save slot (overwrite old save)
    /// </summary>
    public void ResetAndSave()
    {
        Reset();
        Save(true);
    }

    public void Save(IEnumerable<Savable> savables)
    {
        Save(false, savables);
    }



    void Save(bool isFreshSsave, IEnumerable<Savable> savables = null)
    {
        Debug.Log("Saving...");
        if (isFreshSsave)
        {
            current.isFreshSave = true;
        }
        else
        {
            if (savables != null)
            {
                foreach (var savable in savables)
                    savable.SaveProgress(this);
            }
        }
        gameProgressStorage.Save(currentGameSlotIndex, Current);
        Debug.Log("Game was successfuly saved");
    }

    void LoadToScene(IEnumerable<Savable> savables)
    {
        if (savables != null)
        {
            foreach (var savable in savables)
                savable.LoadProgress(this);
        }
    }

}

