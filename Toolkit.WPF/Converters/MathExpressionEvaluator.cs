using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Toolkit.WPF.Converters
{
    public class MathExpressionEvaluator : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            this._Value = value;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isNumeric = targetType == typeof(float)
                         || targetType == typeof(double)
                         || targetType == typeof(int)
                         || targetType == typeof(uint);

            if (isNumeric)
            {
                return Evaluator.Evaluate(value.ToString(), this._Value, targetType);
            }

            return value;
        }

        /// <summary>
        /// 評価するクラス
        /// </summary>
        private class Evaluator
        {
            public static object Evaluate(string expression, object defaultValue, Type targetType)
            {
                return new Evaluator().Analyze(expression, defaultValue, targetType);
            }

            private enum Operator
            {
                None,
                Addition,
                Subtraction,
                Multiplication,
                Division,
            }

            public object Analyze(string expression, object value, Type targetType)
            {
                var builder = new StringBuilder();

                for (int i = 0; i < expression.Length; i++)
                {
                    switch (expression[i])
                    {
                        case ' ':
                            break;
                        case '+':
                            this._Operator = Operator.Addition;
                            break;
                        case '-':
                            this._Operator = Operator.Subtraction;
                            break;
                        case '*':
                            this._Operator = Operator.Multiplication;
                            break;
                        case '/':
                            this._Operator = Operator.Division;
                            break;
                        default:
                            builder.Append(expression[i]);
                            break;
                    }
                }

                if( targetType == typeof(float) )
                {
                    switch (this._Operator)
                    {
                        case Operator.Addition:
                            return (float)value + float.Parse(builder.ToString());
                        case Operator.Subtraction:
                            return (float)value - float.Parse(builder.ToString());
                        case Operator.Multiplication:
                            return (float)value * float.Parse(builder.ToString());
                        case Operator.Division:
                            return (float)value / float.Parse(builder.ToString());
                        default:
                            return float.Parse(builder.ToString());
                    }
                }

                if (targetType == typeof(double))
                {
                    switch (this._Operator)
                    {
                        case Operator.Addition:
                            return (double)value + double.Parse(builder.ToString());
                        case Operator.Subtraction:
                            return (double)value - double.Parse(builder.ToString());
                        case Operator.Multiplication:
                            return (double)value * double.Parse(builder.ToString());
                        case Operator.Division:
                            return (double)value / double.Parse(builder.ToString());
                        default:
                            return double.Parse(builder.ToString());
                    }
                }

                if (targetType == typeof(int))
                {
                    switch (this._Operator)
                    {
                        case Operator.Addition:
                            return (int)value + int.Parse(builder.ToString());
                        case Operator.Subtraction:
                            return (int)value - int.Parse(builder.ToString());
                        case Operator.Multiplication:
                            return (int)value * int.Parse(builder.ToString());
                        case Operator.Division:
                            return (int)value / int.Parse(builder.ToString());
                        default:
                            return int.Parse(builder.ToString());
                    }
                }

                throw new InvalidOperationException();
            }

            private Operator _Operator = Operator.None;
        }

        private object _Value;
    }
}
