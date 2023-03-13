﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]//Para mostrar al personaje en el inspector.
public struct Character
{
    public Sprite image;
    public string name;
    [Range(0, 100)] public float speed;
    [Range(0, 100)] public float power;
    public int price;

    public bool isPurchased;

}
