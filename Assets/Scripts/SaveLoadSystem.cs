using System;
using UnityEngine;

//Implemented in components with saveable state
public interface ISaveableLoadable
{
    SerializableData GetData();

    void LoadData(SerializableData data);

    void LoadDefault();
}

[Serializable]
public abstract class SerializableData
{
    protected SerializableData() { }
}

[Serializable]
public readonly struct Position
{
    public readonly float X, Y, Z;

    public Position(Vector3 position)
    {
        this.X = position.x;
        this.Y = position.y;
        this.Z = position.z;
    }

    public Vector3 AsVector3() => new Vector3(X, Y, Z);
}

[Serializable]
public readonly struct Rotation
{
    public readonly float X, Y, Z, W;

    public Rotation(Quaternion quat)
    {
        this.X = quat.x;
        this.Y = quat.y;
        this.Z = quat.z;
        this.W = quat.w;
    }

    public Quaternion AsQuaternion() => new Quaternion(X, Y, Z, W);
}