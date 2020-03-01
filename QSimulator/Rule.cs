using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSimulator
{
    class Rule
    {
        enum Part
        {
            None, Init, Ident, Win, Lose, Cor, Wro
        }

        public int PlayerCount { get; }
        private IReadOnlyList<string> idents; // 各Playerが持つ変数
        private IReadOnlyList<int> identInitValue;
        public IReadOnlyList<Node> win { get; } // 勝ち抜け条件
        private IReadOnlyList<Node> lose { get; } // 敗退条件
        private IReadOnlyList<Node> cor { get; } // 正解時の変数挙動
        private IReadOnlyList<Node> wro { get; } // 誤答時の変数挙動
        private IList<IList<int>> playerValues;


        public Rule(string[] lines)
        {
            List<string> _idents = new List<string>(),
                    _win = new List<string>(),
                    _lose = new List<string>(),
                    _cor = new List<string>(),
                    _wro = new List<string>();
            List<int> _identInit = new List<int>();
            Part part = Part.None;
            foreach (string l in lines)
            {
                string line = l.Replace("\n", "").Replace("\r", "");
                Console.WriteLine(line);
                switch (line.Replace(" ", ""))
                {
                    case "<Init>":
                        part = Part.Init;
                        break;
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
                            case Part.Init:
                                if (line.StartsWith("count:"))
                                {
                                    PlayerCount = int.Parse(line.Substring(6));
                                }
                                break;
                            case Part.Ident:
                                string[] parts = line.Replace(" ", "").Split('=');
                                int n = 0;
                                if (parts.Length > 2 || parts.Length < 0) throw new Exception("Format of variable initialization is incorrect");
                                if(parts.Length == 2)
                                {
                                    if(!int.TryParse(parts[1], out n))
                                    {
                                        throw new Exception("Variable can't initialize");
                                    }
                                }
                                _idents.Add(parts[0]);
                                _identInit.Add(n);
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
            identInitValue = _identInit;
            win = makeNodes(_win);
            lose = makeNodes(_lose);
            cor = makeNodes(_cor);
            wro = makeNodes(_wro);

            playerValues = new List<IList<int>>();
            for(int i = 0; i < PlayerCount; i++)
            {
                IList<int> p = new List<int>();
                foreach(int v in _identInit)
                {
                    p.Add(v);
                }
                playerValues.Add(p);
            }

        }
        public int GetIdentIndex(string ident)
        {
            for(int i = 0; i < idents.Count; i++)
            {
                if (idents[i] == ident) return i;
            }
            throw new Exception("Ident is not found");
        }

        public int GetIdentSize()
        {
            return idents.Count;
        }

        public override string ToString()
        {
            return $"[Rule] {PlayerCount}Player Ident:{string.Join(",", idents)}, IdentInit:{string.Join(",", identInitValue)} Win:{string.Join(",", win)}, Lose:{string.Join(",", lose)}, Correct:{string.Join(",", cor)}, Wrong:{string.Join(",", wro)}";
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

}
