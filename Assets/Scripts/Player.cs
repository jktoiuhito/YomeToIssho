using System.Collections;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

[RequireComponent(typeof(SaveableLoadable))]
public class Player : MonoBehaviour, ISaveableLoadable
{
    public SerializableData GetData()
    {
        //DEBUG
        Debug.Log($"Saving player: {transform.position.x}, {transform.position.y}, {transform.position.z}");

        return new PlayerData(transform);
    }

    public void LoadData(SerializableData data)
    {
        //DEBUG
        Debug.Log($"Loading player: {transform.position.x}, {transform.position.y}, {transform.position.z}");

        var temp = (PlayerData)data;
        transform.position = temp.Position.AsVector3();
        transform.rotation = temp.Rotation.AsQuaternion();

        //DEBUG
        Debug.Log($"Loaded player: {transform.position.x}, {transform.position.y}, {transform.position.z}");

        StartCoroutine(InitializeFPSContrller());
    }

    public void LoadDefault()
    {
        StartCoroutine(InitializeFPSContrller());
    }

    //As the FPS controller f*cks everything up on loading the character, we need to wait a bit before its initialization.
    //It absolutely not cannot be done on the same frame as the data loading.
    IEnumerator InitializeFPSContrller()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<FirstPersonController>().enabled = true;
    }

    [System.Serializable]
    private class PlayerData : SerializableData
    {
        public readonly Position Position;
        public readonly Rotation Rotation;

        public PlayerData(Transform trans)
        {
            this.Position = new Position(trans.position);
            this.Rotation = new Rotation(trans.rotation);
        }
    }
}
