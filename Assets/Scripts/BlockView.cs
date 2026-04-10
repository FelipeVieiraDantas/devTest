using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlockView : MonoBehaviour, IPointerClickHandler
{
    public Block BlockData { get; private set; }
    public int Row { get; private set; }
    public int Col { get; private set; }

    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void Initialize(Block block, int row, int col)
    {
        BlockData = block;
        Row = row;
        Col = col;
        _image.sprite = block.Sprite;
    }

    public void SetGridPosition(int row, int col)
    {
        Row = row;
        Col = col;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.OnBlockClicked(Row, Col);
    }
}
