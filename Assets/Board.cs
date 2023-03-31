using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UIElements;

public class Board : NetworkBehaviour
{
    [SerializeField] private Box[][] boxes;
    private bool isCrossPlayer = false;
    private bool isPlayerTurn = false;

    public Box[][] Boxes => boxes;
    public List<Box> BoxList
    {
        get
        {
            return new List<Box>(boxes[0].Concat(boxes[1]).Concat(boxes[2]));
        }
        
    }

    private void Start()
    {
        boxes = LoadBoxes();
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            isCrossPlayer = true;
            isPlayerTurn = true;
        };

        List<Box> list = BoxList;
        for (int i = 0; i < list.Count; i++)
        {
            Box box = list[i];
            int index = i;
            box.Button.onClick.AddListener(() =>
            {
                SendMovementServerRpc(index, NetworkManager.Singleton.LocalClientId);                
            });
        }
    }

    private Box[][] LoadBoxes ()
    {
        Box[][] boxes = new Box[3][];
        boxes[0] = new Box[3];
        boxes[1] = new Box[3];
        boxes[2] = new Box[3];

        for (int i = 0; i < transform.childCount; i++)
        {
            Box box = transform.GetChild(i).GetComponent<Box>();
            boxes[i / 3][i % 3] = box;
        }
        return boxes;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMovementServerRpc (int position, ulong clientId)
    {
        if (IsMovementValid(position))
        {
            if (clientId != NetworkManager.Singleton.LocalClientId)
            {
                
                if (!isPlayerTurn)
                {
                    if (isCrossPlayer)
                        SetBoxContentClientRpc(position, Box.BoxContent.Circle);
                    else
                        SetBoxContentClientRpc(position, Box.BoxContent.Cross);
                    isPlayerTurn = true;
                }
            }
            else
            {
                if (isPlayerTurn)
                {
                    if (isCrossPlayer)
                        SetBoxContentClientRpc(position, Box.BoxContent.Cross);
                    else
                        SetBoxContentClientRpc(position, Box.BoxContent.Circle);
                    isPlayerTurn = false;
                }
            }
        }
    }

    [ClientRpc]
    private void SetBoxContentClientRpc (int position, Box.BoxContent content)
    {
        BoxList[position].Content = content;
    }

    private bool IsMovementValid (int position)
    {
        return true;
    }
}
