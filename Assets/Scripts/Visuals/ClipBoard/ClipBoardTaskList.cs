using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class ClipBoardTaskList : MonoBehaviour
{
    public int curtask = 0;

    public int[] tasks;

    public int[] bigTasks;

    public Vector2[] smallTasks;

    public TextMeshPro TaskText;
    public string StartText;
    public string FinalText;

    public int smalltaskamount;
    public int taskamount;


    public Vector3 startpos;
    public Vector3 moveTo;

    void Start()
    {
        startpos = TaskText.transform.localPosition;
        moveTo = moveTo + startpos;
        StartText = TaskText.text;
        TaskText.text = StartText;

        int startAt = 4; //First line of text

        /*
        string[] someString = StartText.Split("\n");

        foreach (string s in someString)
        {
            print(s + "</s>");
        }
        */



        string[] curString = StartText.Split("\n");

        FinalText = "";

        /*
        for (int ta = 0; ta < curString.Length; ta++)
            if (curString[ta].Contains("\t"))
                tasks[ta] = ta;
        */


        foreach (string s in curString)
            if (s.Contains("\t"))
                smalltaskamount += 1;

        foreach (string s in curString)
            if (s.Contains("<b>"))
                taskamount += 1;

        smallTasks = new Vector2[smalltaskamount];
        bigTasks = new int[taskamount];

        int STask = 0;

        int BTask = 0;

        for (int i = 0; i < curString.Length; i++)
        {

            if (curString[i].Contains("<b>") && i != 0)
            {
                bigTasks[BTask] = i;
                BTask += 1;
            }
            
            if (curString[i].Contains("\t"))
            {
                smallTasks[STask] = new Vector2(BTask, i);
                STask += 1;
            }
            

        }
        /*
        for (int c = 0; c < curString.Length; c++)
        {
            if (curString[c].Contains("\t"))
            {
                smallTasks[STask] = new Vector2(bigTasks[BTask], c);
                STask += 1;
            }

        }*/



    }

    int lasttask;
    void Update()
    {
        //curtask = Mathf.Clamp(curtask, 0, smalltaskamount);
        if (lasttask != curtask)
        {
            GoTo(curtask);
            lasttask = curtask;//Mathf.Clamp(curtask,0,smalltaskamount);
        }
    }


    void GoTo(int line)
    {
        string[] curString = StartText.Split("\n");

        FinalText = "";

        TaskText.transform.localPosition = Vector3.Lerp(startpos, moveTo, (line * 1f / curString.Length * 1f));

        /*
        for (int cs = 0; cs < curString.Length; cs++)
            for (int i = 0; i < smallTasks.Length; i++)
                {
                if (smallTasks[i].y >= cs)
                {
                    FinalText += "<s>" + curString[cs] + "</s> \n";

                }
                else
                    FinalText += curString[cs] + "\n";

                //for (int c = curString[(int)smallTasks[i].y].Length; c >= 0; c--)
                //FinalText += "<s>" + curString[c] + "</s> \n";

            }
        */


        /////

        /*
        for (int cs = 0; cs < curString.Length; cs++)
            for (int i = 0; i < smallTasks.Length; i++)
            {
                if (smallTasks[i].y == cs)
                {
                    FinalText += "<s>" + curString[i] + "</s> \n";
                    print("found" + smallTasks[i].y + " and " + cs + " -  " + "<s>" + curString[i] + "</s> \n");
                }
                else
                    FinalText += curString[i] + "\n";

            }
        */


        /*

        for (int cs = 0; cs < curString.Length; cs++)
        {
            if (curString[cs] == "\n")
            {
                bool notfound = true;
                int finder = cs;
                while(!notfound)
                    {
                    if (curString[finder].Contains("<b>"))
                    {
                        FinalText += "<s>" + curString[finder] + "</s> \n";
                        notfound = true;
                    }
                        finder -= 1;
                    }
            }


        }

        */


        /*
        
                for (int i = 0; i < curString.Length; i++)
        {
            if (i == line! & curString[i].Contains("\t"))
                FinalText += "<u>" + curString[i] + "</u> ---> \n";
            else if (i < line)
            {
                if (curString[i].Contains("\t"))
                    FinalText += "<s>" + curString[i] + "</s> \n";
                else if (curString[i].Contains("<b>") && i != 0 && curString[i-2].Contains("\t"))
                    FinalText += "<s>" + curString[i] + "</s> \n";
                else FinalText += curString[i] + "\n";
            }
            else
                FinalText += curString[i] + "\n";
        }
        */


        
        for (int i = 0; i < curString.Length; i++)
        {
            //print(curString[i].Length);
            //print(curString[i]);
            if (i == line! & curString[i].Contains("\t"))
                FinalText += "<u>" + curString[i] + "</u> ---> \n";
            else if (i < line)
            {
                if (curString[i].Contains("\t"))
                    FinalText += "<s>" + curString[i] + "</s> \n";
                else FinalText += curString[i] + "\n";
            }
            else
                FinalText += curString[i] + "\n";
        }

        /*
                    else if (curString[i].Length == 1)
            {
                print("NBOY");
                bool searching = true;
                int finder = i;
                while (searching)
                {
                    if (curString[finder].Contains("<b>"))
                    {
                        FinalText += "<s>" + curString[finder] + "</s> \n";
                        searching = false;
                    }
                    finder -= 1;
                }
            }
        */



        TaskText.text = FinalText;

    }


}
