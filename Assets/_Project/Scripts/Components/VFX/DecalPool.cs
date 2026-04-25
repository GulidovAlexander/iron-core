using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Components.VFX
{
    public class DecalPool : MonoBehaviour
    {
        public static DecalPool Instance { get; private set; }

        [SerializeField] private GameObject _decalPrefab;
        [SerializeField] private int _poolSize = 50;

        private Queue<GameObject> _pool = new Queue<GameObject>();
        private int _activeCount = 0;

        private void Awake()
        {
            Instance = this;

            // заполняем пул при старте
            for (int i = 0; i < _poolSize; i++)
            {
                var obj = Instantiate(_decalPrefab, transform);
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public GameObject Get(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject decal;

            if (_pool.Count > 0)
            {
                decal = _pool.Dequeue();
            }
            else
            {
                decal = _activeDecals.Dequeue();
                decal.transform.SetParent(transform);
            }

            decal.transform.position = position + rotation * Vector3.forward * 0.02f;
            decal.transform.rotation = rotation;

            if (parent != null)
                decal.transform.SetParent(parent);
            else
                decal.transform.SetParent(transform);

            decal.SetActive(true);
            _activeDecals.Enqueue(decal);

            return decal;
        }

        public void Return(GameObject decal)
        {
            decal.SetActive(false);
            decal.transform.SetParent(transform);
            _pool.Enqueue(decal);
        }

        private Queue<GameObject> _activeDecals = new Queue<GameObject>();
    }
}
