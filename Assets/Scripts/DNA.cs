using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNA 
{   
    
    public List<int> genes = new List<int>();
    
    //Created a random DNA
    // The DNA represents which neightbor of the node the agent should move to
    public DNA(int genomeLength = 50){
        
        for(int i=0;i<genomeLength;i++){
            genes.Add(Random.Range(0,4));
        }

    }

    //Creates a DNA based on the parent and partner DNAs and some mutation constant
    public DNA(DNA parent,DNA partner,float mutationRate=0.02f){

        for(int i=0;i<parent.genes.Count;i++){
            float mutationChance = Random.Range(0f,1f);
            if(mutationChance<=mutationRate){
                genes.Add(Random.Range(0,4));
            }else{
                int choice = Random.Range(0,2);
                if(choice==0){
                    genes.Add(parent.genes[i]);
                }else{
                    genes.Add(partner.genes[i]);
                }

            }
        }
    } 

}
