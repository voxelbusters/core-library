using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VoxelBusters.CoreLibrary
{
    public enum UnityEventType
    {
        Start = 1,

        Destroy,

        OnEnable,

        OnDisable,

        Update,

        TriggerEnter,

        TriggerExit,

        CollisionEnter,

        CollisionExit,

        TriggerEnter2D,

        TriggerExit2D,

        CollisionEnter2D,

        CollisionExit2D,

        Custom,
    }
}