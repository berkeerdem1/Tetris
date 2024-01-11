using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f; //İlerleme suresi
    public float moveDelay = 0.1f; //Hareket suresi
    public float lockDelay = 0.5f; //Kilitlenme suresi

    private float stepTime;
    private float moveTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);

        // Oyuncunun parcada ayarlamalar yapmasina izin vermek icin bir zamanlayici kullaniyoruz
        // yerine kilitlenmeden once
        lockTime += Time.deltaTime;

        // Parcayi saat yonunun tersinde dondurur
        if (Input.GetKeyDown(KeyCode.Q) && !Board.gameOver)
        {
            Rotate(-1);
        }
        // Parcayi saat yonunde dondurur
        else if (Input.GetKeyDown(KeyCode.E) && !Board.gameOver)
        {
            Rotate(1);
        }
        // Direkt hedefe dusus yapar
        if (Input.GetKeyDown(KeyCode.Space) && !Board.gameOver)
        {
            HardDrop();
        }

        // Oyuncunun hareket tuslarini tutmasina izin verir
        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        // Parcayi her .. saniyede bir sonraki satira ilerletir
        if (Time.time > stepTime)
        {
            Step();
        }

        board.Set(this);


    }

    private void HandleMoveInputs()
    {
        // Yumusak dusme hareketi icin
        if (Input.GetKey(KeyCode.S) && !Board.gameOver)
        {
            if (Move(Vector2Int.down))
            {
                // Cift hareketi onlemek iCin adim suresini günceller
                stepTime = Time.time + stepDelay;
            }
        }
        if (Input.GetKey(KeyCode.A) && !Board.gameOver) // Saga hareket eder
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D) && !Board.gameOver) // Sola hareket eder
        {
            Move(Vector2Int.right);
        }
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        // Sonraki satira gecer
        Move(Vector2Int.down);

        // Parca cok uzun sure hareketsiz kaldiginda kilitlenir
        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void HardDrop() //Parcayi direkt hedefe ulastirir
    {
        while (Move(Vector2Int.down)) //Parca asagi hareket ediyorken devam eder
        {
            continue;
        }

        Lock();
    }

    private void Lock()
    {
        board.Set(this);
        board.ClearLines(); //Satiri kontrol eder ve islemlerden sonra ust satira gecer
        board.SpawnPiece(); //Yeni parca olusturur
    }

    private bool Move(Vector2Int translation) //Hareket kontrolu
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x; //x konumunda ilerleme
        newPosition.y += translation.y; //y konumunda ilerleme

        bool valid = board.IsValidPosition(this, newPosition);

        // Hareketi yalnizca yeni konum gecerliyse kaydeder
        if (valid)
        {
            position = newPosition; //yeni konum
            moveTime = Time.time + moveDelay; //hareket suresi
            lockTime = 0f; // reset
        }
        return valid;
    }

    private void Rotate(int direction) //Parcayi dondurur
    {
        // Dondurmenin basarisiz olmasi durumunda gecerli dondurmeyi saklar
        int originalRotation = rotationIndex;

        // Bir dondurme matrisi kullanarak tum hucreleri dondurur
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // Duvar vurusu testleri basarisiz olursa donusu geri al
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix; //Matrisi alir

        //  Bir dondurme matrisi kullanarak tum hucreleri dondurur
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" ve "O" merkez noktasindan dondurulur
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    //Mathf.CeilToInt: Ondalik sayiyi her zaman bir ust sayiya yuvarlar
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    //Mathf.RoundToInt: Ondalik sayiyi 0.5'den buyukse bir ust sayiya yuvarlar
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }
            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection) //Duvar testi kontrolu icin
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++) //Duvar uzunlugu boyunca harekete izin ver
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false; //Duvar uzunlugu asilirsa harekete izin verme
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

}