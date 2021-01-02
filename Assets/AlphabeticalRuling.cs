using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;
using Rnd = UnityEngine.Random; 

public class AlphabeticalRuling : MonoBehaviour {

    public KMBombInfo bomb;
    public KMBombModule module;
    public KMAudio audio;
    public KMSelectable[] button;
    public TextMesh[] NumberDisplays;
    public TextMesh LetterDisplay;

    private string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static int counter = 1;
    private int moduleId;
    private int currentStage = 1;
    private int stages;
    private bool solved = false;
    private int correctButton;
    private static string mess = "You did it!";
    //private int digitAdded; I don't apparently need this here.
    private int giveAns;
    private int[] a = new int[3]{0, 0, 0};
    IEnumerator giveMess()
    {
        yield return null;
        for (int i = 0; i < mess.Length; i++)
        {
            NumberDisplays[0].text += mess[i];
            yield return new WaitForSeconds(0.1f);
        }
    }
    void Awake()
    {
        moduleId = counter++;
        stages = 3;
        
        for (int i = 0; i < button.Length; i++)
        {
            int news = i;
            button[i].OnInteract += delegate
            {
                Pressing(news); return false;
            };
        }
    }
    void displayNumber()
    {
        int i = UnityEngine.Random.Range(0, 9);
        int[] nums = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        NumberDisplays[0].text = nums[i].ToString();
        NumberDisplays[1].text = "" + currentStage;
        int j = UnityEngine.Random.Range(0, 26);
        LetterDisplay.text = "" + letters.ElementAt(j);
        if (LetterDisplay.text == "Q")
        {
            LetterDisplay.fontSize = 60;
        }
        else
        {
            LetterDisplay.fontSize = 65;
        }
        Debug.LogFormat("[Alphabetical Ruling #{0}] Number {1} and Letter {2} are given.", moduleId, NumberDisplays[0].text, LetterDisplay.text);
        giveAns = (correctButtons(LetterDisplay) % 10);
        if (giveAns == 0)
        {
            giveAns++;
        }

    }
	// Use this for initialization
	void Start () {
        module.OnActivate += Some;
        
    }
    void Some()
    {
        displayNumber();
        Debug.LogFormat("[Alphabetical Ruling #{0}] Stage {1} started!", moduleId, currentStage);
    }
    void Pressing(int i)
    {
        int correctted = giveAns;
        correctButton = i + 1;
        button[i].AddInteractionPunch();
        audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if (!solved)
        {
            Debug.LogFormat("[Alphabetical Ruling #{0}] You have pressed button {1}.", moduleId, correctButton);
            
            if (correctted == correctButton)
            {
                correct();
                if (currentStage == 3)
                {
                    NumberDisplays[0].text = "";
                    NumberDisplays[0].transform.localPosition = new Vector3(-4f, 0.01f, 4.42f);
                    NumberDisplays[0].fontSize = 40;
                    NumberDisplays[1].text = "";
                    LetterDisplay.text = "";
                    int n = 1;
                    foreach (int s in a)
                    {
                        Debug.LogFormat("[Alphabetical Ruling #{0}] Number {1} to input: {2}", moduleId, n, s);
                        n++;
                    }
                    StartCoroutine(giveMess());
                    solved = true;
                    module.HandlePass();
                }
                else
                {
                    currentStage++;
                    Some();
                }
            }
            else
            {
                incorrect();
                module.HandleStrike();
            }
        }
    }
    void correct()
    {
        Debug.LogFormat("[Alphabetical Ruling #{0}] The input is correct!", moduleId);
    }
    void incorrect()
    {
        Debug.LogFormat("[Alphabetical Ruling #{0}] The input is incorrect!", moduleId);
    }
    int oldOnes(int aq)
    {
        if ((aq % 8) == 1)
        {
            return 2;
        }
        else if ((aq % 8) == 2)
        {
            return 30;
        }
        else if ((aq % 8) == 3)
        {
            return 33;
        }
        else if ((aq % 8) == 4)
        {
            return 38;
        }
        else if ((aq % 8) == 5)
        {
            return 28;
        }
        else if ((aq % 8) == 6)
        {
            return 29;
        }
        else if ((aq % 8) == 7)
        {
            return 18;
        }
        else
        {
            return 8;
        }
    }
    
