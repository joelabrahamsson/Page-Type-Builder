using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Linq;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Machine.Specifications;
using Microsoft.CSharp;
using PageTypeBuilder.Specs.Synchronization;
using PageTypeBuilder.Synchronization.Hooks;

namespace PageTypeBuilder.Specs.Hooks
{
    [Subject("Synchronization")]
    public class given_a_class_implementing_IPreSynchronizationHook
        : SynchronizationSpecs
    {
        static Assembly assembly;
        Establish context = () =>
        {
            var code = "using PageTypeBuilder.Synchronization.Hooks;" 
                + "public class ExamplePreSynchronizationHook : IPreSynchronizationHook {"
                           + "public static bool PreSynchronizationHasBeenInvoked;"

                           + "public void PreSynchronization(ISynchronizationHookContext context)"
                           + "{"
                           + "PreSynchronizationHasBeenInvoked = true;"
                           + "}}";
            assembly = BuildAssembly(code);
            
            SyncContext.AssemblyLocator.Add(assembly);
            //SyncContext.AssemblyLocator.Add(typeof(ExamplePreSynchronizationHook).Assembly);
        };

        private static Assembly BuildAssembly(string code)
        {
            CSharpCodeProvider provider =
               new CSharpCodeProvider();
            ICodeCompiler compiler = provider.CreateCompiler();
            CompilerParameters compilerparams = new CompilerParameters();
            compilerparams.GenerateExecutable = false;
            compilerparams.GenerateInMemory = true;
            compilerparams.ReferencedAssemblies.Add("PageTypeBuilder.dll");
            CompilerResults results =
               compiler.CompileAssemblyFromSource(compilerparams, code);
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compiler Errors :\r\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat("Line {0},{1}\t: {2}\n",
                           error.Line, error.Column, error.ErrorText);
                }
                throw new Exception(errors.ToString());
            }
            else
            {
                return results.CompiledAssembly;
            }
        }

        Because of =
            () => SyncContext.PageTypeSynchronizer.SynchronizePageTypes();

        It should_invoke_the_class_PreSynchronization_method
            = () => ((bool)assembly.GetTypes().Where(t => t.Name == "ExamplePreSynchronizationHook").First().InvokeMember("PreSynchronizationHasBeenInvoked", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public, null, null, null)).ShouldBeTrue(); //ExamplePreSynchronizationHook.PreSynchronizationHasBeenInvoked.ShouldBeTrue();
    }

    //public class ExamplePreSynchronizationHook : IPreSynchronizationHook
    //{
    //    public static bool PreSynchronizationHasBeenInvoked { get; set; }

    //    public void PreSynchronization(ISynchronizationHookContext context)
    //    {
    //        PreSynchronizationHasBeenInvoked = true;    
    //    }
    //}
}
