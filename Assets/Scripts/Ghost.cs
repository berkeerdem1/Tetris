using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    public Tile tile;
    public Board mainBoard;
    public Piece trackingPiece;

    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4]; //Her zaman dort hucrelik parca olusacak
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear() //Hucreleri null yapar
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy() //Hucreleri kopyalar
    {
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.cells[i];
        }
    }

    private void Drop() //Onizleme parcasini en asagi satirda y ekseninde sabitler
    {
        Vector3Int position = trackingPiece.position;

        int current = position.y;
        int bottom = -mainBoard.boardSize.y / 2 - 1;

        mainBoard.Clear(trackingPiece);

        for (int row = current; row >= bottom; row--) //Row'un pozisyonunu en asagidaki satirin pozisyonu yapar
        {
            position.y = row; //Row'un pozisyonunu y pozisyonu yap

            if (mainBoard.IsValidPosition(trackingPiece, position)) //Eger konumda sorun yoksa pozsiyonu alir
            {
                this.position = position;
            }
            else
            {
                break;
            }
        }

        mainBoard.Set(trackingPiece); //Piece kodundaki hareketleri izler
    }

    private void Set()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }

}