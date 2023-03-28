using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class prefabSpawner : MonoBehaviour
{
    public GameObject prefab; // The prefab to spawn
    public TwoBoneIKConstraint RightHandikTarget;
    public TwoBoneIKConstraint LeftHandikTarget;
    private GameObject newObject; // The spawned prefab

    public void SpawnPrefab()
    {
        // Instantiate the prefab as a child of this GameObject
        newObject = Instantiate(prefab, transform);
    }

    private void Start()
    {
        SpawnPrefab();
    }

    void Update()
    {


    }

    private void LateUpdate()
    {
        // Update the IK target position
        var rightHand = newObject.transform.Find("RightHandPosition");
        RightHandikTarget.data.target = rightHand;
        var leftHand = newObject.transform.Find("LeftHandPosition");
        LeftHandikTarget.data.target = leftHand;
    }

}