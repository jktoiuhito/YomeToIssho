using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour, ISaveableLoadable
{
    [SerializeField] private GameObject Flower;
    [SerializeField] private float spawnchance = 0.001f;
    [SerializeField] private int spawnlimit = 10;
    private List<GameObject> Flowers = new List<GameObject>();

    public SerializableData GetData()
    {
        return new FlowerSpawnerData(Flowers);
    }

    public void LoadData(SerializableData data)
    {
        Flowers.ForEach(f => Destroy(f.gameObject));

        var temp = (FlowerSpawnerData)data;
        temp.Positions.ForEach(p => {
            var f = Instantiate(Flower);
            f.transform.position = p.AsVector3();
            Flowers.Add(f);
        });
    }

    void Update()
    {
        if (Random.value < spawnchance && Flowers.Count < spawnlimit)
        {
            //Happens rarely, and only for a little amount of flowers.
            Flowers.ForEach(s => { if (s == null) Flowers.Remove(s); });

            var x = Random.Range(370f, 435f);
            var z = Random.Range(-5f, 58f);
            if (x > 392 && x < 412) return;
            if (z > 15 && z < 35) return;
            var flow = Instantiate(Flower);
            Flowers.Add(flow);
            flow.transform.position = new Vector3(x, 35.89f, z);
        }
    }

    public void LoadDefault()
    {
        //Empty
    }

    [System.Serializable]
    private class FlowerSpawnerData : SerializableData
    {
        public readonly List<Position> Positions;

        public FlowerSpawnerData(List<GameObject> flowers)
        {
            this.Positions = new List<Position>();
            flowers.FindAll(f => f != null).ForEach(f => Positions.Add(new Position(f.transform.position)));
        }
    }
}
