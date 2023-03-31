using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Box : MonoBehaviour
{
    public enum BoxContent
    {
        Cross,
        Circle,
        Empty
    }

    private BoxContent _content;
    [SerializeField] private Button button;
    [SerializeField] private Image contentImage;
    [SerializeField] private Sprite crossVisual;
    [SerializeField] private Sprite circleVisual;

    public Button Button => button;

    public BoxContent Content
    {
        get
        {
            return _content;
        }
        set
        {
            _content = value;
            switch(_content)
            {
                case BoxContent.Cross: contentImage.sprite = crossVisual; break;
                case BoxContent.Circle: contentImage.sprite = circleVisual; break;
                case BoxContent.Empty: contentImage.sprite = null; break;
            }
        }
    }

    private void Start()
    {
        Content = BoxContent.Empty;
    }
}
