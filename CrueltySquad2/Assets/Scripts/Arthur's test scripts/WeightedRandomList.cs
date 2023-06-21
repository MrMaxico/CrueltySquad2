using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightedRandomList<Gameobject> {
    [System.Serializable]
    public struct Pair {
        public Gameobject item;
        public float weight;

        public Pair(Gameobject item, float weight) {
            this.item = item;
            this.weight = weight;
        }
    }

    public List<Pair> list = new List<Pair>();

    public int Count {
        get => list.Count;
    }

    public void Add(Gameobject item, float weight) {
        list.Add(new Pair(item, weight));
    }

    public Gameobject GetRandom() {
        float totalWeight = 0;

        foreach (Pair p in list) {
            totalWeight += p.weight;
        }

        float value = Random.value * totalWeight;

        float sumWeight = 0;

        foreach (Pair p in list) {
            sumWeight += p.weight;

            if (sumWeight >= value) {
                return p.item;
            }
        }

        return default(Gameobject);
    }
}