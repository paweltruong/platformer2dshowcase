using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// with really simple mechanism to prevent manipulation of data 
/// TODO:more advanced security system f.e with public/private keys
/// TODO: base64 to WebEncoders?
/// </summary>
public class FileGameProgressStorageService : IGameProgressStorageService
{
    const char splitter = '.';
    string seed = "BORANmKJPYvttOvI";
    public string SaveDirectory => Application.persistentDataPath;
    bool testModeEnabled;

    public FileGameProgressStorageService()
    {
    }

    public FileGameProgressStorageService(bool testMode)
    {
        testModeEnabled = testMode;
    }
    
    public void Save(int slotIndex, GameProgressData data)
    {
        if (slotIndex < 0)
            throw new System.ArgumentException($"Invalid save file slot index: {slotIndex}");
        var path = GetSaveFilePath(slotIndex);

        File.WriteAllText(path, GetSignedData(data), Encoding.UTF8);
        //Debug.Log($"Data saved to '{path}'");
    }

    public GameProgressData Load(int slotIndex)
    {
        if (slotIndex < 0)
            throw new System.ArgumentException($"Invalid save file slot index: {slotIndex}");
        var path = GetSaveFilePath(slotIndex);
        if (!File.Exists(path))
            Debug.LogError($"Error loading file '{path}' not found");

        return GetDecodedDataFromFile(path);
    }

    public bool IsSlotExists(int slotIndex)
    {
        var path = GetSaveFilePath(slotIndex);
        if (!File.Exists(path))
            return false;
        return true;
    }

    /// <summary>
    /// get signed data in similar to JWT format {base64:content}.{base64:signature}
    /// </summary>
    /// <returns></returns>
    string GetSignedData(GameProgressData data)
    {
        string signedDate = string.Empty;
        var unsignedData = JsonUtility.ToJson(data);
        string signature = GetSignature(unsignedData);
        signedDate = Convert.ToBase64String(Encoding.UTF8.GetBytes(unsignedData)) + splitter + signature;
        return signedDate;
    }

    string GetSignature(string content)
    {
        string signature = string.Empty;
        using (var sha256 = SHA256.Create())
        {
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content + seed));
            var hashSb = new StringBuilder(128);
            foreach (var b in hash)
                hashSb.Append(b.ToString("X2"));
            signature = hashSb.ToString();
        }
        return signature;
    }

    GameProgressData GetDecodedDataFromFile(string path)
    {
        var encodedContent = File.ReadAllText(path);
        var parts = encodedContent.Split(splitter);
        if (parts.Count() != 2)
            Debug.LogError($"Save file '{path}' corrupted (format invalid)");
        var decodedContent = Encoding.UTF8.GetString(Convert.FromBase64String(parts[0]));
        var signature = GetSignature(decodedContent);
        //validate
        if (signature != parts[1])
            Debug.LogError($"Save file '{path}' corrupted (signature invalid)");
        //Debug.Log($"Loading:\r\n{decodedContent}");
        return JsonUtility.FromJson<GameProgressData>(decodedContent);
    }

    public string GetSaveFilePath(int saveSlotIndex)
    {
        if(testModeEnabled)
            return Path.Combine(SaveDirectory, $"SaveTest{saveSlotIndex}.dat");
        return Path.Combine(SaveDirectory, $"Save{saveSlotIndex}.dat");
    }
}
