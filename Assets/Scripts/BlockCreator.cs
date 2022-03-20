using System.Collections.Generic;
using UnityEngine;


//In this class, the map has been created.
//You have to edit GetRelativeBlock section to calculate current relative block to cast player rope to hold on
//Update Block Position section to make infinite map.
public class BlockCreator : MonoBehaviour {

    #region Variables
    private static BlockCreator singleton = null;
    private GameObject[] blockPrefabs;
    private GameObject pointPrefab;
    private GameObject pointObject;
    public int blockCount;

    List<GameObject> blockPool = new List<GameObject>();
    private float lastHeightUpperBlock = 10;
    private int difficulty = 1;

    Camera cam => Camera.main;

    int currentBlockIndex;
    #endregion

    public static BlockCreator GetSingleton()
    {
        if(singleton == null)
        {
            singleton = new GameObject("_BlockCreator").AddComponent<BlockCreator>();
        }
        return singleton;
    }

    public void Initialize(int bCount, GameObject[] bPrefabs, GameObject pPrefab)
    {
        blockCount = bCount;
        blockPrefabs = bPrefabs;
        pointPrefab = pPrefab;
        InstantiateBlocks();
    }
	
    public void InstantiateBlocks()
    {
        float zPos = 1;
        for (int i = 0; i < blockCount; i++)
        {
            #region Upper Block
            int randomPrefab = Random.Range(0, blockPrefabs.Length); // Get Random Prefab From The Array
            float randomPosY = Random.Range(10, 14); // Y Pos Spawn Limits
            Vector3 spawnPos = new Vector3(0, randomPosY, 0); // Set Y Position Of The Upper Block
            GameObject upperBlock = Instantiate(blockPrefabs[randomPrefab], spawnPos, Quaternion.identity);
            upperBlock.name = "UpperBlock";
            #endregion // Upper Block Ends

            #region Lower Block
            randomPosY = Random.Range(-10, -7); // Y Pos Spawn Limits
            spawnPos = new Vector3(0, randomPosY, 0); // Set Y Position Of The Lower Block
            GameObject lowerBlock = Instantiate(blockPrefabs[randomPrefab], spawnPos, Quaternion.identity);
            lowerBlock.name = "LowerBlock";
            #endregion // Lower Block Ends

            #region Parent
            GameObject parentPassage = new GameObject("Passage " + (i + 1));
            upperBlock.transform.parent = parentPassage.transform;
            lowerBlock.transform.parent = parentPassage.transform;
            parentPassage.transform.position = new Vector3(0, 0, zPos); // Set Parent In Correct Z-Axis
            blockPool.Add(parentPassage);
            zPos++;
            #endregion // Parent Ends
        }
    }

    void Update ()
    {
        if (PlayerController.Instance.IsPlayerReleased)
        {
            Debug.Log("Next Hook Name = "  + GetRelativeBlock(PlayerController.Instance.transform.position.z).parent.name);
            UpdateBlockPosition(currentBlockIndex);
        }
    }

    public Transform GetRelativeBlock(float playerPosZ)
    {
        //You may need this type of getter to which block are we going to cast our rope into
        foreach (GameObject block in blockPool)
        {
            if (block.transform.position.z == Mathf.Round(playerPosZ))
            {
                currentBlockIndex = blockPool.IndexOf(block);
                return blockPool[currentBlockIndex + 2].transform.Find("UpperBlock").transform;
            }
        }
        return null;
    }

    public void UpdateBlockPosition(int blockIndex)
    {
        //Block Pool has been created. Find a proper way to make infite map when it is needed
        foreach (GameObject block in blockPool)
        {
            if (blockPool.IndexOf(block) < blockIndex - 1)
            {
                blockPool.Remove(block);
                block.transform.position = blockPool[blockPool.Count - 1].transform.position + Vector3.forward;
                float randomYPos = Random.Range(8, 14);
                block.transform.Find("UpperBlock").transform.localPosition = new Vector3(0, randomYPos, 0);
                randomYPos = Random.Range(-10, -6);
                block.transform.Find("LowerBlock").transform.localPosition = new Vector3(0, randomYPos, 0);
                blockPool.Add(block);
            }
        }
    }
}