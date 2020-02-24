using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QSimulator
{
    enum AType
    {
        Correct, Wrong, Through
    }

    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            string termtext = terminal.Text;
            try
            {
                List<Answer> answers = SepareteTerm(termtext);
                console.Text = message.Text = "";
                foreach(Answer a in answers)
                {
                    console.Text = console.Text + $"Player{a.Player} -> {a.AnswerType}\n";
                }
            }
            catch(Exception ex)
            {
                message.Text = ex.Message;
            }

        }

        private List<Answer> SepareteTerm(string text)
        {
            List<Answer> answers = new List<Answer>();

            int pln = 0;
            
            foreach(char c in text)
            {
                if(char.IsDigit(c))
                {
                    pln = pln * 10 + int.Parse(c.ToString());
                }
                else if(c == 'o' || c == 'x')
                {
                    // Correct or Wrong -- pln must not be 0
                    if(pln == 0)
                    {
                        throw new Exception("Not found PlayerIndex");
                    }
                    else
                    {
                        answers.Add(new Answer(pln, (c == 'o') ? AType.Correct : AType.Wrong));
                        pln = 0;
                    }
                }
                else if(c == 't')
                {
                    // Through -- pln must be 0
                    if (pln == 0)
                    {
                        answers.Add(new Answer(0, AType.Through));
                    }
                    else
                    {
                        throw new Exception("Need not PlayerIndex");
                    }
                }
                else
                {
                    throw new Exception("Invalid letter");
                }
            }

            if(pln != 0)
            {
                throw new Exception("Not found C/W key");
            }

            return answers;
        }



    }

    class Answer
    {
        private int player_index;
        private AType answer_type;
        public Answer(int pl, AType aType)
        {
            player_index = pl;
            answer_type = aType;
        }

        public int Player
        {
            get
            {
                return player_index;
            }
        }

        public AType AnswerType
        {
            get
            {
                return answer_type;
            }
        }
    }
}
