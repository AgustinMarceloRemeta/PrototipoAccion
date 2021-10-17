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
    [SerializeField] private GameObject[] enemigos;
    [SerializeField] public Transform lobbyCamera, player;
    private Enemy enemy;
    private Player jugador;
    public int Kills= 0;
    public Text ContadorKills;
    public Text Vida;
    public float life = 100f;
    public List<Transform> red;
    public List<Transform> orange;
    public List<Transform> black;
    public List<Transform> white;
    public Transform Spawn;
    public int Muertes= 0;
    public int Score = 0;
    public int KillsRed = 0;
    public int KillsWhite = 0;
    public int KillsBlack = 0;
    public int KillsOrange = 0;
    public int bonus;
    public Text MuertesFinal;
    public Text ScoreFinal;
    public Text textKillsFinal;
    public Text textKillsRed;
    public Text textKillsWhite;
    public Text textKillsBlack;
    public Text textKillsOrange;
    public Text explicacion;
    public Text textScore;
    public Text textBonus;
    public GameObject star;




    #endregion

    private void start()
    {
        jugador = FindObjectOfType<Player>();
        Vida.text = "Vida: 100";
        explicacion.text = " ";
        textScore.text = "Score: 0";
        ContadorKills.text = "Kills: 0";
        

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
        if (timer <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            BeginMatch();
            Reset();
        }

        if(lobbyCamera.gameObject.activeSelf) 
            lobbyCamera.Rotate(5f * Time.deltaTime * Vector3.up);

        if (!timing && timer <= 0) return;
        if (_timer > 0) _timer -= Time.deltaTime;
        else { _timer = 1f; Tick(); }
        ContadorKills.text = ("Kills: " + Kills);
        Vida.text = ("Vida: " + life);
        textScore.text = "Score: " + Score;
        explicacion.text = " ";
        if (timer == 0) {
            MuertesFinal.text = "Muertes:" + Muertes + "(-"+ Muertes*5+ "p)" ;
            ScoreFinal.text = "Score:" + Score + "p";
            textKillsBlack.text = "Kills Black:" + KillsBlack + "(" + KillsBlack *10 + "p)";
            textKillsRed.text = "Kill Red:" + KillsRed + "(" + KillsRed*7 + "p)";
            textKillsOrange.text = "Kill Orange:" + KillsOrange + "(" + KillsOrange * 5 + "p)";
            textKillsWhite.text = "Kills White:" + KillsWhite + "(" + KillsWhite * 2 + "p)";
            textKillsFinal.text = "kills Totales:" + Kills;
            textBonus.text = "Bonus: " + bonus + "p";
            Vida.text = "";
            ContadorKills.text = "";
            textScore.text = "";
            
        }


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
        Spawn = spawners[rdm];
        for (int i = 0; i <spawners.Count; i++)
        {
            if (i <= 2) { red.Add(spawners[i]);}
            if (i <= 5 && i>2) { white.Add(spawners[i]);}
            if (i <= 8 && i > 5) { black.Add(spawners[i]);}
            if (i <= 11 && i > 8) { orange.Add(spawners[i]);}
        }
       
        foreach (var spawner in white)
              Instantiate(enemigos[0], spawner.position, enemigos[0].transform.rotation);
        foreach (var spawner in red)
            Instantiate(enemigos[1], spawner.position, enemigos[1].transform.rotation);
        foreach (var spawner in orange)
            Instantiate(enemigos[2], spawner.position, enemigos[2].transform.rotation);
        foreach (var spawner in black)
            Instantiate(enemigos[3], spawner.position, enemigos[3].transform.rotation); 
        
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
    private void Reset()
    {
        Muertes = 0;
        Score = 0;
        KillsRed = 0;
        KillsWhite = 0;
        KillsBlack = 0;
        KillsOrange = 0;
        bonus = 0;
        life = 100f;
        Kills = 0;
        MuertesFinal.text = "";
        ScoreFinal.text = "";
        textKillsFinal.text = "";
        textKillsRed.text = "";
        textKillsWhite.text = "";
        textKillsBlack.text = "";
        textKillsOrange.text = "";
        textScore.text = "";
        textBonus.text = "";
        Instantiate(star, new Vector3(7.87f, 1.8f, -2.53f), Quaternion.identity);
        Instantiate(star, new Vector3(-7.3f, 1.8f, -2.53f), Quaternion.identity);
        Instantiate(star, new Vector3(-7.3f, 1.8f, 3.6f), Quaternion.identity);
        Instantiate(star, new Vector3(7.87f, 1.8f, 3.6f), Quaternion.identity);
    }
    #endregion


}
