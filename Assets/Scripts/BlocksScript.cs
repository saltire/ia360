using System.Collections.Generic;
using UnityEngine;

public class BlocksScript : MonoBehaviour {
    public static BlocksScript instance;

    public BlockScript blockPrefab;

    public Material materialA;
    public Material materialB;
    List<BlockScript> blocksA;
    List<BlockScript> blocksB;
    System.Random random = new System.Random();

    void Start() {
        instance = this;
        blocksA = new List<BlockScript>(GetComponentsInChildren<BlockScript>());
        blocksB = new List<BlockScript>();
    }

    public void FlashBlock(Color color) {
        BlockScript[] blocks = GetComponentsInChildren<BlockScript>();
        BlockScript block = blocks[random.Next(0, blocks.Length)];
        block.Flash(color);
    }

    public void ChangeBlockToA(Color? color) {
        Debug.Log("A->B " + blocksA.Count + " " + blocksB.Count);
        if (blocksB.Count > 0) {
            BlockScript block = blocksB[random.Next(0, blocksB.Count)];
            block.GetComponent<MeshRenderer>().material = materialA;
            blocksB.Remove(block);
            blocksA.Add(block);
        }
        Debug.Log("A " + blocksA.Count + " B " + blocksB.Count);
    }

    public void ChangeAllToA() {
        foreach (BlockScript block in blocksB) {
            block.GetComponent<MeshRenderer>().material = materialA;
        }
        blocksB.Clear();
        blocksA = new List<BlockScript>(GetComponentsInChildren<BlockScript>());
    }

    public void ChangeBlockToB(Color? color) {
        Debug.Log("B->A " + blocksB.Count + " " + blocksA.Count);
        if (blocksA.Count > 0) {
            BlockScript block = blocksA[random.Next(0, blocksA.Count)];
            block.GetComponent<MeshRenderer>().material = materialB;
            blocksA.Remove(block);
            blocksB.Add(block);

            if (color is Color colorV) {
                block.Flash(colorV);
            }
        }
        Debug.Log("A " + blocksA.Count + " B " + blocksB.Count);
    }

    public void ChangeAllToB() {
        foreach (BlockScript block in blocksA) {
            block.GetComponent<MeshRenderer>().material = materialB;
        }
        blocksA.Clear();
        blocksB = new List<BlockScript>(GetComponentsInChildren<BlockScript>());
    }

    public void BuildWalls() {
        float blockSize = blockPrefab.transform.localScale.x;

        foreach (GameObject block in GameObject.FindGameObjectsWithTag("block")) {
            DestroyImmediate(block);
        }

        foreach (GameObject wall in GameObject.FindGameObjectsWithTag("wall")) {
            float bw = Mathf.Ceil(wall.transform.localScale.x / blockSize) + 1;
            float bh = Mathf.Ceil(wall.transform.localScale.y / blockSize) + 2;

            Vector3 wallOrigin = wall.transform.position
                + wall.transform.rotation * Vector3.forward * blockSize / 2
                + wall.transform.rotation * Vector3.left * bw * blockSize / 2
                + Vector3.up * (bh - 1) * blockSize / 2;

            Vector3 right = wall.transform.rotation * Vector3.right * blockSize;
            Vector3 down = Vector3.down * blockSize;

            for (int x = 0; x < bw; x++) {
                for (int y = 0; y < bh; y++) {
                    BlockScript block = Instantiate(blockPrefab, wallOrigin + x * right + y * down,
                        Quaternion.identity, transform);

                    block.GetComponent<MeshRenderer>().material = materialA;
                }
            }
        }

        blocksA = new List<BlockScript>(GetComponentsInChildren<BlockScript>());
        blocksB = new List<BlockScript>();
    }

    public void ExplodeWalls() {
        foreach (BlockScript block in GetComponentsInChildren<BlockScript>()) {
            block.Explode();
        }
    }

    public void UnexplodeWalls() {
        foreach (BlockScript block in GetComponentsInChildren<BlockScript>()) {
            block.Unexplode();
        }
    }
}
