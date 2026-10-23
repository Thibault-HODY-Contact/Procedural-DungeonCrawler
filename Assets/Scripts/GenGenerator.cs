using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenGenerator : MonoBehaviour
{
    [SerializeField] GameObject GeneratorPrefab;

    [SerializeField] int numberLEveles;

    [SerializeField] List<GameObject> GenList = new List<GameObject>() ;
    List<GameObject> LadderGenList = new List<GameObject>();

    [SerializeField] GameObject Ladder;

    [SerializeField] private Transform player;

    [ContextMenu("Generate")]
    void Generate() {
        if(GenList.Count != 0)
        {
            foreach (GameObject objet in GenList)
            {if (objet != null)
                {
                    objet.GetComponent<DungeonGeneratorBSP>().CleanRoom();
                    DestroyImmediate(objet);
                }
            }
        }
        if (LadderGenList.Count != 0)
        {
            foreach (GameObject objet in LadderGenList)
            {
                if (objet != null)
                {
                    DestroyImmediate(objet);
                }
            }
        }


        GenList.Clear();
        LadderGenList.Clear();

        for (int i = 0; i < numberLEveles; i++)
        {
            GenList.Add(Instantiate(GeneratorPrefab));

        }
        foreach (GameObject objet in GenList)
        {
            objet.GetComponent<DungeonGeneratorBSP>().Generate();
            objet.GetComponent<DungeonGeneratorBSP>().RandomSeed =false;
        }
        for (int i = 0; i < numberLEveles; i++)
        {
            DungeonGeneratorBSP level = GenList[i].GetComponent<DungeonGeneratorBSP>();


            if(i>0)
            {
                DungeonGeneratorBSP Prevlevel = GenList[i - 1].GetComponent<DungeonGeneratorBSP>();

                Vector2 distance = level.startRoom.roomBound.Value.center - Prevlevel.endRoom.roomBound.Value.center;

                GenList[i].transform.position -= new Vector3(distance.x, 0, distance.y);
                GenList[i].transform.position += new Vector3(0, GenList[i-1].transform.position.y+15, 0);
                GenList[i].GetComponent<DungeonGeneratorBSP>().Generate();

            }
            LadderGenList.Add(Instantiate(Ladder, new Vector3(level.endRoom.roomBound.Value.center.x, level.transform.position.y, level.endRoom.roomBound.Value.center.y),level.transform.rotation));

            level.buildRoom();
        }
        PlacePlayer();
    }
    private void PlacePlayer()
    {
        DungeonGeneratorBSP firstLevel = GenList[0].GetComponent<DungeonGeneratorBSP>();

        Vector2 center = firstLevel.startRoom.roomBound.Value.center;

        player.position = new Vector3(
            center.x,
            firstLevel.transform.position.y + 1f,
            center.y
        );
    }
}
