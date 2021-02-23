using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IGameProgress
{
    GameProgressData Current { get; }
    /// <summary>
    /// load if save exits else start new game
    /// </summary>
    /// <param name="savables"></param>
    void LoadOrStartNew(IEnumerable<Savable> savables);
    /// <summary>
    /// reset save slot (overwrite old save)
    /// </summary>
    void ResetAndSave();
    /// <summary>
    /// save current progress
    /// </summary>
    /// <param name="savables"></param>
    void Save(IEnumerable<Savable> savables);
}

