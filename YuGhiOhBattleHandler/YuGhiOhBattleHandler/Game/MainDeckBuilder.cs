using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace YuGhiOhBattleHandler
{
    public sealed class MainDeckBuilder
    {
        private List<Object> currentlySelectedDeck=new List<object>();
        private List<Object> allPossibleCards=new List<object>();

        public MainDeckBuilder()
        {
            LoadAllPossibleCards();
        }

        public BitmapImage getCardBack()
        {
            return getCardBackAsync().Result;
        }

        private async Task<BitmapImage> getCardBackAsync()
        {
            var names = this.GetType().GetTypeInfo().Assembly.GetManifestResourceNames();
            string imagePath = "";
            foreach (string n in names)
            {
                if (n.Contains("CardBack"))
                {
                    imagePath = n;
                    break;
                }
            }
            BitmapImage toReturn = new BitmapImage();
            var assembly = this.GetType().GetTypeInfo().Assembly;

            using (var imageStream = assembly.GetManifestResourceStream(imagePath))
            using (var memStream = new MemoryStream())
            {
                await imageStream.CopyToAsync(memStream);

                memStream.Position = 0;

                using (var raStream = memStream.AsRandomAccessStream())
                {
                    toReturn.SetSource(raStream);
                }
            }
            return toReturn;
        }

        public IList<Object> getRandomDeck()
        {
            currentlySelectedDeck = allPossibleCards;
            return currentlySelectedDeck;
        }

        public bool LoadAllPossibleCards()
        {
            LoadAllPossibleCardsAsync().Wait();
            //Give it a 2 seconds to load
            //System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(2)).Wait();
            return true;
        }

        private async Task<bool> LoadAllPossibleCardsAsync()
        {
            try
            {
                var names=this.GetType().GetTypeInfo().Assembly.GetManifestResourceNames();
                string databasePath = "";
                string imagePath = "";
                foreach (string n in names)
                {
                    if(n.Contains("Database"))
                    {
                        databasePath = n;
                        break;
                    }
                }
                foreach(string n in names)
                {
                    if(n.Contains("Images"))
                    {
                        //Truncate the .jpg
                        imagePath = n.Substring(0, n.LastIndexOf("."));
                        //Truncate the actual file name we only want the path.
                        imagePath = imagePath.Substring(0, imagePath.LastIndexOf(".")+1);
                        break;
                    }
                }
                string allLines = "";
                using (StreamReader sr = new StreamReader(this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream(databasePath)))
                {
                    allLines = sr.ReadToEnd();
                }
                string[] splitIntoIndividualLines = allLines.Split('\n');
                for (int i = 1; i < splitIntoIndividualLines.Length; i++)
                {
                    string[] split = splitIntoIndividualLines[i].Split(',');
                    Object c = new Object();
                    BitmapImage bi = new BitmapImage();
                    var assembly = this.GetType().GetTypeInfo().Assembly;

                    using (var imageStream = assembly.GetManifestResourceStream(imagePath+split[14]))
                    using (var memStream = new MemoryStream())
                    {
                        await imageStream.CopyToAsync(memStream);

                        memStream.Position = 0;

                        using (var raStream = memStream.AsRandomAccessStream())
                        {
                            bi.SetSource(raStream);
                        }
                    }
                    if (split[2] == "Spell")
                    {
                        if (split[3] == "Continuous")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Continuous, split[6], long.Parse(split[7]),bi);
                        }
                        else if (split[3] == "Counter")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Counter, split[6], long.Parse(split[7]),bi);
                        }
                        else if (split[3] == "Equip")
                        {
                            string s = split[0];
                            string s1 = split[6];
                            string s2 = split[7];
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Equip, split[6], long.Parse(split[7]),bi);

                        }
                        else if (split[3] == "Field")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Field, split[6], long.Parse(split[7]),bi);

                        }
                        else if (split[3] == "QuickPlay")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.QuickPlay, split[6], long.Parse(split[7]),bi);
                        }
                        else if (split[3]=="Ritual")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Ritual, split[6], long.Parse(split[7]),bi);
                        }
                        else
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Spell, Icon.Normal, split[6], long.Parse(split[7]),bi);
                        }
                    }
                    else if (split[2] == "Trap")
                    {
                        if (split[3] == "Continuous")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Continuous, split[6], long.Parse(split[7]),bi);
                        }
                        else if (split[3] == "Counter")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Counter, split[6], long.Parse(split[7]),bi);
                        }
                        else if (split[3] == "Equip")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Equip, split[6], long.Parse(split[7]),bi);

                        }
                        else if (split[3] == "Field")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Field, split[6], long.Parse(split[7]),bi);

                        }
                        else if (split[3] == "QuickPlay")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.QuickPlay, split[6], long.Parse(split[7]),bi);
                        }
                        else if(split[3]=="Ritual")
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Ritual, split[6], long.Parse(split[7]),bi);
                        }
                        else
                        {
                            c = new SpellAndTrapCard(split[0], CardAttributeOrType.Trap, Icon.Normal, split[6], long.Parse(split[7]),bi);
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
                            Debug.WriteLine("Cannot have XYZ Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.WriteLine("Cannot have Synchro Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.WriteLine("Cannot have Fusion Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Dark, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual,bi);
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
                            Debug.WriteLine("Cannot have XYZ Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.WriteLine("Cannot have Synchro Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.WriteLine("Cannot have Fusion Monsters in Main Deck.");
                            //return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Earth, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual,bi);
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
                            Debug.WriteLine("Cannot have XYZ Monsters in Main Deck.");
                            // return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.WriteLine("Cannot have Synchro Monsters in Main Deck.");
                            // return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.WriteLine("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Fight, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual,bi);
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
                            Debug.WriteLine("Cannot have XYZ Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.WriteLine("Cannot have Synchro Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.WriteLine("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Fire, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual,bi);
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
                            Debug.WriteLine("Cannot have XYZ Monsters in Main Deck.");
                            return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.WriteLine("Cannot have Synchro Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.WriteLine("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Water, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual,bi);
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
                            Debug.WriteLine("Cannot have XYZ Monsters in Main Deck.");
                            return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.WriteLine("Cannot have Synchro Monsters in Main Deck.");
                            //   return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.WriteLine("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Wind, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual,bi);
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
                            Debug.WriteLine("Cannot have XYZ Monsters in Main Deck.");
                            return false;
                        }
                        if (split[10].ToUpper() == "Y")
                        {
                            isSynchro = true;
                            Debug.WriteLine("Cannot have Synchro Monsters in Main Deck.");
                            //   return false;
                        }
                        if (split[11].ToUpper() == "Y")
                        {
                            isSynchroTuner = true;
                        }
                        if (split[12].ToUpper() == "Y")
                        {
                            isFusion = true;
                            Debug.WriteLine("Cannot have Fusion Monsters in Main Deck.");
                            //  return false;
                        }
                        if (split[13].ToUpper() == "Y")
                        {
                            isRitual = true;
                        }

                        c = new MonsterCard(split[0], int.Parse(split[1]), CardAttributeOrType.Light, split[3], int.Parse(split[4]), int.Parse(split[5]), split[6], long.Parse(split[7]), isPendulum, isXyz, isSynchro, isSynchroTuner, isFusion, isRitual,bi);
                    }
                    allPossibleCards.Add(c);
                }
                return true;
            }
            catch(Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}
