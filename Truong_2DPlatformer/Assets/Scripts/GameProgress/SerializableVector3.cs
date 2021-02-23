using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public struct SerializableVector3
{
    public float x;
    public float y;
    public float z;

    public SerializableVector3(UnityEngine.Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }
    public UnityEngine.Vector3 ToVector3()
    {
        return new UnityEngine.Vector3(this.x, this.y, this.z);
    }

    public static bool operator ==(SerializableVector3 a, SerializableVector3 b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(SerializableVector3 a, SerializableVector3 b)
    {
        return a.x != b.x || a.y != b.y || a.z != b.z;
    }

    public static bool operator ==(SerializableVector3 a, UnityEngine.Vector3 b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(SerializableVector3 a, UnityEngine.Vector3 b)
    {
        return a.x != b.x || a.y != b.y || a.z != b.z;
    }

    public static bool operator ==(UnityEngine.Vector3 a, SerializableVector3 b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(UnityEngine.Vector3 a, SerializableVector3 b)
    {
        return a.x != b.x || a.y != b.y || a.z != b.z;
    }
}