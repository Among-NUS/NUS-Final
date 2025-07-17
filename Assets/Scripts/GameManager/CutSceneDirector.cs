using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CutSceneDirector : MonoBehaviour
{
    public HeroActorBehaviour hero;
    public BossBehaviour boss;
    public GameObject choicePanel;
    public CreditsBehaviour credits;
    public GameObject myGameManager;
    private AudioSource gmAudioSource;
    public AudioClip Invisible;
    
    [Header("Dialog System")]
    public TextAsset DialogFile; 
    public GameObject dialogBox; 
    public TextMeshProUGUI englishTextLabel;
    public TextMeshProUGUI chineseTextLabel;
    public event Action OnCreditsFinished;
    
    // 简化的对话变量
    private string[] dialogLines;
    private int currentLine = 0;
    private bool isDialogPlaying = false;
    private bool hasAutoShown = false; // 是否已自动显示第一句
    public GameObject skipHintPanel;
    public string sceneName;

    // Start is called before the first frame update
    void Start()
    {
        // 确保对话框在游戏开始时隐藏
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }

        StartCoroutine(PlayCutScene());
        myGameManager = GameObject.Find("GameManager");
        gmAudioSource = myGameManager.GetComponent<AudioSource>();
        skipHintPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // 如果正在播放对话，不处理其他输入
        if (isDialogPlaying)
        {
            return;
        }
        
    }

    IEnumerator PlayCutScene()
    {
        yield return WalkHeroTime(1f);
        boss.BossTurn();
        yield return BossFrightend();
        yield return BossJump();
        yield return new WaitForSeconds(1f);

        if (DialogFile != null)
        {
            yield return PlayDialog();
        }

        int playerChoice = 0;
        choicePanel.SetActive(true);
        yield return WaitForPlayerChoice((choice) => playerChoice = choice);
        choicePanel.SetActive(false);
        if (playerChoice == 1)
        {
            yield return WalkHeroTime(2.5f);
            boss.BossTurn();
            yield return WalkTogether(5f);
        }
        else
        {
            yield return HeroShoot();
            yield return new WaitForSeconds(1f);
            yield return WalkHeroTime(7.5f);
        }
        gmAudioSource.clip = Invisible;
        gmAudioSource.Play();
        bool creditsFinished = false;
        credits.OnCreditsFinished += () => creditsFinished = true;
        credits.StartCredits();
        yield return new WaitForSeconds(10f);
        if (skipHintPanel != null) skipHintPanel.SetActive(true);
        while (!creditsFinished)
        {
            if (Input.GetKeyDown(KeyCode.U))
            {
                SceneManager.LoadScene("Congratulations");
                yield break; // 直接结束协程
            }
            yield return null;
        }
        SceneManager.LoadScene(sceneName);
    }
    
    IEnumerator PlayDialog()
    {
        // 开始对话播放
        hasAutoShown = true;
        isDialogPlaying = true;
        currentLine = 0;
        Time.timeScale = 0f; 
        LoadDialog();
        if (dialogLines.Length == 0) yield break;
        
        if (dialogBox != null)
            dialogBox.SetActive(true);
        
        // 播放所有对话
        while (currentLine < dialogLines.Length)
        {
            // 显示当前对话
            string line = dialogLines[currentLine].Trim();
            
            // 分离英文和中文
            string english = "";
            string chinese = "";
            
            for (int i = 0; i < line.Length; i++)
            {
                if (IsChinese(line[i]))
                {
                    english = line.Substring(0, i).Trim();
                    chinese = line.Substring(i).Trim();
                    break;
                }
            }
            
            if (string.IsNullOrEmpty(chinese))
            {
                english = line;
            }
            
            // 设置文本
            if (englishTextLabel != null)
                englishTextLabel.text = english;
            
            if (chineseTextLabel != null)
                chineseTextLabel.text = chinese;
            
            // 等待任意键输入（
            bool keyPressed = false;
            while (!keyPressed)
            {
                // 检查任意键输入
                if (Input.anyKeyDown)
                {
                    keyPressed = true;
                }
                yield return null;
            }
            
            // 短暂延迟避免按键重复触发
            yield return new WaitForSecondsRealtime(0.1f);
            
            currentLine++;
        }
        
        // 完成对话
        isDialogPlaying = false;
        if (dialogBox != null)
            dialogBox.SetActive(false);
        
        Time.timeScale = 1f; 
    }
    
    // 加载对话文件
    void LoadDialog()
    {
        if (DialogFile == null) return;
        
        // 简单按行分割
        dialogLines = DialogFile.text.Split('\n');
        currentLine = 0;
        isDialogPlaying = false;
    }
    
    // 判断是否为中文字符
    bool IsChinese(char c)
    {
        return c >= 0x4E00 && c <= 0x9FFF;
    }

    IEnumerator WalkHeroTime(float duration)
    {
        float t = 0;
        hero.MakeWalk();
        while (t < duration)
        {
            t += Time.deltaTime;
            if (hero.heroFacingRight) hero.transform.position += Vector3.right * hero.speed * Time.deltaTime;
            else hero.transform.position += Vector3.left * hero.speed * Time.deltaTime;
            yield return null;
        }
        hero.MakeStand();
        yield return null;

    }

    IEnumerator WalkBossTime(float duration)
    {
        float t = 0;
        boss.MakeWalk();
        while (t < duration)
        {
            t += Time.deltaTime;
            if (boss.bossFacingRight) boss.transform.position += Vector3.right * boss.speed * Time.deltaTime;
            else boss.transform.position += Vector3.left * boss.speed * Time.deltaTime;
            yield return null;
        }
        boss.MakeStand();
        yield return null;
    }

    IEnumerator HeroShoot()
    {
        float t = 0;
        while (t < 0.5f)
        {
            t += Time.deltaTime;
            yield return null;
        }
        hero.MakeShoot();
    }

    IEnumerator BossFrightend()
    {
        boss.BossAlert();
        float waitTime = 0f;
        while (waitTime < 0.5f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
        boss.BossUnAlert();
    }

    IEnumerator BossJump()
    {
        boss.Jump();
        float waitTime = 0f;
        while (waitTime < 0.5f)
        {
            waitTime += Time.deltaTime;
            yield return null;
        }
    }

    IEnumerator WalkTogether(float duration)
    {
        Coroutine heroWalk = StartCoroutine(WalkHeroTime(duration));
        Coroutine bossWalk = StartCoroutine(WalkBossTime(duration));
        yield return new WaitForSeconds(duration);
    }

    IEnumerator WaitForPlayerChoice(System.Action<int> onChoice)
    {
        bool waiting = true;

        while (waiting)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                onChoice?.Invoke(1);  // 选择1
                waiting = false;
            }
            else if (Input.GetKeyDown(KeyCode.J))
            {
                onChoice?.Invoke(2);  // 选择2
                waiting = false;
            }

            yield return null; // 等下一帧继续检测
        }
    }
}
