using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QSimulator
{
    class Node
    {
        enum Process {
            None,
            Plus, Minus, Mult, Div, Quot, Mod, // 算術演算: 足す・引く・掛ける・割る・商・余り
            GRE, EqGRE, LESS, EqLES, EQ, NEQ, // 比較演算: >, >=, <, <=, =, !=
        }
        public Node(string text, Rule rule)
        {
            text = text.Replace(" ", "");
            Process process = Process.None;
            int head = 0, faze = 0;
            Value value1, value2;
            value1 = null;
            value2 = null;
            string ident; bool isInstance;
            ident = "";

            if (!char.IsLetter(text, 0)) throw new Exception("Variable naming error");
            while (head < text.Length)
            {
                switch (faze) {
                    case 0:
                        // Ident1
                        if (char.IsLetterOrDigit(text, head))
                        {
                            ident += text[head++].ToString();
                        }
                        else
                        {
                            value1 = new Value(rule.GetIdentIndex(ident), false);
                            ident = "";
                            faze++;
                        }
                        break;
                    case 1:
                        // Operator
                        switch (text[head])
                        {
                            // 算術演算
                            case '+':
                                process = Process.Plus;
                                break;
                            case '-':
                                process = Process.Minus;
                                break;
                            case '*':
                                process = Process.Mult;
                                break;
                            case '/':
                                process = Process.Div;
                                break;
                            case ':':
                                process = Process.Div;
                                break;
                            case '%':
                                process = Process.Mod;
                                break;
                            // 比較演算
                            case '>':
                                if (text[head + 1] == '=')
                                {
                                    process = Process.EqGRE;
                                    head++;
                                }
                                else
                                {
                                    process = Process.GRE;
                                }
                                break;
                            case '<':
                                if (text[head + 1] == '=')
                                {
                                    process = Process.EqLES;
                                    head++;
                                }
                                else
                                {
                                    process = Process.LESS;
                                }
                                break;
                            case '!':
                                if(text[head + 1] == '=')
                                {
                                    process = Process.NEQ;
                                    head++;
                                }
                                else
                                {
                                    throw new Exception("Operator Error");
                                }
                                break;
                            case '=':
                                if(text[head+1] == '=')
                                {
                                    process = Process.EQ;
                                    head++;
                                }
                                else
                                {
                                    throw new Exception("Operator Error");
                                }
                                break;
                            default:
                                throw new Exception("Operator Error");
                        }
                        faze++;
                        head++;
                        break;
                    case 2:
                        // Judge: Ident or Inst
                        if (char.IsDigit(text, head))
                        {
                            isInstance = true;
                            faze = 3;
                        }
                        else if (char.IsLetter(text, head))
                        {
                            isInstance = false;
                            faze = 4;
                        }
                        else
                        {
                            throw new Exception("Variable naming error");
                        }
                        break;
                    case 3:
                        // Value2 is Instance Value
                        if (char.IsDigit(text, head))
                        {
                            ident += text[head++].ToString();
                        }
                        else
                        {
                            throw new Exception("Variable naming error");
                        }
                        break;
                    case 4:
                        // Value2 is Identifier
                        if(char.IsLetterOrDigit(text, head))
                        {
                            ident += text[head++].ToString();
                        }
                        else
                        {
                            throw new Exception("Variable naming error");
                        }
                        break;
                    default:
                        throw new Exception("Unknown Error: Faze is invalid");
                }
            }

            if(faze == 3)
            {
                value2 = new Value(int.Parse(ident), true);
            }
            else if(faze == 4)
            {
                value2 = new Value(rule.GetIdentIndex(ident), false);
            }
            else
            {
                throw new Exception("Unknown Error: Faze is invalid");
            }

            Console.WriteLine($"Node:: {process} {value1} {value2}");
        }
    }

    class Value
    {
        enum Type { ident, instance }

        public int Number { get; }

        private Type type;
        public bool IsInstance
        {
            get
            {
                return type == Type.instance;
            }
        }

        public Value(int n, bool isInstance)
        {
            type = isInstance ? Type.instance : Type.ident;
            Number = n;
        }

        public override string ToString()
        {
            string tstr = (type == Type.ident) ? "Ident" : "Instance";
            return $"{tstr} {Number}";
        }
    }
}
