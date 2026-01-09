using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int playerLives;
    public PlayerMove player;
    public GameObject[] Stages;

    public Image[] UIPlayerLives;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;


    private void Start()
    {

    }
    private void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
/*
        //Chage Stage
        if (stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE " + (stageIndex + 1);
        }
        else 
        {
            //Game Clear
            //Player Control Lock
            Time.timeScale = 0;
            //Result UI
            Debug.Log("게임 클리어!");
            //Restart Button UI
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            UIRestartBtn.SetActive(true);
        }

        //Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
*/
    }

    public void DownPlayerLives()
    {
        if (playerLives > 1)
        {
            playerLives--;
            UIPlayerLives[playerLives].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            //All playerLives UI Off
            UIPlayerLives[0].color = new Color(1, 0, 0, 0.4f);

            //Player Die Effect
            player.OnDie();

            //Result UI
            Debug.Log("죽었습니다.!");

            //Retry BUtton UI
            UIRestartBtn.SetActive(true);

            EventManager.Instance.TriggerPlayerLivesZero();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Player Reposition
            if (playerLives > 1)
            {
                PlayerReposition();
            }


            //Down playerLives
            DownPlayerLives();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-7, 0, -1);  //시작 위치
        player.VelocityZero();
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
}
