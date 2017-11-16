using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine;
namespace PrefabGenerate
{
    public abstract class PrefabModify: ScriptableObject
    {
       public abstract void ModifyPrefab(ObjectNode node);
    }
}