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
        IEnumerable<Answer> answers;
        Rule rule;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Terminal_TextChanged(object sender, TextChangedEventArgs e)
        {
            string termtext = terminal.Text;
            try
            {
                answers = SepareteTerm(termtext);
                RefreshConsole();
            }
            catch(Exception ex)
            {
                message.Text = ex.Message;
            }

        }

        private void Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                string[] codelines = code.Text.Split('\n');
                rule = new Rule(codelines);
                RefreshConsole();
            }
            catch(Exception ex)
            {
                message.Text = ex.Message;
            }
        }

        private IEnumerable<Answer> SepareteTerm(string text)
        {
            IList<Answer> answers = new List<Answer>();

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

        private void RefreshConsole()
        {
            console.Text = message.Text = "";
            console.Text = rule.ToString();
            //int i = 0;
            //foreach (string s in codelines)
            //{
            //    console.Text += $"{++i}: {s}\n";
            //}
            foreach (Answer a in answers)
            {
                console.Text += $"Player{a.Player} -> {a.AnswerType}\n";
            }
        }

        private void Simulate(string[] rule, IEnumerable<Answer> answers)
        {
            int win = 7;
            int lose = 3;
            int pl_size = 5;
            int val_size = 2;
            List<List<int>> vals = new List<List<int>>();
            for(int i = 0; i < pl_size; i++)
            {
                var lis = new List<int>();
                for (int j = 0; j < val_size; j++) lis.Add(0);
                vals.Add(lis);
            }

            foreach(Answer a in answers)
            {
                if (a.Player > pl_size) throw new Exception("Player index too large");
                switch (a.AnswerType)
                {
                    case AType.Correct:
                        break;
                }
            }
        }
    }

    class Rule
    {
        enum Part
        {
            None, Ident, Win, Lose, Cor, Wro
        }

        private IReadOnlyList<string> idents; // 各Playerが持つ変数
        public IReadOnlyList<Node> win { get; } // 勝ち抜け条件
        private IReadOnlyList<Node> lose { get; } // 敗退条件
        private IReadOnlyList<Node> cor { get; } // 正解時の変数挙動
        private IReadOnlyList<Node> wro { get; } // 誤答時の変数挙動

        public int GetIdentIndex(string ident)
        {
            IReadOnlyList<int> id = (IReadOnlyList<int>)(
                idents.Select((p, i) => new { Content = p, Index = i })
                .Where(x => x.Content == ident)
                .Select(x => x.Index)
                );
            if (id.Count == 0) throw new Exception("Identifier is not found");
            return id[0];
        }

        public override string ToString()
        {
            return $"[Rule] Ident:{string.Join(",", idents)}, Win:{string.Join(",", win)}, Lose:{string.Join(",", lose)}, Correct:{string.Join(",", cor)}, Wrong:{string.Join(",", wro)}";
        }


        public Rule(string[] lines)
        {
            List<string> _idents = new List<string>(),
                    _win = new List<string>(),
                    _lose = new List<string>(),
                    _cor = new List<string>(),
                    _wro = new List<string>();

            Part part = Part.None;
            foreach (string l in lines)
            {
                string line = l.Replace("\n", "").Replace("\r", "");
                Console.WriteLine(line);
                switch (line.Replace(" ", ""))
                {
                    case "<Ident>":
                        part = Part.Ident;
                        break;
                    case "<Win>":
                        part = Part.Win;
                        break;
                    case "<Lose>":
                        part = Part.Lose;
                        break;
                    case "<Correct>":
                        part = Part.Cor;
                        break;
                    case "<Wrong>":
                        part = Part.Wro;
                        break;
                    case "":
                        // 空白はスルー
                        break;
                    default:
                        switch (part)
                        {
                            case Part.Ident:
                                _idents.Add(line.Replace(" ",""));
                                break;
                            case Part.Win:
                                _win.Add(line);
                                break;
                            case Part.Lose:
                                _lose.Add(line);
                                break;
                            case Part.Cor:
                                _cor.Add(line);
                                break;
                            case Part.Wro:
                                _wro.Add(line);
                                break;
                        }
                        break;
                }
            }
            idents = _idents;
            win = makeNodes(_win);
            lose = makeNodes(_lose);
            cor = makeNodes(_cor);
            wro = makeNodes(_wro);
        }

        private IReadOnlyList<Node> makeNodes(List<string> text)
        {
            List<Node> nodes = new List<Node>();
            foreach(string s in text)
            {
                nodes.Add(new Node(s, this));
            }
            return nodes;
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
