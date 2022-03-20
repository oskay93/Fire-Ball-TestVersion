﻿//This class is on the main camera to follow player.
//You may optimize it on SetPosition section and
//Write a proper way to update blocks positions on the map to make it an infite gameplay.

using UnityEngine;

public class PlayerFollower : MonoBehaviour {

    private Transform player;
    private float zDifference;
    private float yDifference;

    [SerializeField] Vector3 offset;
    Vector3 smoothedPos;

    public void SetPosition(Transform p)
    {
        //Optimize this portion
        player = p;
        //zDifference = player.transform.position.z - transform.position.z;
        //yDifference = player.transform.position.y - transform.position.y;

        Vector3 desiredPos = player.position + offset;
        smoothedPos = Vector3.Lerp(transform.position, desiredPos, 10);
    }

    int lastPassageIndex = -1;
    private void Update()
    {
        SetPosition(PlayerController.Instance.transform);
        if (player == null) return; // If No Player Found, Don't Continue Update
        transform.position = smoothedPos;
        /*if (player != null)
        {
            transform.position = new Vector3(transform.position.x, player.position.y - yDifference, player.position.z - zDifference);
            //BlockCreator.GetSingleton().UpdateBlockPosition(passageIndex); //You may call update block position here to make it an infinite map.
            //Hint:
            //It must be called when it is really needed in a very optimized way.
        }*/
    }
}