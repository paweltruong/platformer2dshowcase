using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// helper methods and extensions
/// </summary>
public static class Utils
{
    public static bool IsValidGuid(this string value)
    {
        return !String.IsNullOrEmpty(value) && value != System.Guid.Empty.ToString();
    }

    public static void PlayRandomSound(AudioSource audioSource, AudioClip[] clips)
    {
        if (audioSource != null && clips != null && clips.Length > 0)
        {
            var randomJumpSound = clips[UnityEngine.Random.Range(0, clips.Length)];
            audioSource.PlayOneShot(randomJumpSound);
        }
    }
}