using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //target is what the CustomEditor typeof parameter is. 'as MapGenerator' ensures it's the right class.
        MapGenerator map = target as MapGenerator;

        //Each frame, map.GenerateMap()
        map.GenerateMap();
    }
}
