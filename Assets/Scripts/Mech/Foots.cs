using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foots : MonoBehaviour
{
    public Animator MechAnimator;
    public string SelectedMoveType = "walk";

    private string[] _movementKeys = new string[] { "walk", "fastwalk", "run" };
    void Start()
    {
        foreach (string key in _movementKeys)
        {
            MechAnimator.SetBool(key, false);

        }

        MechAnimator.SetBool(SelectedMoveType, true);
    }
}
