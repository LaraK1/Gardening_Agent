using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
public class PlantManager : MonoBehaviour
{
    [SerializeField] List<Plant> _plants = new List<Plant>();
    [SerializeField] UnityEvent OnAllPlantsHappyEvent; // using unity events to see all events of the plants in inspector
    [SerializeField] UnityEvent OnFirstTouchedEvent;
    [SerializeField] UnityEvent GrowTouchEvent;
    [SerializeField] UnityEvent BadTouch;

    [SerializeField] List<Plant> _plantsRemaining;

    [SerializeField] GardenerMoveAgent agent;
    private void OnEnable() 
    {
        foreach (var plant in _plants)
        {
            plant.OnIsHappy += OnHappy;
            plant.OnTouchedForTheVeryFirstTime += OnFirstTouch;
            plant.OnTouchedToMuch += OnTouchToMuch;
            plant.OnGrow += OnGrowTouch;
        }

        agent.OnRestart += Restart;
    }

    private void OnHappy(Plant plant)
    {
        _plantsRemaining.Remove(plant);
        //plant.OnIsHappy -= OnHappy;

        if(_plantsRemaining.Count == 0)
        {
            OnAllPlantsHappyEvent.Invoke();
        }
    }

    private void OnFirstTouch(bool fristTouch){
        if(fristTouch)
            OnFirstTouchedEvent.Invoke();
    }
    private void OnGrowTouch(bool touch){
        if(touch)
            GrowTouchEvent.Invoke();
    }

    private void OnTouchToMuch(bool touch){
        if(touch)
            BadTouch.Invoke();
    }

    private void Restart(bool isRestart)
    {
        if(isRestart)
        {
            /*
            foreach (var plant in _plantsRemaining)
            {

                plant.OnIsHappy -= OnHappy;
            }

            _plantsRemaining = new List<Plant>(_plants);

            foreach (var plant in _plantsRemaining)
            {

                plant.Restart();
                plant.OnIsHappy += OnHappy;
            }

            */

            _plantsRemaining = new List<Plant>(_plants);
            foreach (var plant in _plants)
            {
                plant.Restart();
            }
        }
    }    
    
    [ContextMenu("AutoFill Plants")]
    private void AutoFillPlants()
    {
        _plants = GetComponentsInChildren<Plant>()
            .ToList();

        //.Where(t=> t.name.ToLower().Contains("red"))
    }
}
