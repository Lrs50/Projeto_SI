using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA 
{
    public List<int> genes = new List<int>();

    public DNA(int genomeLength = 50){

        for(int i=0;i<genomeLength;i++){
            genes.Add(Random.Range(0,4));
        }

    }

    public DNA(DNA parent,DNA partner,float mutationRate=0.05f){
        for(int i=0;i<parent.genes.Count;i++){
            float mutationChance = Random.Range(0f,1f);
            if(mutationChance<=mutationRate){
                genes.Add(Random.Range(0,4));
            }else{
                int chance = Random.Range(0,2);
                if(chance==1){
                    genes.Add(parent.genes[i]);
                }else{
                    genes.Add(partner.genes[i]);
                }
            }
        }
    } 

}
