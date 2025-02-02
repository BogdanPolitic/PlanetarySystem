﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_4_listener : MonoBehaviour
{
    GenericButtonListener buttonListener;
    GenericButtonListener.ObjectCharacteristics characteristics;
    void Start()
    {
        buttonListener = GameObject.Find("ButtonListener").GetComponent<GenericButtonListener>();
    }

    void Update()
    {
        if (GenericButtonListener.locked)
            return;
        if (characteristics == null)
            characteristics = buttonListener.GetButtonCharacteristics(gameObject.name);
        buttonListener.RequestUpdate(characteristics.buttonHash, characteristics.currentTag, characteristics.box, characteristics.glowingText, characteristics.availableLevelOrNoLevelButton);
    }
}
