using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// multiple slots not yet supported
/// </summary>
public interface IGameProgressStorageService
{
    void Save(int slotIndex, GameProgressData data);
    GameProgressData Load(int slotIndex);
    bool IsSlotExists(int slotIndex);
}
