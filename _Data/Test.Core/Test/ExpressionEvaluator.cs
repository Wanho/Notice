using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Test.Core
{
	internal class ExpressionEvaluator
	{
		public ExpressionEvaluator()
		{
		}

		public static bool Evaluate(string expression, List<ParameterSource> source)
		{
			bool? isOperand;
			string str;
			string str1;
			string name;
			List<ExpressionToken> expressionTokens = new List<ExpressionToken>();
			Stack<ExpressionToken> expressionTokens1 = new Stack<ExpressionToken>();
			StringReader stringReader = new StringReader(expression);
		Label0:
			while (stringReader.Peek() >= 0)
			{
				ExpressionToken expressionToken = ExpressionEvaluator.ReadToken(stringReader);
				expressionToken.SetCode();
				isOperand = expressionToken.IsOperand;
				if ((isOperand.GetValueOrDefault() ? isOperand.HasValue : false))
				{
					expressionTokens.Add(expressionToken);
				}
				else if (expressionToken.Name == "(")
				{
					expressionTokens1.Push(expressionToken);
				}
				else if (expressionToken.Name != ")")
				{
					while (expressionTokens1.Count > 0)
					{
						ExpressionToken expressionToken1 = expressionTokens1.Peek();
						if (expressionToken1.OpCode.Order > expressionToken.OpCode.Order)
						{
							break;
						}
						expressionTokens1.Pop();
						expressionTokens.Add(expressionToken1);
					}
					expressionTokens1.Push(expressionToken);
				}
				else
				{
					while (expressionTokens1.Count > 0)
					{
						ExpressionToken expressionToken2 = expressionTokens1.Pop();
						if (expressionToken2.Name == "(")
						{
							goto Label0;
						}
						expressionTokens.Add(expressionToken2);
					}
				}
			}
			while (expressionTokens1.Count > 0)
			{
				expressionTokens.Add(expressionTokens1.Pop());
			}
			foreach (ExpressionToken expressionToken3 in expressionTokens)
			{
				isOperand = expressionToken3.IsOperand;
				if ((isOperand.GetValueOrDefault() ? !isOperand.HasValue : true))
				{
					ExpressionToken expressionToken4 = expressionTokens1.Pop();
					ExpressionToken expressionToken5 = expressionTokens1.Pop();
					bool flag = true;
					string str2 = null;
					double num = 0;
					if (expressionToken4.Name == "null")
					{
						flag = false;
					}
					else if (expressionToken4.Name == "empty")
					{
						str2 = "";
						flag = false;
					}
					else if (expressionToken4.Name == "true")
					{
						str2 = "true";
						flag = false;
					}
					else if (expressionToken4.Name == "false")
					{
						str2 = "false";
						flag = false;
					}
					else if (!expressionToken4.Name.StartsWith("'"))
					{
						if (source != null)
						{
							ParameterSource parameterSource = source.Find((ParameterSource p) => p.Name.MatchDataName(expressionToken4.Name));
							if (parameterSource != null)
							{
								str = parameterSource.Value.ToString();
							}
							else
							{
								str = null;
							}
						}
						else
						{
							str = null;
						}
						str2 = str;
						if (str2 == null)
						{
							flag = false;
						}
						else if (!double.TryParse(str2, out num))
						{
							flag = false;
						}
					}
					else
					{
						str2 = expressionToken4.Name.Trim(new char[] { '\'' });
						flag = false;
					}
					string str3 = null;
					double num1 = 0;
					if (expressionToken5.Name == "null")
					{
						flag = false;
					}
					else if (expressionToken5.Name == "empty")
					{
						str3 = "";
						flag = false;
					}
					else if (expressionToken5.Name == "true")
					{
						str3 = "true";
						flag = false;
					}
					else if (expressionToken5.Name == "false")
					{
						str3 = "false";
						flag = false;
					}
					else if (!expressionToken5.Name.StartsWith("'"))
					{
						if (source != null)
						{
							ParameterSource parameterSource1 = source.Find((ParameterSource p) => p.Name.MatchDataName(expressionToken5.Name));
							if (parameterSource1 != null)
							{
								object value = parameterSource1.Value;
								if (value != null)
								{
									str1 = value.ToString();
								}
								else
								{
									str1 = null;
								}
							}
							else
							{
								str1 = null;
							}
						}
						else
						{
							str1 = null;
						}
						str3 = str1;
						if (str3 == null)
						{
							flag = false;
						}
						else if (!double.TryParse(str3, out num1))
						{
							flag = false;
						}
					}
					else
					{
						str3 = expressionToken5.Name.Trim(new char[] { '\'' });
						flag = false;
					}
					bool flag1 = false;
					string opCode = expressionToken3.OpCode;
					if (expressionToken3.OpCode == ExpressionOpCode.And || expressionToken3.OpCode == ExpressionOpCode.Or)
					{
						if (opCode == "and")
						{
							flag1 = (str2 != "true" ? false : str3 == "true");
						}
						else if (opCode == "or")
						{
							flag1 = (str2 == "true" ? true : str3 == "true");
						}
					}
					else if (flag)
					{
						switch (opCode)
						{
							case "==":
							{
								flag1 = num == num1;
								break;
							}
							case "!=":
							case "<>":
							{
								flag1 = num != num1;
								break;
							}
							case ">=":
							{
								flag1 = num >= num1;
								break;
							}
							case "<=":
							{
								flag1 = num <= num1;
								break;
							}
							case ">":
							{
								flag1 = num > num1;
								break;
							}
							case "<":
							{
								flag1 = num < num1;
								break;
							}
						}
					}
					else if (opCode == "==")
					{
						flag1 = str2 == str3;
					}
					else if (opCode == "!=" || opCode == "<>")
					{
						flag1 = str2 != str3;
					}
					else if (opCode == ">=")
					{
						flag1 = str3.CompareTo(str2) >= 0;
					}
					else if (opCode == "<=")
					{
						flag1 = str3.CompareTo(str2) <= 0;
					}
					else if (opCode == ">")
					{
						flag1 = str3.CompareTo(str2) > 0;
					}
					else if (opCode == "<")
					{
						flag1 = str3.CompareTo(str2) < 0;
					}
					expressionTokens1.Push(new ExpressionToken()
					{
						Name = (flag1 ? "true" : "false")
					});
				}
				else
				{
					expressionTokens1.Push(expressionToken3);
				}
			}
			ExpressionToken expressionToken6 = expressionTokens1.Peek();
			if (expressionToken6 != null)
			{
				name = expressionToken6.Name;
			}
			else
			{
				name = null;
			}
			return name == "true";
		}

        public static ExpressionToken ReadToken(StringReader reader)
        {
            char chr;
            bool? isOperand;
            ExpressionToken expressionToken = new ExpressionToken();
            while (true)
            {
                Label7:
                int num = reader.Peek();
                if (num == -1)
                {
                    break;
                }

                chr = (char)num;
                if (chr > '!')
                {
                    switch (chr)
                    {
                        case '&':
                        case '(':
                        case ')':
                            {
                                break;
                            }
                        case '\'':
                            {
                                goto Label3;
                            }
                        default:
                            {
                                switch (chr)
                                {
                                    case '<':
                                    case '=':
                                    case '>':
                                        {
                                            break;
                                        }
                                    default:
                                        {
                                            if (chr == '|')
                                            {
                                                goto Label5;
                                            }
                                            goto Label3;
                                        }
                                }
                                break;
                            }
                    }
                }

                if (chr != ' ')
                {
                    goto Label6;
                }
                if (expressionToken.Name.Length <= 0)
                {
                    goto Label4;
                }
                else
                {
                    return expressionToken;
                }
                Label5:
                if (!expressionToken.IsOperand.HasValue)
                {
                    expressionToken.IsOperand = new bool?(false);
                }
                isOperand = expressionToken.IsOperand;
                if ((isOperand.GetValueOrDefault() ? isOperand.HasValue : false))
                {
                    return expressionToken;
                }
                ExpressionToken expressionToken1 = expressionToken;
                expressionToken1.Name = string.Concat(expressionToken1.Name, chr.ToString());
                goto Label4;
                Label3:
                if (!expressionToken.IsOperand.HasValue)
                {
                    expressionToken.IsOperand = new bool?(true);
                }
                isOperand = expressionToken.IsOperand;
                if ((isOperand.GetValueOrDefault() ? !isOperand.HasValue : true))
                {
                    return expressionToken;
                }
                ExpressionToken expressionToken2 = expressionToken;
                expressionToken2.Name = string.Concat(expressionToken2.Name, chr.ToString());
                Label4:
                reader.Read();
                goto Label7;
                Label6:
                if (chr == '!')
                {
                    goto Label5;
                }
                goto Label3;
            }

            return expressionToken;
        }
	}
}