using FishNet.Connection;
using FishNet.Managing;
using FishNet.Object;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace FishNet.Component.Spawning
{
    [AddComponentMenu("FishNet/Component/PlayerSpawner")]

    public class PlayerSpawner : MonoBehaviour
    {
        public event Action<NetworkObject> OnSpawned;
        private NetworkConnection _currentConnection;


        [Tooltip("Prefab to spawn for the player.")]
        [SerializeField]
        private NetworkObject _playerPrefab;
        private NetworkObject _currentPlayer;

        [Tooltip("True to add player to the active scene when no global scenes are specified through the SceneManager.")]
        [SerializeField]
        private bool _addToDefaultScene = true;

        [Tooltip("Areas in which players may spawn.")]
        [FormerlySerializedAs("_spawns")]
        public Transform[] Spawns = new Transform[0];

        private NetworkManager _networkManager;
        private int _nextSpawn;
      
        

        private void Start()
        {
            InitializeOnce();
        }

        private void OnDestroy()
        {
            if (_networkManager != null)
                _networkManager.SceneManager.OnClientLoadedStartScenes -= SceneManager_OnClientLoadedStartScenes;
        }

        private void InitializeOnce()
        {
            _networkManager = InstanceFinder.NetworkManager;
            if (_networkManager == null)
            {
                Debug.LogWarning($"PlayerSpawner on {gameObject.name} cannot work as NetworkManager wasn't found on this object or within parent objects.");
                return;
            }

            _networkManager.SceneManager.OnClientLoadedStartScenes += SceneManager_OnClientLoadedStartScenes;
        }

        private void SceneManager_OnClientLoadedStartScenes(NetworkConnection conn, bool asServer)
        {
            _currentConnection = conn;

            if (!asServer)
                return;
            if (_playerPrefab == null)
            {
                Debug.LogWarning($"Player prefab is empty and cannot be spawned for connection {conn.ClientId}.");
                return;
            }

            Vector3 position;
            Quaternion rotation;
            SetSpawn(_playerPrefab.transform, out position, out rotation);

            NetworkObject nob = _networkManager.GetPooledInstantiated(_playerPrefab, _playerPrefab.SpawnableCollectionId, true);
            nob.transform.SetPositionAndRotation(position, rotation);
            _networkManager.ServerManager.Spawn(nob, conn);

            Vector3 spherePosition = nob.transform.position + nob.transform.forward;
            Quaternion sphereRotation = Quaternion.identity;

          

            if (_addToDefaultScene)
                _networkManager.SceneManager.AddOwnerToDefaultScene(nob);

            OnSpawned?.Invoke(nob);
        }

        private void SetSpawn(Transform prefab, out Vector3 pos, out Quaternion rot)
        {
            if (Spawns.Length == 0)
            {
                SetSpawnUsingPrefab(prefab, out pos, out rot);
                return;
            }

            Transform result = Spawns[_nextSpawn];
            if (result == null)
            {
                SetSpawnUsingPrefab(prefab, out pos, out rot);
            }
            else
            {
                pos = result.position;
                rot = result.rotation;
            }

            _nextSpawn++;
            if (_nextSpawn >= Spawns.Length)
                _nextSpawn = 0;
        }

        private void SetSpawnUsingPrefab(Transform prefab, out Vector3 pos, out Quaternion rot)
        {
            pos = prefab.position;
            rot = prefab.rotation;
        }
    }
}
