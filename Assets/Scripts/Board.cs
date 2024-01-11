using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.Collections;
using Unity.Burst.Intrinsics;

public class Board : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    private int score = 0;
    private float time = 0;
    public Text scoreText;
    public Text timeText;

    [Header("game over")]
    public GameObject panel, retryButton;
    public static bool gameOver = false;
    public RectInt Bounds //RectInt: Dikdortgenlerde sinir belirtmede kullanilir.
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);// Yeni alan sinirlarini ayarlama
            return new RectInt(position, boardSize); // Yeni alan sinirlari.
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        panel.SetActive(false);
        SpawnPiece();
    }
    private void FixedUpdate()
    {
        if (!gameOver)
        {
            time += Time.deltaTime;
            string formattedTimer = time.ToString("Time:0.0");
            timeText.text = formattedTimer;
        }
    }

    public void SpawnPiece() //Data'dan random olarak parca olusturma
    {
        if (!gameOver)
        {
            int random = Random.Range(0, tetrominoes.Length);
            TetrominoData data = tetrominoes[random];

            activePiece.Initialize(this, spawnPosition, data);
        }

        //spawn noktasindan parca olusturabiliyorsa piece kodunu calistir yani hucre spawnla
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else //degilse oyun bitsin
        {
            gameOver = true;
            panel.SetActive(true);
            retryButton.SetActive(true);
            GameOver();

        }
    }
    public void GameOver() //bittiginde sahnedeki bloklarin yok olmasi icin
    {
        if (Game_Over.Instance.isClickedRetryButton)
        {
            Game_Over.Instance.ResetPos(scoreText, timeText);

            retryButton.SetActive(false);
            panel.SetActive(false);
            gameOver = false;

            tilemap.ClearAllTiles();

            score = 0;
            time = 0;
            UpdateScoreUI();
            Game_Over.Instance.isClickedRetryButton = false;
        }
        else
        {
            Game_Over.Instance.ChangePos(scoreText, timeText);
        }

    }

    public void Set(Piece piece) //Piece kodundaki hucrelerin olusturulmasini saglar.
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece) //Hucreler olusturulurken bos kisimlar olusturur.
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null); //Belirlenen pozisyonu bos birak
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position) //Konumu kontrol eder
    {
        RectInt bounds = Bounds;

        // Konum yalnızca her hucre geçerliyse geçerlidir
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            if (!bounds.Contains((Vector2Int)tilePosition)) // Hucreler sinirlar icinde degilse
            {
                return false; // Hucrelerin sinirlarin disina cikmasini engeller
            }

            if (tilemap.HasTile(tilePosition)) // Bir hucre zaten bu konumu isgal ediyor, bu nedenle gecersiz
            {
                return false;
            }
        }
        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Asagidan yukariya dogru temizle
        while (row < bounds.yMax)
        {
            // Yalnizca akım temizlenmezse bir sonraki satıra ilerleyin
            // cunku bir sira temizlendiginde yukaridaki dosemeler dusecek
            if (IsLineFull(row)) //Satir tamamen doluysa
            {
                LineClear(row); //Satiri temizle
                score += 1;
                UpdateScoreUI();
            }
            else //Degilse bir ust satira gec
            {
                row++;
            }
        }
    }
    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    public bool IsLineFull(int row) //Bir satirin komple dolu olup olmadigini kontrol etmek icin
    {
        RectInt bounds = Bounds; //Yeni sinirlar

        for (int col = bounds.xMin; col < bounds.xMax; col++) //bir satiri bastan sona artir
        {
            Vector3Int position = new Vector3Int(col, row, 0); //Satirdaki col degerine gore x pozisyonuna yerlestirir

            // Bir doseme eksikse satir dolu degil
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }
        // Satir doluysa true dondurur
        return true;
    }

    public void LineClear(int row) //Bir satiri komple temizlemek icin
    {
        RectInt bounds = Bounds;

        // Satirdaki tum dosemeleri temizle
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0); //Col degeri yani satirdaki hucre
            tilemap.SetTile(position, null); //Satirdaki hucreyi bos birakir
        } //Satiri bastan sona bos birakir yani temizler

        // Her satiri bir asagi kaydir
        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0); //bir ustteki satirdaki col degerlerini yani hucreleri alir
                TileBase above = tilemap.GetTile(position); //Pozisyondaki tile'i alir.

                position = new Vector3Int(col, row, 0); //Asil satirdaki yani alt satirda yeni pozisyon olustururlur
                tilemap.SetTile(position, above); //Ust satirdaki parcalar onceden alinan tile'a yeni pozsiyonda yerlestirilir
            }   //Satir bir sagi satira kaydirilmis olur
            row++;
        }
    }

}