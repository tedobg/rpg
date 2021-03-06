﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {

    [SerializeField]
    private float baseValue;
    private List<float> modifiers;

    public Stat()
    {
        Debug.Log("BLIP");
        modifiers = new List<float>();
    }

    public float GetValue()
    {
        float finalValue = baseValue;

        modifiers.ForEach(modifier => finalValue += modifier);

        return finalValue;
    }

    public float GetBase()
    {
        return baseValue;
    }

    public void AddModifier(float modifier)
    {
        if(modifier != 0)
        {
            modifiers.Add(modifier);
        }
    }

    public void RemoveModifier(float modifier)
    {
        if (modifier != 0)
        {
            modifiers.Remove(modifier);
        }
    }
}
