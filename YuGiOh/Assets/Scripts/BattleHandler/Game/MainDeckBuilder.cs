using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using System.IO;
using Assets.Scripts.BattleHandler.Cards;

namespace Assets.Scripts.BattleHandler.Game
{
    public class MainDeckBuilder
    {
        private List<Cards.Card> currentlySelectedDeck = new List<Cards.Card>();
        private List<Cards.Card> allPossibleCards = new List<Cards.Card>();

        public MainDeckBuilder()
        {
            LoadAllPossibleCards();
        }

        public UnityEngine.Object getCardBack()
        {
            return Resources.Load("CardBack") as Texture;
        }


        public List<Assets.Scripts.BattleHandler.Cards.Card> getRandomDeck()
        {
            currentlySelectedDeck = allPossibleCards;
            return currentlySelectedDeck;
        }


        private void LoadAllPossibleCards()
        {
            try
            {
                TextAsset database = Resources.Load("CardMaster") as TextAsset;
                string allLines = database.text;
                Debug.Log("Loaded CardMaster Text Asset");
                string[] splitIntoIndividualLines = allLines.Split('\n');
                Debug.Log("Number of Lines: " + splitIntoIndividualLines.Length);
                for (int i = 1; i < splitIntoIndividualLines.Length; i++)
                {
                    string[] split = splitIntoIndividualLines[i].Split(',');
                    Debug.Log("Line starting with " + split[0] + " has length " + split.Length);
                    Assets.Scripts.BattleHandler.Cards.Card c = new Assets.Scripts.BattleHandler.Cards.Card();
                    string imageName = split[14].Substring(0, split[14].IndexOf("."));
                    Debug.Log("Found Image Name: " + imageName);
                    Texture bi = Resources.Load(imageName) as Texture;
                    if (split[2] == "Spell")
                    {
                        if (split[3] == "Continuous")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Continuous, split[6], long.Parse(split[7]), bi);
                        }
                        else if (split[3] == "Counter")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Counter, split[6], long.Parse(split[7]), bi);
                        }
                        else if (split[3] == "Equip")
                        {
                            //string s = split[0];
                            //string s1 = split[6];
                            //string s2 = split[7];
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Equip, split[6], long.Parse(split[7]), bi);

                        }
                        else if (split[3] == "Field")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Field, split[6], long.Parse(split[7]), bi);

                        }
                        else if (split[3] == "QuickPlay")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.QuickPlay, split[6], long.Parse(split[7]), bi);
                        }
                        else if (split[3] == "Ritual")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Ritual, split[6], long.Parse(split[7]), bi);
                        }
                        else
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Normal, split[6], long.Parse(split[7]), bi);
                        }
                    }
                    else if (split[2] == "Trap")
                    {
                        if (split[3] == "Continuous")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Continuous, split[6], long.Parse(split[7]), bi);
                        }
                        else if (split[3] == "Counter")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Counter, split[6], long.Parse(split[7]), bi);
                        }
                        else if (split[3] == "Equip")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Equip, split[6], long.Parse(split[7]), bi);

                        }
                        else if (split[3] == "Field")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Field, split[6], long.Parse(split[7]), bi);

                        }
                        else if (split[3] == "QuickPlay")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.QuickPlay, split[6], long.Parse(split[7]), bi);
                        }
                        else if (split[3] == "Ritual")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Ritual, split[6], long.Parse(split[7]), bi);
                        }
                        else
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Normal, split[6], long.Parse(split[7]), bi);
                        }
                    }
                    else if (split[2] == "Dark")
                    {
                        bool isPendulum = false;
                        bool isXyz = false;
                        bool isSynchro = false;
                        bool isSynchroTuner = false;
                        bool isFusion = false;
                        bool isRitual = false;

                        if (split[8].ToUpper() == "Y")
                        {
                            isPendulum = true;
                        }
                        if (split[9].ToUpper() == "Y")
                        {
                            isXyz = true;
                            Debug.Log("Cannot have XYZ Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.Log("Cannot have Synchro Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.Log("Cannot have Fusion Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Dark, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual, bi);
                    }
                    else if (split[2] == "Earth")
                    {
                        bool isPendulum = false;
                        bool isXyz = false;
                        bool isSynchro = false;
                        bool isSynchroTuner = false;
                        bool isFusion = false;
                        bool isRitual = false;

                        if (split[8].ToUpper() == "Y")
                        {
                            isPendulum = true;
                        }
                        if (split[9].ToUpper() == "Y")
                        {
                            isXyz = true;
                            Debug.Log("Cannot have XYZ Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.Log("Cannot have Synchro Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.Log("Cannot have Fusion Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Earth, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual, bi);
                    }
                    else if (split[2] == "Fight")
                    {
                        bool isPendulum = false;
                        bool isXyz = false;
                        bool isSynchro = false;
                        bool isSynchroTuner = false;
                        bool isFusion = false;
                        bool isRitual = false;

                        if (split[8].ToUpper() == "Y")
                        {
                            isPendulum = true;
                        }
                        if (split[9].ToUpper() == "Y")
                        {
                            isXyz = true;
                            Debug.Log("Cannot have XYZ Monsters in Main Deck.");
                            // return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.Log("Cannot have Synchro Monsters in Main Deck.");
                            // return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.Log("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Fight, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual, bi);
                    }
                    else if (split[2] == "Fire")
                    {
                        bool isPendulum = false;
                        bool isXyz = false;
                        bool isSynchro = false;
                        bool isSynchroTuner = false;
                        bool isFusion = false;
                        bool isRitual = false;

                        if (split[8].ToUpper() == "Y")
                        {
                            isPendulum = true;
                        }
                        if (split[9].ToUpper() == "Y")
                        {
                            isXyz = true;
                            Debug.Log("Cannot have XYZ Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.Log("Cannot have Synchro Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.Log("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Fire, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual, bi);
                    }
                    else if (split[2] == "Water")
                    {
                        bool isPendulum = false;
                        bool isXyz = false;
                        bool isSynchro = false;
                        bool isSynchroTuner = false;
                        bool isFusion = false;
                        bool isRitual = false;

                        if (split[8].ToUpper() == "Y")
                        {
                            isPendulum = true;
                        }
                        if (split[9].ToUpper() == "Y")
                        {
                            isXyz = true;
                            Debug.Log("Cannot have XYZ Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.Log("Cannot have Synchro Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.Log("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Water, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual, bi);
                    }
                    else if (split[2] == "Wind")
                    {
                        bool isPendulum = false;
                        bool isXyz = false;
                        bool isSynchro = false;
                        bool isSynchroTuner = false;
                        bool isFusion = false;
                        bool isRitual = false;

                        if (split[8].ToUpper() == "Y")
                        {
                            isPendulum = true;
                        }
                        if (split[9].ToUpper() == "Y")
                        {
                            isXyz = true;
                            Debug.Log("Cannot have XYZ Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.Log("Cannot have Synchro Monsters in Main Deck.");
                            //   return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.Log("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Wind, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual, bi);
                    }
                    else if (split[2] == "Light")
                    {
                        bool isPendulum = false;
                        bool isXyz = false;
                        bool isSynchro = false;
                        bool isSynchroTuner = false;
                        bool isFusion = false;
                        bool isRitual = false;

                        if (split[8].ToUpper() == "Y")
                        {
                            isPendulum = true;
                        }
                        if (split[9].ToUpper() == "Y")
                        {
                            isXyz = true;
                            Debug.Log("Cannot have XYZ Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.Log("Cannot have Synchro Monsters in Main Deck.");
                            //   return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.Log("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Light, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual, bi);
                    }
                    allPossibleCards.Add(c);
                    Debug.Log("Was able to fully create: " + c.CardName);
                }
                //return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                //return false;
            }
        }
    }
}
