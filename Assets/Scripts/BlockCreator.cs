using System.Collections.Generic;
using UnityEngine;

//In this class, the map has been created and infinite map function is here.
public class BlockCreator : MonoBehaviour
{
    #region Variables
    private static BlockCreator singleton = null;
    private GameObject[] blockPrefabs;
    private GameObject pointPrefab;
    private int difficulty = 1;
    private int _currentBlockIndex;
    public int blockCount;

    List<GameObject> blockPool = new List<GameObject>();
    Camera _cam => Camera.main;
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

    void InstantiateBlocks()
    {
        float zPos = 1;
        for (int i = 0; i < blockCount; i++)
        {
            #region Upper Block
            int randomPrefab = Random.Range(0, blockPrefabs.Length); // Get Random Prefab From The Array
            GameObject upperBlock = Instantiate(blockPrefabs[randomPrefab], GameManager.Instance.RandomYPos("U"), Quaternion.identity);
            upperBlock.name = "UpperBlock";
            #endregion // Upper Block Ends

            #region Lower Block
            GameObject lowerBlock = Instantiate(blockPrefabs[randomPrefab], GameManager.Instance.RandomYPos("L"), Quaternion.identity);
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

        GameObject point = Instantiate(pointPrefab, Vector3.zero, pointPrefab.transform.rotation);
        point.name = "Point";
        point.transform.position = blockPool[blockCount - 1].transform.position;
        point.transform.parent = blockPool[blockCount - 1].transform;
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
                return blockPool[_currentBlockIndex + GameManager.Instance.NextBlockToCastRope].transform.Find("UpperBlock").transform;
            }
        }
        return blockPool[0].transform.Find("UpperBlock").transform;
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
                block.transform.Find("UpperBlock").transform.localPosition = GameManager.Instance.RandomYPos("U");
                block.transform.Find("LowerBlock").transform.localPosition = GameManager.Instance.RandomYPos("L");
                blockPool.Add(block);

                //If Block Has Point Objects, Then Activate Again..
                if (block.transform.Find("Point") == null) continue;
                GameObject _childPoint = block.transform.Find("Point").GetChild(0).gameObject;
                if (_childPoint != null && !_childPoint.activeSelf) _childPoint.SetActive(true);
            }
        }
    }
}