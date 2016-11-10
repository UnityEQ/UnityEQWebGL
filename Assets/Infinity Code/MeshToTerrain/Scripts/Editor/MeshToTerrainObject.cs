/*       INFINITY CODE 2013 - 2016         */
/*     http://www.infinity-code.com        */

using UnityEngine;

public class MeshToTerrainObject
{
    public readonly GameObject gameobject;
    public readonly int layer;

    public MeshToTerrainObject(GameObject gameObject)
    {
        gameobject = gameObject;
        layer = gameObject.layer;
    }
}