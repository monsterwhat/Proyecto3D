using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class prefabSpawner : MonoBehaviour
{
    public GameObject prefab; // The prefab to spawn
    public TwoBoneIKConstraint RightHandikTarget;
    public TwoBoneIKConstraint LeftHandikTarget;
    public MultiParentConstraint WeaponPose;
    private GameObject newObject; // The spawned prefab
    private Animator animator;

    public void SpawnPrefab()
    {
        // Spawn the prefab
        newObject = Instantiate(prefab, transform);
        // Position it properly.
        newObject.transform.position = transform.position;
    }

    void updateWeaponPose()
    {
        if (WeaponPose != null)
        {
            WeaponPose.data.constrainedObject = newObject.transform;
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        SpawnPrefab();
    }

    void Update()
    {
        if ( newObject != null )
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
            UpdateHands();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            UpdateHands();
            LateUpdate();
            updateWeaponPose();
        }

    }

    void UpdateHands()
    {
        // Update the IK target position
        var rightHand = newObject.transform.Find("RightHandPosition");
        RightHandikTarget.data.target = rightHand;
        var leftHand = newObject.transform.Find("LeftHandPosition");
        LeftHandikTarget.data.target = leftHand;

    }

    private void LateUpdate()
    {
        UpdateHands();
        updateWeaponPose();
    }

}