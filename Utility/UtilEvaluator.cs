using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace GotWell.Utility
{
    public class UtilEvaluator
    {
        //        private static object _evaluator = null;
        //        private static Type _evaluatorType = null;

        //        /// <summary>
        //        /// JScript代码
        //        /// </summary>
        //        private static readonly string _jscriptSource =
        //            @"class Evaluator
        //              {
        //                  public function Eval(expr : String) : String 
        //                  { 
        //                     return eval(expr); 
        //                  }
        //              }";

        //        static UtilEvaluator()
        //        {
        //            //构造JScript的编译驱动代码
        //            CodeDomProvider provider = CodeDomProvider.CreateProvider("JScript");

        //            CompilerParameters parameters;
        //            parameters = new CompilerParameters();
        //            parameters.GenerateInMemory = true;

        //            CompilerResults results;
        //            results = provider.CompileAssemblyFromSource(parameters, _jscriptSource);

        //            Assembly assembly = results.CompiledAssembly;
        //            _evaluatorType = assembly.GetType("Evaluator");

        //            _evaluator = Activator.CreateInstance(_evaluatorType);
        //        }

        //        public static object Eval(string statement)
        //        {
        //            return _evaluatorType.InvokeMember(
        //                        "Eval",
        //                        BindingFlags.InvokeMethod,
        //                        null,
        //                        _evaluator,
        //                        new object[] { statement }
        //                     );
        //        }

        public UtilEvaluator(EvaluatorItem[] items)
        {
            ConstructEvaluator(items);
        }

        public UtilEvaluator(Type returnType, string expression, string name)
        {
            EvaluatorItem[] items = { new EvaluatorItem(returnType, expression, name) };
            ConstructEvaluator(items);
        }

        public UtilEvaluator(EvaluatorItem item)
        {
            EvaluatorItem[] items = { item };
            ConstructEvaluator(items);
        }

        private void ConstructEvaluator(EvaluatorItem[] items)
        {
            ICodeCompiler comp = (new CSharpCodeProvider().CreateCompiler());
            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("system.dll");
            cp.ReferencedAssemblies.Add("system.data.dll");
            cp.ReferencedAssemblies.Add("system.xml.dll");
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            StringBuilder code = new StringBuilder();
            code.Append("using System; \n");
            code.Append("using System.Data; \n");
            code.Append("using System.Data.SqlClient; \n");
            code.Append("using System.Data.OleDb; \n");
            code.Append("using System.Xml; \n");
            code.Append("namespace ADOGuy { \n");
            code.Append("  public class _Evaluator { \n");
            foreach (EvaluatorItem item in items)
            {
                code.AppendFormat("    public {0} {1}() ",
                                  item.ReturnType.Name,
                                  item.Name);
                code.Append("{ ");
                code.AppendFormat("      return ({0}); ", item.Expression);
                code.Append("}\n");
            }
            code.Append("} }");

            CompilerResults cr = comp.CompileAssemblyFromSource(cp, code.ToString());
            if (cr.Errors.HasErrors)
            {
                StringBuilder error = new StringBuilder();
                error.Append("Error Compiling Expression: ");
                foreach (CompilerError err in cr.Errors)
                {
                    error.AppendFormat("{0}\n", err.ErrorText);
                }
                throw new Exception("Error Compiling Expression: " + error.ToString());
            }
            Assembly a = cr.CompiledAssembly;
            _Compiled = a.CreateInstance("ADOGuy._Evaluator");
        }

        public int EvaluateInt(string name)
        {
            return (int)Evaluate(name);
        }

        public string EvaluateString(string name)
        {
            return (string)Evaluate(name);
        }

        public bool EvaluateBool(string name)
        {
            return (bool)Evaluate(name);
        }

        public object Evaluate(string name)
        {
            MethodInfo mi = _Compiled.GetType().GetMethod(name);
            return mi.Invoke(_Compiled, null);
        }

        static public int EvaluateToInteger(string code)
        {
            UtilEvaluator eval = new UtilEvaluator(typeof(int), code, staticMethodName);
            return (int)eval.Evaluate(staticMethodName);
        }

        static public double EvaluateToDouble(string code)
        {
            UtilEvaluator eval = new UtilEvaluator(typeof(double), code, staticMethodName);
            return (double)eval.Evaluate(staticMethodName);
        }

        static public string EvaluateToString(string code)
        {
            UtilEvaluator eval = new UtilEvaluator(typeof(string), code, staticMethodName);
            return (string)eval.Evaluate(staticMethodName);
        }

        static public bool EvaluateToBool(string code)
        {
            UtilEvaluator eval = new UtilEvaluator(typeof(bool), code, staticMethodName);
            return (bool)eval.Evaluate(staticMethodName);
        }

        static public object EvaluateToObject(string code)
        {
            UtilEvaluator eval = new UtilEvaluator(typeof(object), code, staticMethodName);
            return eval.Evaluate(staticMethodName);
        }

        const string staticMethodName = "__foo";
        Type _CompiledType = null;
        object _Compiled = null;
    }

    public class EvaluatorItem
    {
        public EvaluatorItem(Type returnType, string expression, string name)
        {
            ReturnType = returnType;
            Expression = expression;
            Name = name;
        }

        public Type ReturnType;
        public string Name;
        public string Expression;
    }
}

