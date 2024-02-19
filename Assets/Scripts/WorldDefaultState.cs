using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDefaultState : MonoBehaviour
{
    public List<WorldData> worlds;
    private void Start ()
    {
        LevelManager.Instance.SetWorldsDefaultState(this);
    }
}
