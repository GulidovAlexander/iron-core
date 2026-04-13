# Iron Core — Technical Design Document

> Версия: 0.1 — Апрель 2026
> Разработчик: Alexander
> Движок: Unity 6, C#

---

## 1. Структура проекта

```
Assets/
└── _Project/               ← весь код проекта здесь, не в корне
    ├── Scripts/
    │   ├── Player/         ← всё связанное с игроком
    │   ├── Enemies/        ← AI, здоровье, поведение врагов
    │   ├── Weapons/        ← оружие, пули, граната, raycast
    │   ├── World/          ← двери, триггеры, интерактивные объекты, бочки
    │   ├── UI/             ← HUD, меню, экраны
    │   ├── Systems/        ← GameManager, checkpoint, сохранения
    │   └── Utils/          ← хелперы, расширения, общие утилиты
    ├── Prefabs/
    │   ├── Player/
    │   ├── Enemies/
    │   ├── Weapons/
    │   ├── World/
    │   └── UI/
    ├── ScriptableObjects/
    │   ├── Weapons/        ← данные оружий (WeaponData SO)
    │   └── Enemies/        ← данные врагов (EnemyData SO)
    ├── Scenes/
    │   ├── MainMenu
    │   ├── Level_01
    │   └── _Test           ← тестовая сцена, не для билда
    ├── Materials/
    ├── Audio/
    │   ├── SFX/
    │   └── Music/
    ├── VFX/
    └── Animations/
```

---

## 2. Соглашения по именованию

### Файлы и классы

```
PlayerController.cs       ← PascalCase для классов
EnemyAI.cs
WeaponData.cs             ← ScriptableObject — суффикс Data
HealthComponent.cs        ← компонент — суффикс Component
IDamageable.cs            ← интерфейс — префикс I
GameEvent.cs              ← событие — суффикс Event
```

### Переменные

```csharp
// приватные — camelCase с _
private float _currentHealth;
private bool _isGrounded;

// публичные / serialized — camelCase без _
[SerializeField] private float moveSpeed;
[SerializeField] private int maxHealth;

// константы — UPPER_SNAKE_CASE
private const float COYOTE_TIME = 0.15f;
private const int MAX_AMMO = 30;
```

### GameObject'ы в сцене

```
Player                    ← PascalCase
Enemy_Drone_01            ← тип + порядковый номер
Door_Main_Entrance        ← тип + описание
Trigger_BossArena         ← тип + описание
```

### Слои (Layers)

```
Default
Player
Enemy
Projectile
Ground
Interactable
Ignore Raycast
```

### Теги (Tags)

```
Player
Enemy
Interactable
Checkpoint
Pickup
```

---

## 3. Архитектура систем

### 3.1 Игрок

```
Player (GameObject)
├── PlayerController.cs     ← движение, прыжок, бег, приседание
├── PlayerHealth.cs         ← HP, броня, получение урона, смерть
├── PlayerWeapon.cs         ← текущее оружие, стрельба, перезарядка
├── PlayerInventory.cs      ← слоты оружий, переключение
├── PlayerInteract.cs       ← raycast для E-взаимодействия
└── PlayerInput.cs          ← Input System, маппинг клавиш
```

**Паттерн:** Компонентная архитектура. Каждая система — отдельный компонент. Не один God Object с 1000 строк.

### 3.2 Здоровье и урон

```csharp
// Интерфейс — любой объект который можно повредить
public interface IDamageable
{
    void TakeDamage(float amount, DamageType type);
    bool IsAlive { get; }
}

// Тип урона
public enum DamageType { Bullet, Explosion, Melee }

// Компонент здоровья — вешается на Player и Enemy
public class HealthComponent : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxArmor  = 100f;

    private float _currentHealth;
    private float _currentArmor;

    public void TakeDamage(float amount, DamageType type)
    {
        // броня поглощает первой
        if (_currentArmor > 0)
        {
            float absorbed = Mathf.Min(_currentArmor, amount * 0.7f);
            _currentArmor -= absorbed;
            amount        -= absorbed;
        }
        _currentHealth -= amount;
        if (_currentHealth <= 0) Die();
    }
}
```

### 3.3 Оружие — ScriptableObject паттерн

