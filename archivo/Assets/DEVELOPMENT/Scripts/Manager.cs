using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    #region Variables
    public int timer; private float _timer; private bool timing;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Lobby")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform lobbyCamera, player;
    private Enemy enemy;
    private Player jugador;
    public int Kills= 0;
    public Text ContadorKills;
    public Text Vida;
    public float life = 100f;
   
    #endregion

    private void start()
    {
        jugador = FindObjectOfType<Player>();
        Vida.text = ("Vida: 100");
    }

    #region Audio
    [Header("Audio")]
    [SerializeField] AudioClip[] audioClips;
    private AudioSource source;
    private AudioClip[] Get(params string[] identifiers) => System.Array.FindAll(audioClips, clip => Keywords(clip.name, identifiers));
    private AudioClip GetOne(params string[] identifiers)
    {
        AudioClip[] clips = Get(identifiers);
        return clips[Random.Range(0, clips.Length)];
    }

    public bool Keywords(string word, params string[] keywords)
    {
        foreach (string k in keywords)
            if (!word.Contains(k)) return false;
        return true;
    }

    public void PlaySound(AudioClip clip)
    {
        if (source.isPlaying) return;
        source.clip = clip;
        source.Play();
    }

    public void PlaySoundPriority(AudioClip clip)
    {
        source.Stop();
        source.clip = clip;
        source.Play();
    }

    public void PlaySound(params string[] identifiers) => PlaySound(GetOne(identifiers));
    public void PlaySoundPriority(params string[] identifiers) => PlaySoundPriority(GetOne(identifiers));
    #endregion

    #region Core
    private void Awake() => source = gameObject.AddComponent<AudioSource>();

    private void Update()
    {
        if (timer <= 0 && Input.GetKeyDown(KeyCode.Return)) BeginMatch();

        if(lobbyCamera.gameObject.activeSelf) 
            lobbyCamera.Rotate(5f * Time.deltaTime * Vector3.up);

        if (!timing && timer <= 0) return;
        if (_timer > 0) _timer -= Time.deltaTime;
        else { _timer = 1f; Tick(); }
        ContadorKills.text = ("Kills:" + Kills);
        Vida.text = ("Vida:" + life);
        
    }

    private void Tick()
    {
        timer--;
        if (timer == 60) PlaySoundPriority("60s");
        if (timer == 30) PlaySoundPriority("30s");
        if (timer == 0)
        {
            PlaySoundPriority("timeout");
            EndMatch();
        }
    }
    #endregion

    #region Match
    public void SpawnAll()
    {
        List<Transform> spawners = spawnPoints.ToList();
        int rdm = Random.Range(0, spawnPoints.Length);
        player.position = spawners[rdm].position + Vector3.up;
        player.gameObject.SetActive(true);
        lobbyCamera.gameObject.SetActive(false);
        spawners.RemoveAt(rdm);
        foreach (var spawner in spawners)
            Instantiate(enemyPrefab, spawner.position, enemyPrefab.transform.rotation);
        timing = true;
        timer = 90;
    }

    public void BeginMatch() => StartCoroutine(IBeginMatch());

    private IEnumerator IBeginMatch()
    {
        AudioClip clip = GetOne("start");
        PlaySoundPriority(clip);
        yield return new WaitForSeconds(clip.length + 1f);
        SpawnAll();
    }

    public void EndMatch()
    {
        timing = false;
        foreach (var e in FindObjectsOfType<Enemy>())
            Destroy(e.gameObject);
        player.gameObject.SetActive(false);
        lobbyCamera.gameObject.SetActive(true);
    }

    public void QueueSpawn(Transform toSpawn) => StartCoroutine(IQueueSpawn(toSpawn));
    private IEnumerator IQueueSpawn(Transform toSpawn)
    {
        yield return new WaitForSeconds(Random.Range(3f, 5f));
        if (toSpawn == null) yield break;
        toSpawn.position = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        if (toSpawn.TryGetComponent(out Player player)) player.ItsAlive();
        if (toSpawn.TryGetComponent(out Enemy enemy)) enemy.ItsAlive();
    }
    #endregion

    
}
