using System;
using System.Collections.Generic;
using UnityEngine;

public class GenGenerator : MonoBehaviour
{
    [SerializeField] GameObject GeneratorPrefab;

    [SerializeField] int numberLEveles;

    List<GameObject> GenList = new List<GameObject>() ;

    [ContextMenu("Generate")]
    void Generate() {
        if(GenList.Count != 0)
        {
            foreach (GameObject objet in GenList)
            {
                DestroyImmediate(objet);
            }
        }

        GenList.Clear();

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
            if(i == 0)
            {
                continue;
            }
            else
            {

                Vector2 distance = GenList[i].GetComponent<DungeonGeneratorBSP>().startRoom.room.Value.center - GenList[i-1].GetComponent<DungeonGeneratorBSP>().endRoom.room.Value.center;

                GenList[i].transform.position -= new Vector3(distance.x, 0, distance.y);
                GenList[i].transform.position += new Vector3(0, GenList[i-1].transform.position.y+25, 0);
                GenList[i].GetComponent<DungeonGeneratorBSP>().Generate();

            }
        }

    }
    
    

}
