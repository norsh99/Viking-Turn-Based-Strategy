using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject cannonBall;
    [SerializeField] private GameObject waterSplash;
    [SerializeField] private ParticleSystem smokeTrail;




    private Vector3 pointB;
    private float velocity;
    private float speed;
    private float dist;

    private Transform pointA;
    private Vector3 pointC;

    private Vector3 dotA;
    private Vector3 dotB;

    private bool flightOver;

    private Player playerShootingThisCannonBall;
    [OdinSerialize, ReadOnly] int timer = 0;
    public void CannonLaunch(Transform launchTransform, Vector3 destinationTransfrom, Player player)
    {

        flightOver = false;
        this.playerShootingThisCannonBall = player;

        pointA = launchTransform;
        pointC = destinationTransfrom;
        pointB = (launchTransform.position + destinationTransfrom) / 2;
        dist = Vector3.Distance(pointA.position, pointC);
        pointB.y += dist/2;
        speed = 50f / dist;
        this.transform.position = launchTransform.position;
    }

    private void Update()
    {
        timer++;
        if (!flightOver)
        {
            velocity = velocity + (Time.deltaTime * speed);
            dotA = Vector3.Lerp(pointA.position, pointB, velocity);
            dotB = Vector3.Lerp(pointB, pointC, velocity);

            this.transform.position = Vector3.Lerp(dotA, dotB, velocity);
        }

        if (timer > 500)
        {

            DestroyCannonBall();
        }
    }
    private void DestroyCannonBall()
    {
        Destroy(gameObject);

    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("ahhhhhhhhh");
        Unit unit = collider.GetComponent<Unit>();
        if (unit != null && playerShootingThisCannonBall != unit.playerOwner)
        {
            unit.Damage();
            transform.DOScale(transform.localScale, 2).OnComplete(DestroyCannonBall);
            cannonBall.SetActive(false);
            explosion.SetActive(true);

        }


        else if (collider.GetComponent<Hex>() != null)
        {
            if (collider.GetComponent<Hex>().terrainType == TerrainType.Water && timer > 5)
            {
                flightOver = true;
                rigidbody.isKinematic = false;
                rigidbody.useGravity = true;
                waterSplash.SetActive(true);
                smokeTrail.Stop();
                transform.DOScale(transform.localScale, 3).OnComplete(DestroyCannonBall);
            }
        }
    }
}
