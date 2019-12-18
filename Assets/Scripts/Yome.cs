using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Yome : MonoBehaviour, ISaveableLoadable
{
    [SerializeField] private GameObject PlayertToFollow;
    [SerializeField] private float NoticeDistance = 10f;
    [SerializeField] private TextMeshProUGUI UI;
    private Vector3 target;

    //Saved
    private int AffectionLevel = 0;
    [SerializeField] private float ClosenessPointInterval = 10f;
    [SerializeField] private float HeadpatCooldown = 5f;
    private float PlayerClosenessTime = 0f;
    private float LastHeadpatTime = 10f;

    [Header("Different states")]
    [SerializeField] Renderer Front;
    [SerializeField] Renderer Back;
    [SerializeField] Material NormalFront;
    [SerializeField] Material NormalBack;
    [SerializeField] Material HappyFront;
    [SerializeField] Material HappyBack;
    [SerializeField] Material TalkingFront;
    [SerializeField] Material TalkingBack;
    [SerializeField] Material Talking2Front;
    [SerializeField] Material Talking2Back;
    [SerializeField] Material Happy100Front;
    [SerializeField] Material Happy100Back;

    private Coroutine coroutine;

    [Header("Dialogue")]
    string[] headpat = new string[]
    {
        "Fufu, that tickles!",
        "You really like doing that, dont you!",
        "Do you like my hair that much?"
    };
    string[] spendtime = new string[]
    {
        "Were together an awful lot lately... I think its a good thing!",
        "If you ever need company, I'll always be here for you!",
        "It's fun to hang around with you!"
    };
    string[] flowergiven = new string[]
    {
        "Waa, thank you!",
        "It's so pretty!",
        "Wow! Where did you find this?"
    };
    string[] random = new string[]
    {
        "Life might be hard, but I'll always be here for you",
        "Have you seen the flowers around here? They are sooo pretty!",
        "It's allright to feel bad. You can't stay strong all the time.",
        "Remember to take a break if life feels hard on you.",
        "All things will get better in time.",
        "I wonder how did we get on this island... where are, exactly?",
        "Be gentle on yourself. No one is perfect.",
        "If you want to rest at night, just go inside. Just mind the door, its a bit shakey!"
    };

    private void Start()
    {
        if (UI == null)
            Debug.LogError("Yome does not have an associated UI text");
        else
            UI.text = "";
    }

    void Update()
    {
        if (Vector3.Distance(transform.position, PlayertToFollow.transform.position) < NoticeDistance)
        {
            PlayerClosenessTime += Time.deltaTime;
            LastHeadpatTime += Time.deltaTime;

            if (PlayerClosenessTime > ClosenessPointInterval)
            {
                IncreaseAffection(5);
                PlayerClosenessTime = 0f;
                ClosenessPointInterval++;

                //TODO: display time spent dialogue
                //string temp;
                //if (spendtime.Length == 0)
                //    temp = $"Were together awful lot lately... I think its a good thing!";
                //else
                //    temp = spendtime[Mathf.RoundToInt(Random.Range(0, spendtime.Length))];
                //UI.text = temp + $" ({AffectionLevel})";
                //StopCoroutine(coroutine);
                //coroutine = StartCoroutine(ResetText());
                if(AffectionLevel < 100)
                    Dialoguething(spendtime, HappyFront, HappyBack);
                else
                    Dialoguething(spendtime, Happy100Front, Happy100Back);
            }

            //Due to this Yome tips over anyway...
            transform.LookAt(PlayertToFollow.transform);
        }
        else
            PlayerClosenessTime = 0f;
    }

    public void LoadDefault()
    {
        //Empty
    }

    //TODO: call this from pressing Yome
    public void TalkTo()
    {
        //string temp;
        //if (random.Length == 0)
        //    temp = $"What is it!";
        //else
        //    temp = random[Mathf.RoundToInt(Random.Range(0, random.Length))];
        //UI.text = temp + $" ({AffectionLevel})";
        //StopCoroutine(coroutine);
        //coroutine = StartCoroutine(ResetText());

        Material tempfront;
        Material tempback;

        if (Random.value < 0.5)
        {
            tempfront = TalkingFront;
            tempback = TalkingBack;
        }
        else
        {
            tempfront = Talking2Front;
            tempback = Talking2Back;
        }
        Dialoguething(random, tempfront, tempback);
    }

    public void PatHead()
    {
        if(LastHeadpatTime > HeadpatCooldown)
        {
            IncreaseAffection(1);
            HeadpatCooldown++;
            LastHeadpatTime = 0f;

            //TODO: display dialogue for headpat
            //string temp;
            //if (headpat.Length == 0)
            //    temp = $"Fufu, that tickles!";
            //else
            //    temp = headpat[Mathf.RoundToInt(Random.Range(0, headpat.Length))];
            //UI.text = temp + $" ({AffectionLevel})";
            //StopCoroutine(coroutine);
            //coroutine = StartCoroutine(ResetText());

            if(AffectionLevel < 100)
                Dialoguething(headpat, HappyFront, HappyBack);
            else
                Dialoguething(headpat, Happy100Front, Happy100Back);
        }
    }

    public void GiveFlower()
    {
        IncreaseAffection(10);

        //TODO: display dialogue for flower giving
        //string temp;
        //if (flowergiven.Length == 0)
        //    temp = $"Waa, thank you!";
        //else
        //    temp = flowergiven[Mathf.RoundToInt(Random.Range(0, flowergiven.Length))];
        //UI.text = temp + $" ({AffectionLevel})";
        //StopCoroutine(coroutine);
        //coroutine = StartCoroutine(ResetText());

        if(AffectionLevel < 100)
            Dialoguething(flowergiven, HappyFront, HappyBack);
        else
            Dialoguething(flowergiven, Happy100Front, Happy100Back);
    }

    private void Dialoguething(string[] dialogue, Material front, Material back)
    {
        string temp;
        if (dialogue.Length == 0)
            temp = "Fufun";
        else
            temp = dialogue[Mathf.RoundToInt(Random.Range(0, dialogue.Length))];
        UI.text = temp + $" ({AffectionLevel})";
        Front.material = front;
        Back.material = back;

        if(coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(ResetText());
    }

    private void IncreaseAffection(int increase)
    {
        Front.material = HappyFront;
        Back.material = HappyBack;
        AffectionLevel += increase;
    }

    IEnumerator ResetText()
    {
        yield return new WaitForSeconds(5f);
        Front.material = NormalFront;
        Back.material = NormalBack;
        UI.text = "";
    }

    public SerializableData GetData()
    {
        return new YomeData(AffectionLevel, ClosenessPointInterval, HeadpatCooldown, PlayerClosenessTime, LastHeadpatTime);
    }

    public void LoadData(SerializableData data)
    {
        var temp = (YomeData)data;
        AffectionLevel = temp.Affection;
        ClosenessPointInterval = temp.ClosenessInterval;
        HeadpatCooldown = temp.HeadpatInterval;
        PlayerClosenessTime = temp.ClosenessTime;
        LastHeadpatTime = temp.HeadpatTime;
    }

    [System.Serializable]
    private class YomeData : SerializableData
    {
        public readonly int Affection;
        public readonly float ClosenessInterval, HeadpatInterval, ClosenessTime, HeadpatTime;

        public YomeData(int affection, float closenessinterval, float headpatinterval, float closenesstime, float headpattime)
        {
            this.Affection = affection;
            this.ClosenessInterval = closenessinterval;
            this.HeadpatInterval = headpatinterval;
            this.ClosenessTime = closenesstime;
            this.HeadpatTime = headpattime;
        }
    }
}
