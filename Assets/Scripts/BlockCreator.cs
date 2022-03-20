using System.Collections.Generic;
using UnityEngine;


//In this class, the map has been created and infinite map function is here.
public class BlockCreator : MonoBehaviour
{

    #region Variables
    private static BlockCreator singleton = null;
    private GameObject[] blockPrefabs;
    private GameObject pointPrefab;
    private GameObject pointObject;
    public int blockCount;

    List<GameObject> blockPool = new List<GameObject>();
    private float lastHeightUpperBlock = 10;
    private int difficulty = 1;

    Camera _cam => Camera.main;

    int _currentBlockIndex;

    [Tooltip("Player Will Cast Rope After That Amount Of Block")]
    [SerializeField] int _nextBlockToCastRope = 2;

    [Header("Block Spawning Limits")]
    [SerializeField] float _upperBlockLimitMax = 14;
    [SerializeField] float _upperBlockLimitMin = 8;
    [SerializeField] float _lowerBlockLimitMax = -8;
    [SerializeField] float _lowerBlockLimitMin = -14;
    #endregion

    public static BlockCreator GetSingleton()
    {
        if (singleton == null)
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

    /// <summary>
    /// Returns Randomized Vector3 That Is Used For Block's Y Position
    /// </summary>
    /// <param name="type">Use "U" For UpperBlock, "L" For LowerBlock</param>
    /// <returns>Vector3 Random Y Position</returns>
    Vector3 RandomYPos(string type)
    {
        switch (type)
        {
            case "U":
                float randomPosY = Random.Range(_upperBlockLimitMin, _upperBlockLimitMax); // Y Pos Spawn Limits
                Vector3 spawnPos = new Vector3(0, randomPosY, 0);
                return spawnPos;
            case "L":
                randomPosY = Random.Range(_lowerBlockLimitMin, _lowerBlockLimitMax); // Y Pos Spawn Limits
                spawnPos = new Vector3(0, randomPosY, 0);
                return spawnPos;
            default:
                return Vector3.zero;
        }
    }

    public void InstantiateBlocks()
    {
        float zPos = 1;
        for (int i = 0; i < blockCount; i++)
        {
            #region Upper Block
            int randomPrefab = Random.Range(0, blockPrefabs.Length); // Get Random Prefab From The Array
            GameObject upperBlock = Instantiate(blockPrefabs[randomPrefab], RandomYPos("U"), Quaternion.identity);
            upperBlock.name = "UpperBlock";
            #endregion // Upper Block Ends

            #region Lower Block
            GameObject lowerBlock = Instantiate(blockPrefabs[randomPrefab], RandomYPos("L"), Quaternion.identity);
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

    void Update()
    {
        if (PlayerController.Instance == null || blockPool == null)
        {
            return;
        }

        if (PlayerController.Instance.IsPlayerReleased)
        {
            GetRelativeBlock(PlayerController.Instance.transform.position.z); // Used To Update _currentBlockIndex In Smoother Way.
            UpdateBlockPosition(_currentBlockIndex);
        }
    }

    public Transform GetRelativeBlock(float playerPosZ)
    {
        foreach (GameObject block in blockPool)
        {
            if (block.transform.position.z == Mathf.Round(playerPosZ))
            {
                _currentBlockIndex = blockPool.IndexOf(block);
                return blockPool[_currentBlockIndex + _nextBlockToCastRope].transform.Find("UpperBlock").transform;
            }
        }
        return null;
    }

    public void UpdateBlockPosition(int blockIndex)
    {
        //GetRelativeBlock(PlayerController.Instance.transform.position.z); // Could be used here to update _currentBlockIndex as well
        foreach (GameObject block in blockPool)
        {
            if (blockPool.IndexOf(block) < blockIndex - 1)
            {
                blockPool.Remove(block);
                block.transform.position = blockPool[blockPool.Count - 1].transform.position + Vector3.forward;
                block.transform.Find("UpperBlock").transform.localPosition = RandomYPos("U");
                block.transform.Find("LowerBlock").transform.localPosition = RandomYPos("L");
                blockPool.Add(block);
            }
        }
    }
}