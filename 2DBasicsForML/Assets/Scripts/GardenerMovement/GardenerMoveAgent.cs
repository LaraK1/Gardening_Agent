using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TMPro;
using System;

public class GardenerMoveAgent : Agent
{   
    [Header ("agent rewards")]
    [SerializeField] float _borderReward = -0.1f;
    [SerializeField] float _plantReward = 0.1f;
    [SerializeField] float _plantFirstTouchReward = 1f;
    [SerializeField] float _allPlantsHappyReward = 10f;
    [SerializeField] float _toMuchTouchReward = 0f;



    [Header ("agent visuals")] 
    [SerializeField] bool _useText;
    [SerializeField] TMP_Text _rewardUI;
    [SerializeField] float _speed = 1f;
    [SerializeField] SpriteRenderer _rewardDot;
    [SerializeField] Color darkGreen;
    Animator _animator;


    [Header ("agent and the environment")]
    [SerializeField] string _tagBorder = "border";
    [SerializeField] string _tagPlant = "plant"; 
    [SerializeField] string _tagHappyPlant = "happyPlant";  
    [SerializeField] Collider2D _agentCollider;

    public event Action<bool> OnRestart;

    private void Awake() {
        _animator = GetComponent<Animator>();

        _agentCollider = GetComponent<Collider2D>();
        if(_agentCollider == null)
            Debug.LogWarning("Agents needs a collider to work.");
    }

    public override void OnEpisodeBegin()
    {
        _agentCollider.enabled = false;

        OnRestart?.Invoke(true); // restarts plants over plantmanager and plantmanager

        _agentCollider.enabled = true;
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = Mathf.Clamp(actions.ContinuousActions[0], -1f, 1f); // l r
        float moveY = Mathf.Clamp(actions.ContinuousActions[1], -1f, 1f); // u d

        transform.position += new Vector3(moveX, moveY, 0) * Time.deltaTime * _speed;
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        var tag = other.gameObject.tag;
        if(tag == _tagPlant || tag == _tagHappyPlant) // triggers plant and gets reward through unity actions from plant manager
        {   
            if(_animator != null)
                {
                    _animator.SetTrigger("Action");
                }

            Plant plant = other.gameObject.GetComponent<Plant>();
            if(plant != null)
                plant.IsTriggered();

        }
    }

    private void OnCollisionStay2D(Collision2D other) 
    {
        var tag = other.gameObject.tag;
        if (tag == _tagBorder)
        {
            AddReward(_borderReward);

            if(_useText)
                _rewardUI.text = GetCumulativeReward().ToString();

            if(_rewardDot != null)
                _rewardDot.color = Color.red;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    public void FirstTouchReward()
    {                
        AddReward(_plantFirstTouchReward);

        if(_rewardDot != null)
            _rewardDot.color = Color.green;

        if(_useText)
            _rewardUI.text = GetCumulativeReward().ToString();
    }

    public void AllHappy()
    {
        AddReward(_allPlantsHappyReward);

        if(_useText)
            _rewardUI.text = GetCumulativeReward().ToString();

        if(_rewardDot != null)
            _rewardDot.color = Color.yellow;

        EndEpisode();
    }

    public void OnToMuchTuch() // TODO try to use negative rewards here
    {
        AddReward(_toMuchTouchReward);

        if(_useText)
            _rewardUI.text = GetCumulativeReward().ToString();

        if(_rewardDot != null)
            _rewardDot.color = Color.red;
    }

    public void OnGrowTouch()
    {
        AddReward(_plantReward);
        
        if(_rewardDot != null)
            _rewardDot.color = darkGreen;

        if(_useText)
            _rewardUI.text = GetCumulativeReward().ToString();
    }
}
