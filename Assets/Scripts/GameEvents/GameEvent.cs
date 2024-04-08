using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEvent<T>
{
    private event UnityAction<T> Action = (T arg) => { };

    public void Publish(T param)
    {
        Action?.Invoke(param);
    }

    public void Add(UnityAction<T> subscriber)
    {
        Action += subscriber;
    }

    public void Remove(UnityAction<T> subscriber)
    {
        Action -= subscriber;
    }
}