using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace TagFormating
{
    class Program // NEPLATNÉ TAGY PŘEPISUJE A FORMÁTUJE JAKOBY BYLY SOUČÁSTÍ TEXTU
    {
        public static ConsoleColor defaultBgColor = Console.BackgroundColor;
        public static ConsoleColor defaultFgColor = Console.ForegroundColor;
        static void Main(string[] args)
        {
            //Tagy a jejich vytváření pomocí paramatrů (jméno,formát textu, barva textu, barva pozadí)
            Tag t1 = new Tag("red", Tag.TextFormat.Normal, ConsoleColor.Red);
            Tag t2 = new Tag("green", Tag.TextFormat.Normal, ConsoleColor.Green);
            Tag t3 = new Tag("capital", Tag.TextFormat.ToUpper);
            Tag t4 = new Tag("highlight", Tag.TextFormat.Highlight);
            Tag t5 = new Tag("hide", Tag.TextFormat.Hide);
            Tag t6 = new Tag("yellowhighlight", Tag.TextFormat.Normal, default, ConsoleColor.Yellow);

            //čtení souboru z adresy (první argument při volání z příkazového řádku)
            using (StreamReader sr = new StreamReader(args[0]))
            {
                while (sr.Peek() != -1)
                {
                    if (sr.Peek() == 60) // = '<'
                    {
                        sr.Read(); //program narazil na '<' => jde zkontrolovat zda se jedná o tag
                        TagIndetification(sr);
                    }
                    else
                    {
                        Console.Write((char)sr.Read());
                    }
                }
            }
            Console.ReadLine();
        }

        static bool moreFormating = false; //bool zda se provádí více formátování najednou
        static List<string> previous_tag = new List<string>();  //list předchozích tagů, na které program narazil při více jak jednom formátování 
        static void TagIndetification(StreamReader sr)
        {
            StringBuilder tag1 = new StringBuilder("");
            if (sr.Peek() == 47)// = '/' //při několikanásobném formátování je možné, že se jedná o uzavírající tag => po závorce následuje '/'
            {
                if (previous_tag.Count >= 1) //pokud v listu předchozích tagu nalezneme nějaleký tag pošme ho do hodnoty tag1 a později porovnáme s tag2
                    tag1.Append(previous_tag[previous_tag.Count - 1]);
                else
                {//pokud v listu není žádný předchozí neukončený tag jedná se o chybu a program nebude formátovat jen slepě přepíše
                    Console.Write("<");
                    return;
                }         
            }
            else // jedná se o nový tag
            {
                while (sr.Peek() != 62) // = '>'
                {
                    tag1.Append((char)sr.Read()); //zjistíme jméno tagu
                }
                sr.Read();

                StringBuilder needsFormating = new StringBuilder("");
                while (sr.Peek() != 60) // = '<'
                {
                    needsFormating.Append((char)sr.Read()); //hodnota textu, který je potřeba formátovat
                }
                Tag.formatedText += needsFormating.ToString(); // uložení do statické proměnné, aby text mohlo upravovat více tagů najednou
                sr.Read();

            }
                
            if (sr.Peek() == 47) // = '/' //hledáme ukončující tag
            {
                sr.Read();
                StringBuilder tag2 = new StringBuilder("");
                while (sr.Peek() != 62) // = '>'
                {
                    tag2.Append((char)sr.Read()); //hodnota ukončujícího tagu
                }
                sr.Read();

                if (tag1.ToString() == tag2.ToString() || moreFormating) // porovnání otevíracího a ukončovacího tagu ||  porovnání minulého otevírajícího tagu a ukončovacího tagu
                {
                    if (moreFormating) // v případě, že předchozí tag je platný budeme neplatný formátovat společně s textem
                    {
                        if (previous_tag[previous_tag.Count - 1] == tag2.ToString() && previous_tag[previous_tag.Count - 1] != tag1.ToString())
                        {
                            Tag.formatedText = $"<{tag1}>{Tag.formatedText}";
                            tag1.Clear();
                            tag1.Append(previous_tag[previous_tag.Count - 1]);
                            if (previous_tag.Count <= 1) //pokud v zbývá jeden tag, změníme hodnotu moreformating na false, při posledním formátování se vypíše text
                                moreFormating = false;
                        }
                    }                        
                    if (Tag.ListOfTagNames.Contains(tag1.ToString()) && tag1.ToString() == tag2.ToString()) // kontrola zda tag existuje v našem seznamu
                    {
                        foreach (Tag x in Tag.ListOfTags) //hledání objektu tag, kterému náleží jméno tagu
                        {
                            if (x.Name == tag1.ToString())
                            {
                                x.handler.Invoke(); //volání delegáta, který obsahuje všechny formátovací metody pro určitý tag
                                if (!moreFormating) //pokud se jedná o několikanásobné formátování program zatím nevypíše text
                                {
                                    x.Write();
                                    Tag.formatedText = "";
                                }


                                previous_tag.Remove(tag2.ToString()); //pokud se jedná o tag, který se nacházel v seznamu předchozích tagu tak ho odstraníme
                                if (previous_tag.Count <= 1) //pokud v zbývá jeden tag, změníme hodnotu moreformating na false, při posledním formátování se vypíše text
                                    moreFormating = false;

                                break;
                            }
                        }

                    }
                    else
                    {
                        Tag.formatedText = $"<{tag1}>{Tag.formatedText}</{tag2}>"; // několikanásobně formátujeme, budeme formátovat neplatné tagy spolěčně s textem

                        if (previous_tag.Count <= 1) //pokud v zbývá jeden tag, změníme hodnotu moreformating na false, při posledním formátování se vypíše text
                            moreFormating = false;
                    }


                }
                else
                {
                    if(Tag.ListOfTagNames.Contains(tag1.ToString()))
                    {
                        previous_tag.Add(tag1.ToString()); //pokud program narazil na na špatný uzavírající Tag, uložího hodnotu toho minulého(pokud existuje) do listu
                        Tag.formatedText = $"{Tag.formatedText}</{tag2}>";
                    }
                    else if (moreFormating)
                    {
                        Tag.formatedText = $"<{tag1}>{Tag.formatedText}</{tag2}>"; //pokud několikanásobně formátujeme, budeme formátovat neplatné tagy spolěčně s textem
                        if (previous_tag.Count <= 1) //pokud v zbývá jeden tag, změníme hodnotu moreformating na false, při posledním formátování se vypíše text
                            moreFormating = false;
                    }
                    else
                        Console.Write($"<{tag1}>{Tag.formatedText}</{tag2}>"); //chybný tag spolěčně s textem slepě přepíšeme
                }
                    

            }
            else
            {
                if (Tag.ListOfTagNames.Contains(tag1.ToString()))
                {
                    previous_tag.Add(tag1.ToString()); //pokud program narazil na nový otevírající Tag, uložího hodnotu toho minulého(pokud existuje) do listu
                    moreFormating = true; //více formátování
                }
                else if (moreFormating)
                {
                    Tag.formatedText = $"<{tag1}>{Tag.formatedText}"; //pokud několikanásobně formátujeme, budeme formátovat neplatný tag spolěčně s textem
                }
                else
                {
                    Console.Write($"<{tag1}>{Tag.formatedText}"); //pokud neexistuje slepě vypíše
                }
                TagIndetification(sr); //znovu volání identifikace tagu protože jsme narazili na nový tag
            }
        }
    }
}