```csharp
// Данные оружия — ScriptableObject
[CreateAssetMenu(menuName = "Iron Core/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public float  damage;
    public float  fireRate;
    public int    magazineSize;
    public float  reloadTime;
    public float  range;
    public float  recoilAmount;
}

// Контроллер оружия — использует данные из SO
public class WeaponController : MonoBehaviour
{
    [SerializeField] private WeaponData data;

    public void Shoot()
    {
        // Raycast выстрел
        if (Physics.Raycast(Camera.main.transform.position,
                            Camera.main.transform.forward,
                            out RaycastHit hit, data.range))
        {
            if (hit.collider.TryGetComponent<IDamageable>(out var target))
                target.TakeDamage(data.damage, DamageType.Bullet);
        }
    }
}
```

### 3.4 Enemy AI — State Machine

```
EnemyAI (StateMachine)
├── PatrolState     ← ходит по waypoints
├── DetectState     ← заметил игрока
├── ChaseState      ← преследует NavMesh
└── AttackState     ← атакует в радиусе
```

```csharp
public class EnemyAI : MonoBehaviour
{
    private enum State { Patrol, Detect, Chase, Attack }
    private State _currentState = State.Patrol;

    private NavMeshAgent _agent;

    private void Update()
    {
        switch (_currentState)
        {
            case State.Patrol: Patrol(); break;
            case State.Chase:  Chase();  break;
            case State.Attack: Attack(); break;
        }
    }

    private void TransitionTo(State newState) => _currentState = newState;
}
```

### 3.5 Интерактивные объекты

```csharp
// Интерфейс
public interface IInteractable
{
    void Interact(PlayerInteract player);
    string InteractPrompt { get; }  // текст подсказки "Открыть дверь"
}

// Реализация двери
public class Door : MonoBehaviour, IInteractable
{
    public string InteractPrompt => "Открыть дверь";

    public void Interact(PlayerInteract player)
    {
        // анимация открытия
    }
}
```

### 3.6 Events — развязка систем

Вместо прямых ссылок между системами используем события:

```csharp
// GameEvent ScriptableObject
[CreateAssetMenu(menuName = "Iron Core/Game Event")]
public class GameEvent : ScriptableObject
{
    private List<GameEventListener> _listeners = new();

    public void Raise()
    {
        foreach (var listener in _listeners)
            listener.OnEventRaised();
    }
}

// Примеры событий (Assets/ScriptableObjects/Events/)
// OnPlayerDied.asset
// OnCheckpointReached.asset
// OnEnemyKilled.asset
```

### 3.7 GameManager

```csharp
// Синглтон — единственный на сцене
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    public enum GameState { Playing, Paused, Dead, Win }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
        Time.timeScale = state == GameState.Paused ? 0f : 1f;
    }
}
```

### 3.8 Checkpoint и сохранения

```csharp
[System.Serializable]
public class SaveData
{
    public Vector3 playerPosition;
    public float   playerHealth;
    public float   playerArmor;
    public string  activeWeapon;
    public string  lastCheckpointId;
}

public class SaveSystem : MonoBehaviour
{
    private const string SAVE_KEY = "iron_core_save";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SAVE_KEY, json);
        PlayerPrefs.Save();
    }

    public static SaveData Load()
    {
        string json = PlayerPrefs.GetString(SAVE_KEY, "");
        return string.IsNullOrEmpty(json)
            ? new SaveData()
            : JsonUtility.FromJson<SaveData>(json);
    }
}
```

---

## 4. Cinemachine настройка

```
CinemachineBrain (на Main Camera)
└── CM_PlayerCam (Virtual Camera)
    ├── Follow: Player/CameraTarget
    ├── Look At: null (FPS — смотрим через мышь)
    ├── Noise: Basic Multi Channel Perlin (для head-bob)
    └── CinemachineImpulseListener (для camera shake)

// Camera shake при взрыве
[SerializeField] private CinemachineImpulseSource _impulseSource;

public void Shake(float force)
{
    _impulseSource.GenerateImpulse(force);
}
```

---

## 5. Слои и коллизии

|                | Default | Player | Enemy | Projectile | Ground | Interactable |
| -------------- | ------- | ------ | ----- | ---------- | ------ | ------------ |
| **Default**    | ✓       | ✓      | ✓     | ✓          | ✓      | ✓            |
| **Player**     | ✓       | ✗      | ✓     | ✗          | ✓      | ✓            |
| **Enemy**      | ✓       | ✓      | ✗     | ✓          | ✓      | ✗            |
| **Projectile** | ✓       | ✗      | ✓     | ✗          | ✓      | ✗            |

