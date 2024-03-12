using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class Mission : MonoBehaviour
{
    public TextMeshProUGUI missionCount;
    public TextMeshProUGUI missionBrief;

    new BoxCollider2D col;
    new Transform transform;

    string mission;
    int goalCount;
    int count;
    int child;

    bool onMission;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider2D>();
        transform = GetComponent<Transform>();

        goalCount = 0;
        count = 0;
        child = 6;
        onMission = false;
        for (int i = 0; i < 10; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (goalCount != 0 && goalCount >= count)
        {
            MissionDone();
        }

        switch (mission)
        {
            case "Circle": //Green Cube: Health Up
                missionBrief.text = "Send Medical Supplies for Support";
                child = 0;
                break;

            case "Triangle": //Donut: Time Stop
                missionBrief.text = "????????????????????????????";
                child = 1;
                break;

            case "9-Sliced": //Silver Sphere: MinionAttack
                missionBrief.text = "Scavange Minions to make Allies";
                child = 2;
                break;

            case "Hexagon Pointed-Top": //BlueDode: AttackStall
                missionBrief.text = "Provide Electricity for Shelters";
                child = 3;
                break;

            case "Hexagon Flat-Top": //Brown Dode: RobotAttack
                missionBrief.text = "Send Ammo to Replenish Soldiers";
                child = 4;
                break;

            default:
                missionBrief.text = "";
                missionCount.text = "";
                break;
        }

        if (onMission)
        {
            gameObject.tag = mission;
            missionCount.text = (goalCount - count) + " Needed";
            transform.GetChild(0).transform.GetChild(child).gameObject.SetActive(true);
        }


    }

    public void MissionSet(string missionName, int missionGoal)
    {
        mission = missionName;
        goalCount = missionGoal;
        onMission = true;
    }

    private void MissionDone()
    {
        onMission = false;
        transform.GetChild(0).transform.GetChild(child).gameObject.SetActive(false);
        mission = "";
        missionBrief.text = "";
        missionCount.text = "";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == this.gameObject.tag)
        {
            count++;
        }
    }
}