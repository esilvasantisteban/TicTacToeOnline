using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;

public class Board : NetworkBehaviour
{
    [SerializeField] private Box[][] boxes;

    public Box[][] Boxes => boxes;

    private void Awake()
    {
        boxes = new Box[3][];
        boxes[0] = new Box[3];
        boxes[1] = new Box[3];
        boxes[2] = new Box[3];

        for (int i = 0; i < transform.childCount; i++)
        {
            Box box = transform.GetChild(i).GetComponent<Box>();
            boxes[i / 3][i % 3] = box;
            if (IsServer)
            {
                Box.BoxContent[] contents = GetContents();
                UpdateBoardClientRpc(contents);
            }
        }
    }

    [ClientRpc]
    private void UpdateBoardClientRpc (Box.BoxContent[] contents)
    {
        SetContents(contents);
        print("Updated received from server!");
    }

    private Box.BoxContent[] GetContents ()
    {
        Box.BoxContent[] contents = new Box.BoxContent[9];
        
        return contents;
    }

    private void SetContents (Box.BoxContent[] contents)
    {

    }
}
