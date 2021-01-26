using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveVelocity : MonoBehaviour
{

    [SerializeField] private float moveSpeed;

    [OdinSerialize, ReadOnly] private Vector3 velocityVector;
    [SerializeField] private Rigidbody rigidbody;


    public void SetVelocity(Vector3 velocityVector)
    {
        this.velocityVector = velocityVector;
    }

    private void FixedUpdate()
    {
        //transform.position += velocityVector * moveSpeed * Time.deltaTime;
        rigidbody.velocity = velocityVector * moveSpeed;
    }

}
