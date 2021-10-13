using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("movement/terrainTextureManager")]
public class TerrainTextureManager : MonoBehaviour
{
    public Transform playerTransform;
    private Terrain t;

    private int posX;
    private int posZ;
    public float[] textureValues = { 0, 0, 0, 0 };

    void Update()
    {
        // For better performance, move this out of update 
        // and only call it when you need a footstep.
        t = GetClosestCurrentTerrain(playerTransform.position);
        GetTerrainTexture();
    }

    public void GetTerrainTexture()
    {
        ConvertPosition(playerTransform.position);
        CheckTexture();
    }

    void ConvertPosition(Vector3 playerPosition)
    {
        Vector3 terrainPosition = playerPosition - t.transform.position;

        Vector3 mapPosition = new Vector3
        (terrainPosition.x / t.terrainData.size.x, 0,
        terrainPosition.z / t.terrainData.size.z);

        float xCoord = mapPosition.x * t.terrainData.alphamapWidth;
        float zCoord = mapPosition.z * t.terrainData.alphamapHeight;

        posX = (int)xCoord;
        posZ = (int)zCoord;
    }

    void CheckTexture()
    {
        if (posX >= 0 && posZ >= 0)
        {
            if (posX <= t.terrainData.alphamapWidth - 1 && posZ <= t.terrainData.alphamapHeight - 1)
            {
                float[,,] aMap = t.terrainData.GetAlphamaps(posX, posZ, 1, 1);
                textureValues = new float[aMap.Length];
                if (aMap.Length != 0)
                {
                    for (int i = 0; i < aMap.Length; i++)
                    {
                        textureValues[i] = aMap[0, 0, i];
                    }
                }
            }
        }
        }

        Terrain GetClosestCurrentTerrain(Vector3 playerPos)
    {
        //Get all terrain
        Terrain[] terrains = Terrain.activeTerrains;

        //Make sure that terrains length is ok
        if (terrains.Length == 0)
            return null;

        //If just one, return that one terrain
        if (terrains.Length == 1)
            return terrains[0];

        //Get the closest one to the player
        float lowDist = float.MaxValue;
        var terrainIndex = 0;

        for (int i = 0; i < terrains.Length; i++)
        {
            Terrain terrain = terrains[i];
            Vector3 terrainPos = GetTerainCenter(terrain);
            Vector3 comparePlayerPos = new Vector3(playerPos.x, 0, playerPos.z);

            //Find the distance and check if it is lower than the last one then store it
            var dist = Vector3.Distance(terrainPos, comparePlayerPos);
            if (dist < lowDist)
            {
                lowDist = dist;
                terrainIndex = i;
            }
        }
        return terrains[terrainIndex];
    }

    Vector3 GetTerainCenter(Terrain terrain)
    {
        Vector3 offset = new Vector3(terrain.terrainData.size.x / 2, 0, terrain.terrainData.size.z / 2);
        Vector3 terrainPos = terrain.GetPosition();
        return new Vector3(terrainPos.x, 0, terrainPos.z) + offset;
    }
}



