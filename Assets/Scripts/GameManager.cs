using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    void Start() => Instance = this;

    [Tooltip("Player Will Cast Rope After That Amount Of Block")]
    [SerializeField] int _nextBlockToCastRope = 2;
    public int NextBlockToCastRope { get { return _nextBlockToCastRope; } }

    [Header("Block Spawning Limits")]
    [SerializeField] float _blockLimitMax = 12;
    [SerializeField] float _blockLimitMin = 8;

    /// <summary>
    /// Returns Randomized Vector3 That Is Used For Block's Y Position
    /// </summary>
    /// <param name="type">Use "U" For UpperBlock, "L" For LowerBlock</param>
    /// <returns>Vector3 Random Y Position</returns>
    public Vector3 RandomYPos(string type)
    {
        switch (type)
        {
            case "U":
                float randomPosY = Random.Range(_blockLimitMin, _blockLimitMax); // Y Pos Spawn Limits
                Vector3 spawnPos = new Vector3(0, Mathf.Round(randomPosY), 0);
                return spawnPos;
            case "L":
                randomPosY = Random.Range(-(_blockLimitMax), -(_blockLimitMin)); // Y Pos Spawn Limits
                spawnPos = new Vector3(0, Mathf.Round(randomPosY), 0);
                return spawnPos;
            default:
                return Vector3.zero;
        }
    }
}