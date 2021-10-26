using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]List<Vector3> _spawnPositions;
    List<Vector3> _openSpawnPositions;

    [SerializeField] List<GameObject> _plantPrefabs = new List<GameObject>();
    List<GameObject> _plants = new List<GameObject>();
    [SerializeField] GameObject _agent;
    [SerializeField] GameObject _plantParent;

    [SerializeField] PlantManager _plantManager;
    [SerializeField] int _maxPlants = 7;
    int _minPositions = 3;
    int _numberOfPositions;

    float _playerOffsetY = 0.2f;
    float _playerOffsetX = 0.5f;

    private void Awake() 
    {
        _numberOfPositions = _spawnPositions.Count;

        _maxPlants++;
        if(_maxPlants > _numberOfPositions)
        {
            Debug.LogWarning("There can not be more than " + (_numberOfPositions-1) + " plants.");
            _maxPlants = _numberOfPositions;
        } else if(_maxPlants < _minPositions)
        {
            Debug.LogWarning("There can not be less than " + (_minPositions-1) + " plants.");
            _maxPlants = _minPositions;
        }

        if(_plantPrefabs.Count == 0)
            Debug.LogWarning("There are no plant prefabs added to the list.");
            
        CreatePlants();

        GardenerMoveAgent gardenerMoveAgent = _agent.GetComponent<GardenerMoveAgent>();

        if(gardenerMoveAgent != null)
            gardenerMoveAgent.OnRestart += RepositionEnvironment;
        else
            Debug.LogWarning("Spawner could not find the Gardener Move Agent script.");
    }

    private List<Vector3> GetSpawnPositions()
    {
        _openSpawnPositions = new List<Vector3>(_spawnPositions);
        int positionsGiven = 0;
        positionsGiven = Random.Range(_minPositions,_maxPlants); // random number of objects, at least 2, for agent and two plant

        List<Vector3> newSpawnPositions = new List<Vector3>();

        for (int i = 0; i < positionsGiven; i++)
        {
            int randomIndex = Random.Range(0, _openSpawnPositions.Count);
            newSpawnPositions.Add(_openSpawnPositions[randomIndex]);
            _openSpawnPositions.RemoveAt(randomIndex);
        }

        return newSpawnPositions;
    }

    private void SpawnObjects()
    {
        List<Vector3> spawnPostions = new List<Vector3>(GetSpawnPositions());
        
        Vector3 agentPositon = spawnPostions[0];
        agentPositon.x += _playerOffsetX;
        agentPositon.y += _playerOffsetY;
        _agent.transform.localPosition = agentPositon;

        int numberOfPositions = spawnPostions.Count;
        for (int i = 1; i < numberOfPositions; i++)
        {
            _plants[i].transform.localPosition = spawnPostions[i];
            _plants[i].SetActive(true);
        }
    }

    public void RepositionEnvironment(bool restart)
    {
        if(restart)
        {
            foreach (var plant in _plants)
            {
                plant.SetActive(false);
            }

            SpawnObjects();
            _plantManager.Reset();
        }
    }

    private void CreatePlants()
    {
        int numberOfPlantPrefabs = _plantPrefabs.Count;
        int currentPrefabCount = 0;

        for (int i = 0; i < _numberOfPositions; i++)
        {
            var newObject = GameObject.Instantiate(_plantPrefabs[currentPrefabCount]);
            newObject.transform.parent = _plantParent.transform;
            newObject.gameObject.SetActive(false);
            _plants.Add(newObject);

            currentPrefabCount++;
            if (currentPrefabCount >= numberOfPlantPrefabs)
            {
                currentPrefabCount = 0;
            }
        }
    }
}
