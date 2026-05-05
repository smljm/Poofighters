using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;
    public float clampX = 8.5f;

    [Header("대쉬 설정 (Space)")]
    public float dashDistance = 3f;

    [Header("스피드 부스트 설정 (E키)")]
    public float speedBoostMultiplier = 2.5f;
    public float speedBoostDuration = 1f;
    public float speedBoostCooldown = 5f;

    [Header("주변 제거 설정 (Q키)")]
    public float clearRadius = 3f;
    public float clearCooldown = 7f;

    [Header("스킬 UI")]
    public TextMeshProUGUI skillText;

    private float currentSpeedMultiplier = 1f;
    private float speedBoostTimer = 0f;
    private float clearTimer = 0f;
    private bool isSpeedBoosting = false;

    public float SpeedBoostCooldownRatio =>
        speedBoostTimer > 0f ? speedBoostTimer / speedBoostCooldown : 0f;
    public float ClearCooldownRatio =>
        clearTimer > 0f ? clearTimer / clearCooldown : 0f;

    void Update()
    {
        HandleCooldowns();
        HandleMovement();
        HandleDash();
        HandleSpeedBoost();
        HandleClear();
        UpdateSkillUI();
    }

    void HandleCooldowns()
    {
        if (speedBoostTimer > 0f) speedBoostTimer -= Time.deltaTime;
        if (clearTimer > 0f) clearTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        float input = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input = 1f;

        float speed = moveSpeed * currentSpeedMultiplier;
        float newX = transform.position.x + input * speed * Time.deltaTime;
        newX = Mathf.Clamp(newX, -clampX, clampX);

        transform.position = new Vector3(newX, transform.position.y, 0f);
    }

    void HandleDash()
    {
        if (!Keyboard.current.spaceKey.wasPressedThisFrame) return;

        float input = 0f;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) input = -1f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) input = 1f;
        if (Mathf.Abs(input) < 0.1f) return;

        float targetX = Mathf.Clamp(transform.position.x + input * dashDistance, -clampX, clampX);
        transform.position = new Vector3(targetX, transform.position.y, 0f);
    }

    void HandleSpeedBoost()
    {
        if (!Keyboard.current.eKey.wasPressedThisFrame) return;
        if (speedBoostTimer > 0f) return;
        if (isSpeedBoosting) return;

        StartCoroutine(SpeedBoostCoroutine());
    }

    IEnumerator SpeedBoostCoroutine()
    {
        isSpeedBoosting = true;
        currentSpeedMultiplier = speedBoostMultiplier;

        yield return new WaitForSeconds(speedBoostDuration);

        currentSpeedMultiplier = 1f;
        isSpeedBoosting = false;
        speedBoostTimer = speedBoostCooldown;
    }

    void HandleClear()
    {
        if (!Keyboard.current.qKey.wasPressedThisFrame) return;
        if (clearTimer > 0f) return;

        StartCoroutine(ShowClearRadius());

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, clearRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Poop"))
                Destroy(hit.gameObject);
        }

        clearTimer = clearCooldown;
    }

    IEnumerator ShowClearRadius()
    {
        GameObject circle = new GameObject("ClearEffect");
        circle.transform.position = transform.position;
        LineRenderer lr = circle.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.loop = true;
        lr.widthMultiplier = 0.1f;
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = new Color(0f, 1f, 1f, 0.8f);
        lr.endColor = new Color(0f, 1f, 1f, 0f);

        int segments = 40;
        lr.positionCount = segments;
        for (int i = 0; i < segments; i++)
        {
            float angle = 2 * Mathf.PI * i / segments;
            float x = transform.position.x + Mathf.Cos(angle) * clearRadius;
            float y = transform.position.y + Mathf.Sin(angle) * clearRadius;
            lr.SetPosition(i, new Vector3(x, y, 0));
        }

        yield return new WaitForSeconds(0.3f);
        Destroy(circle);
    }

    void UpdateSkillUI()
    {
        if (skillText == null) return;

        string spaceStr = "Space(Dash): Ready";

        string eStr = isSpeedBoosting
            ? "E(Boost): Active!"
            : speedBoostTimer > 0f
                ? $"E(Boost): {speedBoostTimer:F1}s"
                : "E(Boost): Ready";

        string qStr = clearTimer > 0f
            ? $"Q(Clear): {clearTimer:F1}s"
            : "Q(Clear): Ready";

        skillText.text = $"{spaceStr}\n{eStr}\n{qStr}";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Poop"))
        {
            if (GameManager.Instance != null)
                GameManager.Instance.GameOver();
            else
                Debug.LogError("GameManager가 씬에 없습니다!");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, clearRadius);
    }
}