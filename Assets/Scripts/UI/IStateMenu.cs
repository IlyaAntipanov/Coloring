using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStateMenu
{
    public void StartState();
    public void EndState();
    public event Action<Type> SetState;
}
