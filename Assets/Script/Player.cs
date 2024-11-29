using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public SpriteRenderer characterRenderer;
    public Sprite[] characterSprites;

    public Rigidbody2D rb;

    public float moveSpeed = 1f;
    public float upForce = 300f;
    public float forwardForce = 150f;
    public float rotateSpeed = 1f;
    private bool isGrounded = true;

    public Vector2 originalPos;

    public GameObject LostPanel;
    public GameObject WinPanel;

    public float sensitivity = 20f;
    public float loudness = 0f;
    public Slider sensitivitySlider;

    private AudioClip micClip;
    private const int sampleWindow = 128;
    void Start()
    {
        originalPos = transform.position;
        int selectedIndex = PlayerPrefs.GetInt("SelectedCharacterIndex", -1);

        if (selectedIndex >= 0 && selectedIndex < characterSprites.Length)
        {
            characterRenderer.sprite = characterSprites[selectedIndex];
        }
        else
        {
            Debug.LogError("No character selected or index out of range!");
        }

        rb = gameObject.GetComponent<Rigidbody2D>();

        micClip = Microphone.Start(null, true, 10, 44100);
        while (!(Microphone.GetPosition(null) > 0)) { }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = sensitivity;
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
        }
    }

    void Update()
    {
        loudness = GetLoudnessFromMic() * sensitivity;

        float horizontalInput = Input.GetAxis("Horizontal");

        if (loudness > 0.5f && loudness < 1f && isGrounded)
        {
            Move();
        }
        else if (isGrounded && loudness > 2f)
        {
            Jump();
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            Time.timeScale = 0;
            LostPanel.SetActive(true);
        }
        if (collision.gameObject.CompareTag("Flag"))
        {
            Time.timeScale = 0;
            WinPanel.SetActive(true);
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        LostPanel.SetActive(false);
        transform.position = originalPos;
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("SelectAnimal");
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex + 1);
    }
    public void OnSensitivityChanged(float value)
    {
        sensitivity = value;
    }

    public void Move()
    {
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y * Time.deltaTime);
    }

    public void Jump()
    {
        isGrounded = false;
        rb.AddForce(Vector2.up * upForce + Vector2.right * forwardForce);
    }

    private float GetLoudnessFromMic()
    {
        if (micClip == null) return 0;

        float[] sampleData = new float[sampleWindow];

        int micPosition = Microphone.GetPosition(null) - sampleWindow;
        if (micPosition < 0) return 0;

        micClip.GetData(sampleData, micPosition);

        float sum = 0;
        for (int i = 0; i < sampleWindow; i++)
        {
            sum += sampleData[i] * sampleData[i];
        }

        return Mathf.Sqrt(sum / sampleWindow);
    }
}
