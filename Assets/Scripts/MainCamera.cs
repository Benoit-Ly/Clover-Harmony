using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCamera : MonoBehaviour
{
    #region Services
    GameManager m_GameManager;
    BattleManager m_BattleManager;
    #endregion

    Transform m_Camera;
    Vector3[] m_ShakePositions =
    {
        new Vector3(0.25f, 0.75f, 0f),
        new Vector3(0.75f, 0.25f, 0f)
    };
    int m_NbShakePositions;

    [SerializeField]
    Image fadeImage;
    [SerializeField]
    public float fadeDuration = 1f;
    [SerializeField]
    public float fadeBlackScreenTime = 1f;
    [SerializeField]
    GameObject playerOne, playerTwo;
    [SerializeField]
    float smoothShifting, smoothZoom, smoothY;
    [SerializeField]
    float minDistancePlayer, maxDistancePlayer;
    [SerializeField]
    float minFieldOfView, maxFieldOfView;
    [SerializeField]
    float maxYPos;
    [SerializeField]
    float minClampY, mediumClampY, maxClampY;

    bool isArena = false;
    GameObject leftCollider;
    GameObject rightCollider;

    Vector3 m_posCameraArena;
    float m_FieldOfViewArena;

    float YValueMax = 1.2f;
    float YValueMin = 0.9f;

    float m_ShakeForce = 0.05f;

    private void Start()
    {
        m_GameManager = ServiceLocator.Instance.GetService<GameManager>(ManagerType.GAME_MANAGER);
        m_BattleManager = ServiceLocator.Instance.GetService<BattleManager>(ManagerType.BATTLE_MANAGER);
        m_BattleManager.OnDamage += OnEntityDamage;

        m_Camera = transform.Find("Camera");
        m_NbShakePositions = 5;

        leftCollider = transform.GetChild(0).gameObject;
        rightCollider = transform.GetChild(1).gameObject;

        fadeImage.canvasRenderer.SetAlpha(0f);
        fadeImage.gameObject.SetActive(true);

        m_GameManager.mainCamera = this;
    }

    void Update()
    {
        if (!isArena)
            TargetPlayer();

        else if (isArena)
            UpdatePosCam();

        UpdateCollider();
    }

    public void FadeIn()
    {
        fadeImage.CrossFadeAlpha(1.0f, fadeDuration, true);
    }

    public void FadeOut()
    {
        fadeImage.CrossFadeAlpha(0.0f, fadeDuration, true);
    }

    void TargetPlayer()
    {
        float playerPos = (playerOne.transform.position.x + playerTwo.transform.position.x) / 2;

        float xValue = Mathf.Lerp(transform.position.x, playerPos, smoothShifting * Time.deltaTime);

        CheckDistance();

        transform.position = new Vector3(xValue, transform.position.y, transform.position.z);
    }

    void ChangeFieldOfView(float fieldOfView)
    {
        Camera.main.orthographicSize = fieldOfView;
    }

    void CheckDistance()
    {
        float value;

        if ((Mathf.Abs(playerOne.transform.position.x - playerTwo.transform.position.x)) < minDistancePlayer)
        {
            value = (Mathf.Abs(playerOne.transform.position.x - playerTwo.transform.position.x)) / 100 + YValueMin;

            transform.position = new Vector3(transform.position.x, Mathf.Lerp(Camera.main.transform.position.y, Mathf.Max(value, minClampY) * maxYPos, smoothY * Time.deltaTime), transform.position.z);

            ChangeFieldOfView(Mathf.Lerp(Camera.main.orthographicSize, minFieldOfView, smoothZoom * Time.deltaTime));

            //value = (Mathf.Abs(playerOne.transform.position.x - playerTwo.transform.position.x)) / 100 + YValueMin;
            //transform.position = new Vector3(transform.position.x, Mathf.Lerp(Camera.main.transform.position.y, value * 13, smoothZoom * Time.deltaTime), transform.position.z);
        }

        else if ((Mathf.Abs(playerOne.transform.position.x - playerTwo.transform.position.x)) > maxDistancePlayer)
        {
            value = (Mathf.Abs(playerOne.transform.position.x - playerTwo.transform.position.x)) / 100 + YValueMax;

            transform.position = new Vector3(transform.position.x, Mathf.Lerp(Camera.main.transform.position.y, Mathf.Min(value, maxClampY) * maxYPos, smoothY * Time.deltaTime), transform.position.z);

            ChangeFieldOfView(Mathf.Lerp(Camera.main.orthographicSize, maxFieldOfView, smoothZoom * Time.deltaTime));
        }

        else
        {
            value = (Mathf.Abs(playerOne.transform.position.x - playerTwo.transform.position.x)) / 100 + YValueMax;

            transform.position = new Vector3(transform.position.x, Mathf.Lerp(Camera.main.transform.position.y, Mathf.Min(value, mediumClampY) * maxYPos, smoothY * Time.deltaTime), transform.position.z);

            ChangeFieldOfView(Mathf.Lerp(Camera.main.orthographicSize, Mathf.Abs(playerOne.transform.position.x - playerTwo.transform.position.x), smoothZoom * Time.deltaTime));
        }


        //transform.position = new Vector3(transform.position.x, Mathf.Lerp(Camera.main.transform.position.y, value * maxYPos, smoothZoom * Time.deltaTime), transform.position.z);
    }

    public void EndArena()
    {
        isArena = false;
    }

    public void SetPos(float x)
    {
        transform.position = new Vector3(x, transform.position.y, transform.position.z);
    }

    public void RunArena(Vector3 posCamera, float fieldOfView)
    {
        isArena = true;
        transform.position = posCamera;

        ChangeFieldOfView(fieldOfView);
    }

    public void LaunchArena(Vector3 posCamera, float fieldOfView)
    {
        isArena = true;
        m_posCameraArena = posCamera;
        m_FieldOfViewArena = fieldOfView;
    }

    void UpdatePosCam()
    {
        transform.position = new Vector3((Mathf.Lerp(transform.position.x, m_posCameraArena.x, smoothZoom * Time.deltaTime)), m_posCameraArena.y, m_posCameraArena.z);
        ChangeFieldOfView(Mathf.Lerp(Camera.main.orthographicSize, m_FieldOfViewArena, smoothZoom * Time.deltaTime));
    }

    void UpdateCollider()
    {
        //leftCollider.transform.position = new Vector3(-Camera.main.aspect * (Camera.main.orthographicSize), transform.position.y, transform.position.z);
        //Debug.Log(leftCollider.transform.position);
        //rightCollider.transform.position = new Vector3(Camera.main.aspect * (Camera.main.orthographicSize), transform.position.y, transform.position.z);

        leftCollider.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(0f, Screen.height / 2f, Camera.main.nearClipPlane));
        rightCollider.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height / 2f, Camera.main.nearClipPlane));
    }

    void OnEntityDamage(Entity owner, Entity target, AttackStats stats)
    {
        Shake();
    }

    public void Shake()
    {
        StartCoroutine(ShakeCoroutine(m_ShakeForce));
    }

    IEnumerator ShakeCoroutine(float force)
    {
        for (int i = 0; i < m_NbShakePositions; i++)
        {
            Vector3 direction = Random.insideUnitCircle.normalized;
            m_Camera.localPosition = new Vector3(direction.x, direction.y, 0f) * force;
            yield return new WaitForEndOfFrame();
        }
        m_Camera.localPosition = Vector3.zero;
    }

    private void OnDestroy()
    {
        m_BattleManager.OnDamage -= OnEntityDamage;
    }
}