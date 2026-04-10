using UnityEngine;

//Note: There was no need to make this a SO just to hold a sprite.
//But I made it so just to showcase a commonly used pattern
[CreateAssetMenu(fileName = "NewBlock", menuName = "Puzzle/Block")]
public class Block : ScriptableObject
{
    public Sprite Sprite;
}
