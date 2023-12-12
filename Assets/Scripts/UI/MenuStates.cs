using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class MenuStates : MonoBehaviour
{
    public List<GameObject> GameObjectStatesMenu = new List<GameObject>();
    public List<IStateMenu> StatesMenu;
    public int StateActive
    {
        get => stateActive; set
        {
            StatesMenu[stateActive].EndState();
            stateActive = value;
            StatesMenu[stateActive].StartState();
        }
    }
    private int stateActive;

    public void StatesMenuGeneration()
    {
        StatesMenu = GameObjectStatesMenu.Select(s => s.GetComponent<IStateMenu>()).ToList();
        for (int i = 0; i < StatesMenu.Count; i++)
        {
            StatesMenu[i].SetState += (state) =>
            {
                StateActive = StatesMenu.FindIndex(f => f.GetType() == state);
            };
        }
        StatesMenu[stateActive].StartState();
    }
    void Start()
    {
        StatesMenuGeneration();
    }
}
