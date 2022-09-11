using UnityEngine;

namespace RSM
{
    public class ObjectSpawner : MonoBehaviour
    {
        public GameObject spawnable;

        public void SpawnObject()
            => Instantiate(spawnable, transform.position, Quaternion.identity);
    }
}