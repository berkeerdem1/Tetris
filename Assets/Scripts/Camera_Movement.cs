using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Movement : MonoBehaviour
{
    public float moveSpeed = 5f; // Kamera sag-sol hareket hizi
    public float moveVerticalSpeed = 5f; // Kamera asagi hareket hizi
    public float moveHardSpeed = 5f;
    public float returnSpeed = 2f; // Kameranin eski haline donme hızı

    private Vector3 originalPosition; // Kameranin baslangic pozisyonu
    private bool isMoving = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        if (!Board.gameOver)
        {
            HandleInput();
            ReturnToOriginalPosition();
        }
    }

    void HandleInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // A tusuna basinca sola kaydir
        if (horizontalInput < 0 && !Board.gameOver)
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            isMoving = true;
        }
        // D tusuna basinca saga kaydir
        else if (horizontalInput > 0 && !Board.gameOver)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            isMoving = true;
        }
        // S tusuna basinca asagi kaydir
        if (verticalInput < 0 && !Board.gameOver)
        {
            transform.Translate(Vector3.down * moveVerticalSpeed * Time.deltaTime);
            isMoving = true;
        }
        // Spcae tusuna basinca asagi sert kaydir
        if (Input.GetKeyDown(KeyCode.Space) && !Board.gameOver)
        {
            transform.Translate(Vector3.down * moveHardSpeed * Time.deltaTime);
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    void ReturnToOriginalPosition()
    {
        // Eger kamera hareket halinde degilse ve pozisyon farkliysa  eski haline dondur
        if (!isMoving && transform.position != originalPosition)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, returnSpeed * Time.deltaTime);
        }
    }
}
