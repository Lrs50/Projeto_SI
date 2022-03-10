using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


// Basic heap class created with C# double linked list
public class Heap <Tkey,Tvalue>{
    private Func<Tkey,Tkey,bool> avaliate;
    private LinkedList<KeyValuePair<Tkey,Tvalue>> vector = new LinkedList<KeyValuePair<Tkey, Tvalue>>();

    public Heap(Func<Tkey,Tkey,bool> avaliate){
        this.avaliate = avaliate;
    }

    public void Add(Tkey key,Tvalue value){
        LinkedListNode<KeyValuePair<Tkey,Tvalue>> node = vector.First;
        LinkedListNode<KeyValuePair<Tkey,Tvalue>> nodeToAdd = new LinkedListNode<KeyValuePair<Tkey, Tvalue>>(new KeyValuePair<Tkey, Tvalue>(key,value));

        if(node == null){
            vector.AddFirst(nodeToAdd);
        }else{
            while(!avaliate(key,node.Value.Key))
            {
                node= node.Next;
                if(node==null)
                    break;
            }
            if(node==null){
            vector.AddLast(nodeToAdd);
            }else{
                vector.AddBefore(node,nodeToAdd);
            }
        }
    }

    public KeyValuePair<Tkey,Tvalue> Pop(){
        LinkedListNode<KeyValuePair<Tkey,Tvalue>> node = vector.First;
        vector.RemoveFirst();
        return node.Value;
    }

    public bool Empty(){
        return (vector.Count==0);
    }

    public override string ToString(){
        LinkedListNode<KeyValuePair<Tkey,Tvalue>> node = vector.First;
        string output = "";
        while(node!=null){
            output+= node.Value.ToString()+ " ";
            node= node.Next;
        }   

        return output;
    }

}