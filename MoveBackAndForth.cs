using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class MoveBackAndForth : MonoBehaviour
{
    public int secondLocation;
    private Vector3 startPosition;
    private Vector3 secondPosition;



    private void Start()
    {
        startPosition = transform.position;
        secondPosition = new Vector3(startPosition.x + secondLocation, startPosition.y, startPosition.z);
        MoveTransformRight();
    }

    private void MoveTransformRight()
    {
        transform.DOMove(secondPosition, 2).SetEase(Ease.InOutSine).OnComplete(MoveTransformLeft);
    }
    private void MoveTransformLeft()
    {
        transform.DOMove(startPosition, 2).SetEase(Ease.InOutSine).OnComplete(MoveTransformRight);
    }
}
