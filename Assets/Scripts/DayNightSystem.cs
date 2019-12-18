using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightSystem : MonoBehaviour, ISaveableLoadable
{
    //Day length in hours
    private static float DayLength = 0.5f;

    //TODO: change to transform.rotation
    private static float Rotation = 0f;

    private static bool speedskipping = false;
    private static float speedskippingincrement = 50f;

    //Rotation is transform.rotation.x
    void Update()
    {
        if (transform.rotation.eulerAngles.x > 0 && transform.rotation.eulerAngles.x < 90)
            speedskipping = false;

        //1 degree per second = 360 in 360 seconds = 6 minutes.
        //24 h day by dividing with 1440 / 60 = 24.
        var increment = Time.deltaTime / DayLength;

        if (speedskipping)
            increment *= speedskippingincrement;

        transform.Rotate(Vector3.right * increment);
        Rotation = transform.rotation.eulerAngles.x;
    }

    public static float GetRotation()
    {
        return Rotation;
    }

    public static void SkipToMorning()
    {
        speedskipping = true;
    }

    public SerializableData GetData()
    {
        return new SunData(transform, Rotation);
    }

    public void LoadData(SerializableData data)
    {
        var temp = (SunData)data;
        transform.rotation = temp.Rotation.AsQuaternion();
        Rotation = transform.rotation.eulerAngles.x;
    }

    public void LoadDefault()
    {
        //Empty
    }

    [System.Serializable]
    private class SunData : SerializableData
    {
        public readonly Rotation Rotation;
        public readonly float rotation;

        public SunData(Transform trans, float rotation)
        {
            this.Rotation = new Rotation(trans.rotation);
            this.rotation = rotation;
        }
    }
}
