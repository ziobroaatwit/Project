using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    Rigidbody rBody;
    public float forceMultiplier = 10;
    public Transform Target;
    private Vector3 startingPositionAgent;
    private Vector3 startingPositionTarget;
    public float episodeDuration = 60f;
    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        startingPositionAgent = this.transform.localPosition;
        startingPositionTarget = Target.localPosition;

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(rBody.velocity.x);
        sensor.AddObservation(rBody.velocity.z);
        sensor.AddObservation(rBody.velocity.y);
        sensor.AddObservation(rBody.rotation.x);
        sensor.AddObservation(rBody.rotation.y);
        sensor.AddObservation(rBody.rotation.z);

    }
    /*
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }
    */
    public override void OnEpisodeBegin()
    {
        if (this.transform.localPosition.y > 0)
        {
            Target.localPosition = startingPositionTarget;
            this.transform.localPosition = startingPositionAgent;
            StartCoroutine(EpisodeTimer());
        }

    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);
        // Rewards
        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            Debug.Log("EPISODE SHOULD HAVE ENDED+1");
            SetReward(1.0f);
            EndEpisode();
        }
    }
    private IEnumerator EpisodeTimer()
    {
        float timer = 0f;

        while (timer < episodeDuration)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        Debug.Log("EPISODE SHOULD HAVE ENDED-1");
        SetReward(-1.0f);
        EndEpisode();
    }
}
