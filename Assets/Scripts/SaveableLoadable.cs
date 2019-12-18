using System.Collections.Generic;
using UnityEngine;

class SaveableLoadable : MonoBehaviour
{
    private ISaveableLoadable[] SaveableComponents;

    private void Awake() => SaveableComponents = GetComponents<ISaveableLoadable>();

    public ISaveableLoadable[] GetSaveableComponents() => GetComponents<ISaveableLoadable>();

    public void Load(Dictionary<string, SerializableData> objectsData)
    {
        foreach (var comp in SaveableComponents)
        {
            if (objectsData.TryGetValue(comp.GetType().Name, out SerializableData data))
                comp.LoadData(data);
        }
    }

    public void LoadDefault()
    {
        foreach (var comp in SaveableComponents)
        {
            comp.LoadDefault();
        }
    }
}