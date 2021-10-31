using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TagFormating
{
    class Tag
    {
        public delegate void Del();
        public Del handler;

        public static List<Tag> ListOfTags = new List<Tag>();
        public static List<string> ListOfTagNames = new List<string>();
        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != "" && !ListOfTagNames.Contains(value))
                {
                    _name = value;
                }
            }
        }
        public enum TextFormat { Normal, ToUpper, ToLower, Hide, Highlight };
        public TextFormat Format { get; set; }

        private ConsoleColor _fgColor = Program.defaultFgColor;
        public ConsoleColor FgColor
        {
            get { return _fgColor; }
            set { _fgColor = value; }
        }

        private ConsoleColor _bgColor = Program.defaultBgColor;
        public ConsoleColor BgColor
        {
            get { return _bgColor; }
            set { _bgColor = value; }
        }

        public static string formatedText;

        public Tag(string name, TextFormat format = TextFormat.Normal, ConsoleColor fgColor = default, ConsoleColor bgColor = default)
        {
            Name = name;
            if (fgColor == default)
                fgColor = Program.defaultFgColor;
            if (bgColor == default)
                bgColor = Program.defaultBgColor;
            if (format != TextFormat.Normal)
            {
                Format = format;
                handler += ChangeFormat;
            }
            if (fgColor != FgColor)
            {
                FgColor = fgColor;
                handler += ChangeFgColor;
            }
            if (bgColor != BgColor)
            {
                BgColor = bgColor;
                handler += ChangeBgColor;
            }
            ListOfTagNames.Add(Name);
            ListOfTags.Add(this);
        }

        public void ChangeFgColor()
        {
            Console.ForegroundColor = FgColor;
        }
        public void ChangeBgColor()
        {
            Console.BackgroundColor = BgColor;
        }
        public void ChangeFormat()
        {
            switch (Format)
            {
                case TextFormat.Normal:
                    break;
                case TextFormat.ToUpper:
                    formatedText = formatedText.ToUpper();
                    break;
                case TextFormat.ToLower:
                    formatedText = formatedText.ToLower();
                    break;
                case TextFormat.Hide:
                    string hide = "";
                    foreach (char c in formatedText)
                    {
                        hide += "*";
                    }
                    formatedText = formatedText.Replace(formatedText, hide);
                    break;
                case TextFormat.Highlight:
                    formatedText = $"***{formatedText.ToUpper()}***";
                    break;
            }
        }
        public void Write()
        {
            Console.Write(formatedText);
            //Back to default
            Console.ForegroundColor = Program.defaultFgColor;
            Console.BackgroundColor = Program.defaultBgColor;
        }


    }

}
