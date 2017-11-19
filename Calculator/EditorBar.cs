using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Calculator
{
    class EditorBar : INotifyPropertyChanged
    {
        #region ClassAttributes
        /// <summary>
        /// The result of the actual computation
        /// </summary>
        public double Result { get; set; }

        /// <summary>
        /// Boolen for check if we printed a computation result, and the next button hit should clear the cached computation
        /// </summary>
        private bool clearCachedForNextInput;


        private string editorBarContent;


        /// <summary>
        /// The actually displayed content in the UI TextBox, this is displayed in the lower textbox
        /// </summary>
        public string EditorBarContent
        {
            get { return editorBarContent; }
            set
            {
                editorBarContent = value;
                PropertyChange("EditorBarContent");
            }
        }

        /// <summary>
        /// A string containing the actual computation, this is displayed in the upper textbox
        /// </summary>
        private string cachedComputation { get; set; }

        public string CachedComputation
        {
            get { return cachedComputation; }
            set
            {
                cachedComputation = value;
                PropertyChange("CachedComputation");
            }
        }

        /// <summary>
        /// A list containing the finished computations
        /// </summary>
        public List<string> ContentToWrite { get; set; }
        #endregion
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public EditorBar()
        {
            Result = 0f;
            EditorBarContent = "";
            CachedComputation = "";
            ContentToWrite = new List<string>();
            clearCachedForNextInput = false;
        }

        #region MyMethods

        /// <summary>
        /// Handle the input from the UI (numbers and computation symbols).
        /// This method is always called if a button is clicked, except for save button.
        /// </summary>
        /// <param name="input"></param>
        public void HandleNextInput(string input)
        {
            if(clearCachedForNextInput)
            {
                CachedComputation = "";
                clearCachedForNextInput = false;
            }
            switch (input)
            {
                case "0":
                case "1":
                case "2":
                case "3":
                case "4":
                case "5":
                case "6":
                case "7":
                case "8":
                case "9":
                    if (EditorBarContent.Length < 10)
                    {
                        EditorBarContent += input;
                    }
                    break;
                case "DEL":
                    if (EditorBarContent.Length > 0)
                    {
                        EditorBarContent = EditorBarContent.Remove(EditorBarContent.Length - 1);
                    }
                    break;
                case ".":
                    if (!EditorBarContent.Contains("."))
                    {
                        if (EditorBarContent.Length > 0)
                        {
                            EditorBarContent += input;
                        }
                        else
                        {
                            EditorBarContent += "0.";
                        }
                    }
                    break;
                case "÷":
                    if (EditorBarContent.Length > 0)
                    {
                        CachedComputation += EditorBarContent;
                        EditorBarContent = "";
                        CachedComputation += " / ";
                    }
                    break;
                case "x":
                    if (EditorBarContent.Length > 0)
                    {
                        CachedComputation += EditorBarContent;
                        EditorBarContent = "";
                        CachedComputation += " * ";
                    }
                    break;
                case "-":
                    if (EditorBarContent.Length > 0)
                    {
                        CachedComputation += EditorBarContent;
                        EditorBarContent = "";
                        CachedComputation += " - ";
                    }
                    break;
                case "+":
                    if (EditorBarContent.Length > 0)
                    {
                        CachedComputation += EditorBarContent;
                        EditorBarContent = "";
                        CachedComputation += " + ";
                    }
                    break;
                case "√":
                    if (EditorBarContent.Length > 0)
                    {
                        CachedComputation += EditorBarContent;
                        CachedComputation += " sqrt";
                        Result = Evaluate(CachedComputation);
                        CachedComputation += "=" + FirstNineCaharcters(Result);
                        ContentToWrite.Add(CachedComputation);
                        CachedComputation = "";
                        EditorBarContent = FirstNineCaharcters(Result);
                    }
                    break;
                case "1/x":
                    if (EditorBarContent.Length > 0)
                    {
                        CachedComputation += EditorBarContent;
                        CachedComputation += " inv";
                        Result = Evaluate(CachedComputation);
                        CachedComputation += "=" + FirstNineCaharcters(Result);
                        ContentToWrite.Add(CachedComputation);
                        CachedComputation = "";
                        EditorBarContent = FirstNineCaharcters(Result);
                    }
                    break;
                case "±":
                    if (EditorBarContent.Length > 0 && EditorBarContent != "0")
                    {
                        //Debug.WriteLine("EditorBarContent[0] = " + EditorBarContent[0]);
                        if (EditorBarContent[0] == '-')
                        {
                            EditorBarContent = EditorBarContent.TrimStart(new char[] { '-' });
                        }
                        else
                        {
                            EditorBarContent = '-' + EditorBarContent;
                        }
                    }
                    break;
                case "CE":
                    EditorBarContent = "";
                    break;
                case "C":
                    CachedComputation = "";
                    EditorBarContent = "";
                    break;
                case "=":
                    if (EditorBarContent.Length > 0)
                    {
                        CachedComputation += EditorBarContent;
                        //Debug.WriteLine("Computation = {}", CachedComputation);
                        Result = Evaluate(CachedComputation);
                        CachedComputation += "=" + FirstNineCaharcters(Result);
                        ContentToWrite.Add(CachedComputation);
                        EditorBarContent = FirstNineCaharcters(Result);
                        clearCachedForNextInput = true;
                    }
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// Formatting function for only printing the first nine characters to the screen
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string FirstNineCaharcters(double input)
        {
            var stringToTruncate = input.ToString("0.########");
            var result = !String.IsNullOrWhiteSpace(stringToTruncate) && stringToTruncate.Length >= 9
                ? stringToTruncate.Substring(0, 9)
                : stringToTruncate;
            return result;
        }

        /// <summary>
        /// Evaluate the input string as a mathematical expression
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static double Evaluate(String input)
        {
            Stack<String> ops = new Stack<String>();
            Stack<Double> vals = new Stack<Double>();

            String bracket = "( " + input + " )";
            var expr = bracket.Split(new string[] { " " }, StringSplitOptions.None);
            for (int i = 0; i < expr.Length; i++)
            {
                String s = expr[i];
                Debug.WriteLine("String = " + s);
                if (s.Equals("(")) { }
                else if (s.Equals("+")) ops.Push(s);
                else if (s.Equals("-")) ops.Push(s);
                else if (s.Equals("*")) ops.Push(s);
                else if (s.Equals("/")) ops.Push(s);
                else if (s.Equals("sqrt")) ops.Push(s);
                else if (s.Equals("inv")) ops.Push(s);
                else if (s.Equals(")"))
                {
                    int count = ops.Count;
                    while (count > 0)
                    {
                        String op = ops.Pop();
                        double v = vals.Pop();
                        if (op.Equals("+")) v = vals.Pop() + v;
                        else if (op.Equals("-")) v = vals.Pop() - v;
                        else if (op.Equals("*")) v = vals.Pop() * v;
                        else if (op.Equals("/")) v = vals.Pop() / v;
                        else if (op.Equals("sqrt")) v = Math.Sqrt(v);
                        else if (op.Equals("inv")) v = 1f / v;
                        vals.Push(v);

                        count--;
                    }
                }
                else vals.Push(Double.Parse(s));
            }
            return vals.Pop();
        }
        #endregion

        #region ParentInterfaceMethods

        /// <summary>
        /// INotifyPropertyChanged interface method to notify the UI the given property is changed in the class.
        /// </summary>
        /// <param name="propertyName"></param>
        private void PropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}