    int factorialGive(int a)
    {
        int b = 1;
        for (int i = a; i > 0; i--)
        {
            b *= i;
        }
        return b;
    }
    int correctButtons(TextMesh s)
    {
        int digitAdded = 0;
        switch (s.text)
        {
            case "A":
                digitAdded = int.Parse(NumberDisplays[0].text) - bomb.GetSerialNumberNumbers().Last();
                break;
            case "B":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetPortCount(Port.Parallel);
                break;
            case "C":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetBatteryHolderCount();
                break;
            case "D":
                digitAdded = int.Parse(NumberDisplays[0].text) - bomb.GetBatteryCount(Battery.AA);
                break;
            case "E":
                digitAdded = int.Parse(NumberDisplays[0].text) + 280;
                break;
            case "F":
                digitAdded = int.Parse(NumberDisplays[0].text) % 6;
                break;
            case "G":
                digitAdded = int.Parse(NumberDisplays[0].text) - 69;
                break;
            case "H":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetPortCount(Port.PS2);
                break;
            case "I":
                digitAdded = int.Parse(NumberDisplays[0].text) - bomb.GetModuleIDs().Where(x => x == "omegaForget").Count();
                break;
            case "J":
                int ni = 0;
                ni = bomb.GetModuleIDs().Where(x => x == "necronomicon").Count();
                if (ni >= 1 && (!(bomb.GetSerialNumberNumbers().Last() == 0) && !(bomb.GetSerialNumberNumbers().Last() == 9)))
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) + oldOnes(bomb.GetSerialNumberNumbers().Last());
                }
                else
                {
                    int pi = bomb.GetPortCount(Port.PS2);
                    digitAdded = int.Parse(NumberDisplays[0].text) + pi;
                }
                break;
            case "K":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetIndicators().Count();
                break;
            case "L":
                if (bomb.GetBatteryCount() == 4 && bomb.GetBatteryHolderCount() == 3 && bomb.GetPortCount() == 2)
                {
                    digitAdded = int.Parse(NumberDisplays[0].text);
                }
                else
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) + 3;
                }
                break;
            case "M":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetModuleNames().Count();
                break;
            case "N":
                if (bomb.GetBatteryCount() == 2 && bomb.GetBatteryHolderCount() == 1 && bomb.GetPortCount(Port.PS2) > 0)
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) - factorialGive(bomb.GetSerialNumberNumbers().Last());
                }
                else
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetModuleIDs().Where(x => x == "kugelblitz").Count();
                }
                break;
            case "O":
                digitAdded = int.Parse(NumberDisplays[0].text) + 81;
                break;
            case "P":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetPortCount(Port.Serial);
                break;
            case "Q":
                if (bomb.GetPortCount() > 2)
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetPortPlateCount() + 2;
                }
                else
                {
                    digitAdded = int.Parse(NumberDisplays[0].text);
                }
                break;
            case "R":
                digitAdded = int.Parse(NumberDisplays[0].text) + (bomb.GetBatteryCount(Battery.D) + bomb.GetPortCount(Port.PS2)) - bomb.GetSerialNumberNumbers().Sum();
                digitAdded = Math.Abs(digitAdded);
                break;
            case "S":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetSerialNumberNumbers().Sum();
                break;
            case "T":
                if (bomb.GetSerialNumberNumbers().Sum() > 10)
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) * 5;
                }
                else
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) * 10;
                }
                digitAdded %= 100;
                break;
            case "U":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetSerialNumberNumbers().Count();
                break;
            case "V":
                int a = 0;
                foreach (string module in bomb.GetModuleNames())
                {
                    string temp = module.ToLower();
                    if (temp.Contains("forget") && !temp.Contains("forgetting") && !temp.Contains("forgets")){
                        a++;
                    }
                }
                if (a > 0)
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) + (bomb.GetBatteryCount() * 2);
                    break;
                }
                else
                {
                    digitAdded = int.Parse(NumberDisplays[0].text) * 49;
                    break;
                }
                
            case "W":
                digitAdded = int.Parse(NumberDisplays[0].text) * 25;
                break;
            case "X":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetBatteryCount() + bomb.GetBatteryHolderCount();
                break;
            case "Y":
                digitAdded = int.Parse(NumberDisplays[0].text) + bomb.GetPortPlateCount()-1;
                break;
            case "Z":
                digitAdded = int.Parse(NumberDisplays[0].text) + 11;
                break;
        }
        a[currentStage-1] += digitAdded % 10;
        digitAdded %= 10;
        if (digitAdded < 0) { digitAdded += 10; };
        if (digitAdded == 0)
        {
            Debug.LogFormat("[Alphabetical Ruling #{0}] Number to press: {1}", moduleId, digitAdded+1);
        }
        else
        {
            Debug.LogFormat("[Alphabetical Ruling #{0}] Number to press: {1}", moduleId, digitAdded);
        }
        Debug.LogFormat("[Alphabetical Ruling #{0}] Rule used: {1}", moduleId, s.text);
        return digitAdded;
    }
    // Twitch plays
    private bool isValid(string par)
    {
        string[] something = {"1", "2", "3", "4", "5", "6", "7", "8", "9"};
        if (!something.Contains(par))
        {
            return false;
        }
        return true;
    }
#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <#> [Presses the button labeled '#'] | Valid numbers are 1-9";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {   
            yield return null;
            if (parameters.Length < 2)
            {
                yield return "sendtochaterror Please specify what number you would like to press!";
            }
            else if (parameters.Length > 2)
            {
                yield return "sendtochaterror Too many arguements!";
            }
            else
            {
                if (!isValid(parameters.ElementAt(1)))
                {
                    yield return "sendtochaterror " + parameters[1] + ", which was a number inputted to the module, is invalid.";
                    yield break;
                }
                int temp = 0;
                int.TryParse(parameters[1], out temp);
                button[temp - 1].OnInteract();
            }
        }
    }
    IEnumerator TwitchHandleForcedSolve()
    {
        for (int i = currentStage; i < 4; i++)
        {
            button[giveAns-1].OnInteract();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
