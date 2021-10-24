using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Plant : MonoBehaviour
{
    int _maxPlantContacts = 4;
    [SerializeField] int _currentPlantContacts = 0;

    // visuals
    [SerializeField] private SpriteRenderer _plantRenderer;
    
    [SerializeField] private Sprite[] _plantSprites;
    [SerializeField] private SpriteRenderer _boxRenderer;
    [SerializeField] private SpriteRenderer _boxRenderer2;
    

    public event Action<Plant> OnIsHappy;
    public event Action<bool> OnTouchedForTheVeryFirstTime; // TODO find Actions without Parameter? delegate
    public event Action<bool> OnTouchedToMuch;
    public event Action<bool> OnGrow; 

    [SerializeField] string _plantTag = "plant";
    [SerializeField] string _happyPlantTag = "happyPlant";

    private void Awake() {

        _maxPlantContacts = _plantSprites.Length-1;
    }
    public void IsTriggered()
    {
        if(_currentPlantContacts < _maxPlantContacts)
        {   
            _currentPlantContacts++;
            _plantRenderer.sprite = _plantSprites[_currentPlantContacts];
            
            if(_currentPlantContacts == 1) // plant has been found by agent and is happy to have found new friends
                OnTouchedForTheVeryFirstTime?.Invoke(true);
            else // plant has been revisited and is not jet full grown; is happy about the agents help
                OnGrow?.Invoke(true);

            if(_currentPlantContacts == _maxPlantContacts) // pant has grown to full size and is happy
            {
                _boxRenderer.color = Color.green;
                _boxRenderer2.color = Color.green;
                this.tag = _happyPlantTag;
                OnIsHappy?.Invoke(this);
            }
        } else // plant has already fully grown and is not happy that the agent is still clingy
        {
                OnTouchedToMuch?.Invoke(true);
        }
    }

    public void Restart()
    {
        this.tag = _plantTag;
        _boxRenderer.color = Color.white;
        _boxRenderer2.color = Color.white;

        _currentPlantContacts = 0;
        _plantRenderer.sprite = _plantSprites[_currentPlantContacts];
    }
}
