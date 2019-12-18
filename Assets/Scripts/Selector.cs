using TMPro;
using UnityEngine;

public class Selector : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI UI;
    [SerializeField] Yome yome;
    private GameObject CurrentCollider;

    private GameObject HeldFlower;

    private void Start()
    {
        UI.text = "";
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && CurrentCollider != null)
        {
            switch (CurrentCollider.tag)
            {
                case "Door":
                    if(DayNightSystem.GetRotation() > 90)
                    {
                        UI.text = "Sleeping until morning...";
                        DayNightSystem.SkipToMorning();
                    }
                    break;
                case "Yome":
                    if (HeldFlower == null)
                        yome.TalkTo();
                    else
                    {
                        yome.GiveFlower();
                        Destroy(HeldFlower.gameObject);
                        HeldFlower = null;
                        UI.text = "You give Yome a flower";
                    }
                    break;
                case "YomeHead":
                    UI.text = "You pat Yome on the head";
                    yome.PatHead();
                    break;
                case "Flower":
                    //TODO: pick flower up to give to Yome

                    HeldFlower = CurrentCollider.gameObject;
                    HeldFlower.transform.SetParent(this.transform.parent);

                    UI.text = "You pick up a flower";
                    break;
                default:
                    UI.text = "You can do nothing with that";
                    break;
            }


            Debug.Log($"Clicked on {CurrentCollider.name}");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CurrentCollider = other.gameObject;
        switch (other.tag)
        {
            case "Door":
                //Frickin eulerAngles, man!
                if (DayNightSystem.GetRotation() > 90)
                    UI.text = "Sleep";
                break;
            case "Yome":
                UI.text = "Yome";
                break;
            case "YomeHead":
                UI.text = "Pat Yome";
                break;
            case "Flower":
                UI.text = "Flower";
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        UI.text = "";
    }
}
