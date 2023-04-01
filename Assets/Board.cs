using Unity.Netcode;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Board : NetworkBehaviour
{
    [SerializeField] private Box[][] boxes;
    [SerializeField] private GameObject restartBtn;
    [SerializeField] private TextMeshProUGUI message;
    private bool isCrossPlayer = false;
    private bool isPlayerTurn = false;
    private Player thisPlayer = Player.Circle;

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
            thisPlayer = Player.Cross;

            restartBtn.GetComponent<Button>().onClick.AddListener(() => {
                restartBtn.SetActive(false);
                SetMessageActiveClientRpc(false);
            });
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
        //print("Client: " + clientId + " trying to make a move...");
        if (!IsMovementValid(position)) return;
        if (clientId != NetworkManager.Singleton.LocalClientId)
        {
            if (isPlayerTurn) return;
            Box.BoxContent content = isCrossPlayer? Box.BoxContent.Circle: Box.BoxContent.Cross;
            SetBoxContentClientRpc(position, content);
            isPlayerTurn = true;
        }
        else
        {
            if (!isPlayerTurn) return;
            Box.BoxContent content = isCrossPlayer ? Box.BoxContent.Cross : Box.BoxContent.Circle;
            SetBoxContentClientRpc(position, content);
            isPlayerTurn = false;
        }

        Player winner = GetWinner();
        if (winner != Player.None)
        {
            print("There is a winner!" + ((winner == Player.Cross)? "Cruz": "Circulo"));
            restartBtn.SetActive(true);
            SendWinnerClientRpc(winner);
        }
    }

    [ClientRpc]
    public void ClearBoardClientRpc ()
    {
        foreach(Box box in BoxList)
        {
            box.Content = Box.BoxContent.Empty;
        }
    }

    [ClientRpc]
    private void SetBoxContentClientRpc (int position, Box.BoxContent content)
    {
        BoxList[position].Content = content;
    }

    private bool IsMovementValid (int position)
    {
        if (BoxList[position].Content != Box.BoxContent.Empty) return false;
        return true;
    }

    private Player GetWinner ()
    {
        List<Box> b = BoxList;

        if (b[0].Content == b[1].Content && b[1].Content == b[2].Content) return (Player)b[0].Content;
        if (b[3].Content == b[4].Content && b[5].Content == b[3].Content) return (Player)b[3].Content;
        if (b[6].Content == b[7].Content && b[8].Content == b[6].Content) return (Player)b[6].Content;

        if (b[0].Content == b[3].Content && b[6].Content == b[0].Content) return (Player)b[0].Content;
        if (b[1].Content == b[4].Content && b[7].Content == b[1].Content) return (Player)b[1].Content;
        if (b[2].Content == b[5].Content && b[8].Content == b[2].Content) return (Player)b[2].Content;
       
        if (b[0].Content == b[4].Content && b[8].Content == b[0].Content) return (Player)b[0].Content;
        if (b[2].Content == b[4].Content && b[6].Content == b[2].Content) return (Player)b[2].Content;

        return Player.None;
    }

    [ClientRpc]
    private void SendWinnerClientRpc (Player winner)
    {
        string message;
        if (thisPlayer == winner) message = "Felicidades! Ganaste";
        else message = "Lástima. Has perdido";
        DisplayMessage(message);
    }

    [ClientRpc]
    private void SetMessageActiveClientRpc (bool value)
    {
        message.gameObject.SetActive(value);
    }

    private void DisplayMessage (string message)
    {
        this.message.gameObject.SetActive(true);
        this.message.text = message;
    }
}

public enum Player
{
    Cross = Box.BoxContent.Cross,
    Circle = Box.BoxContent.Circle,
    Draw,
    None
}