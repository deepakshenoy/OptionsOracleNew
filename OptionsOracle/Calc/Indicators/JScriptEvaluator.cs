/*
 * Copyright (c) 2004 Unknown
 *
 * This code is free software.
 * 
 */

using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.JScript;

namespace OptionsOracle.Calc.Indicators
{
    class JScriptEvaluator
    {
        public static int EvalToInteger(string statement)
        {
            string s = EvalToString(statement);
            return int.Parse(s.ToString());
        }

        public static double EvalToDouble(string statement)
        {
            string s = EvalToString(statement);
            return double.Parse(s);
        }

        public static string EvalToString(string statement)
        {
            object o = EvalToObject(statement);
            return o.ToString();
        }

        public static object EvalToObject(string statement)
        {
            return _evaluatorType.InvokeMember(
                        "Eval",
                        BindingFlags.InvokeMethod,
                        null,
                        _evaluator,
                        new object[] { statement }
                     );
        }

        static JScriptEvaluator()
        {
            CodeDomProvider compiler = CodeDomProvider.CreateProvider("JScript");
            
            CompilerParameters parameters;
            parameters = new CompilerParameters();
            parameters.GenerateInMemory = true;
            
            CompilerResults results;
            results = compiler.CompileAssemblyFromSource(parameters, _jscriptSource);

            Assembly assembly = results.CompiledAssembly;
            _evaluatorType = assembly.GetType("Evaluator.Evaluator");
            
            _evaluator = Activator.CreateInstance(_evaluatorType);
        }

        private static object _evaluator = null;
        private static Type _evaluatorType = null;

        private static readonly string _jscriptSource = 
            @"package Evaluator
            {
                class Evaluator
                {
                    public function Eval(expr : String) : String 
                    { 
                        return eval(expr); 
                    }
                }
            }";
    }
}