**Raycast маски:**

```csharp
// для выстрела — попадаем во врагов и окружение, не в игрока
LayerMask shootMask = LayerMask.GetMask("Enemy", "Default", "Ground");

// для E-взаимодействия — только интерактивные объекты
LayerMask interactMask = LayerMask.GetMask("Interactable");
```

---

## 6. Производительность

### Пулинг объектов

```csharp
// Пули — не Instantiate/Destroy, а пул
public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int        poolSize = 20;

    private Queue<GameObject> _pool = new();

    private void Awake()
    {
        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(bulletPrefab);
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            var obj = _pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        return Instantiate(bulletPrefab);
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        _pool.Enqueue(obj);
    }
}
```

### Правила

- Не использовать `FindObjectOfType` в `Update` — только в `Awake/Start`
- Кешировать компоненты в `Awake`: `_rb = GetComponent<Rigidbody>()`
- VFX через VFX Graph, не старый Particle System для сложных эффектов
- LOD для дальних объектов когда появятся большие уровни

---

## 7. Сцены

```
MainMenu    ← только UI, никакой игровой логики
Level_01    ← первый уровень
_Test       ← тестовая сцена для отдельных механик (не в билд)
```

**Загрузка сцен:**

```csharp
// Async загрузка без фриза
SceneManager.LoadSceneAsync("Level_01");
```

---

## 8. Git соглашения

### Ветки

```
main        ← стабильная версия
dev         ← разработка
feature/*   ← новые фичи
fix/*       ← исправления
```

### Commit messages (Conventional Commits)

```
feat: добавить систему здоровья игрока
fix: исправить coyote time при прыжке с края
refactor: вынести логику урона в HealthComponent
docs: обновить TDD
```

### .gitignore

```
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uild/
/[Bb]uilds/
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db
/[Aa]ssets/[Pp]lugins/Android/
*.unitypackage
```

---

## 9. Билд настройки

**Target Platform:** Windows x64
**Rendering Pipeline:** HDRP
**Scripting Backend:** IL2CPP
**Api Compatibility Level:** .NET Standard 2.1

**Player Settings:**

```
Company Name: Alexander Gulidov
Product Name: Iron Core
Version: 0.1.0
```

---

## 10. Зависимости (Packages)

| Package      | Версия        | Назначение              |
| ------------ | ------------- | ----------------------- |
| Cinemachine  | 3.x           | Камера, shake, head-bob |
| ProBuilder   | 6.x           | Левел-дизайн геометрия  |
| DOTween      | 1.2.x         | Анимации UI и объектов  |
| Input System | 1.7.x         | Ввод клавиатура/мышь    |
| NavMesh      | built-in      | Навигация врагов        |
| VFX Graph    | built-in HDRP | Визуальные эффекты      |
| TextMeshPro  | built-in      | UI текст                |

---

## 11. Известные технические решения

### Coyote Time (прыжок с края)

```csharp
private float _coyoteTimeCounter;
private const float COYOTE_TIME = 0.15f;

private void Update()
{
    if (IsGrounded()) _coyoteTimeCounter = COYOTE_TIME;
    else _coyoteTimeCounter -= Time.deltaTime;

    if (Input.GetKeyDown(KeyCode.Space) && _coyoteTimeCounter > 0)
        Jump();
}
```

### FOV kick при беге

```csharp
private void UpdateFOV()
{
    float targetFOV = _isSprinting ? sprintFOV : normalFOV;
    Camera.main.fieldOfView = Mathf.Lerp(
        Camera.main.fieldOfView, targetFOV, Time.deltaTime * fovSpeed);
}
```

### Radius Damage (взрыв)

```csharp
public static void ApplyExplosionDamage(
    Vector3 center, float radius, float maxDamage)
{
    Collider[] hits = Physics.OverlapSphere(center, radius);
    foreach (var hit in hits)
    {
        if (!hit.TryGetComponent<IDamageable>(out var target)) continue;

        float dist   = Vector3.Distance(center, hit.transform.position);
        float falloff = 1f - Mathf.Clamp01(dist / radius);
        target.TakeDamage(maxDamage * falloff, DamageType.Explosion);
    }
}
```

---

_Документ обновляется по мере разработки._
