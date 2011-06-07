using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications;
using Microsoft.CSharp;
using PageTypeBuilder.Migrations;
using PageTypeBuilder.Specs.Synchronization;
using PageTypeBuilder.Synchronization.Hooks;

namespace PageTypeBuilder.Specs.Migrations.Validation
{
    public class given_two_migrations_in_different_namespaces : SynchronizationSpecs
    {
        static Assembly assembly;
        static MigrationsHook migrationsHook;
        static Exception thrownException;

        Establish context = () =>
        {
            var code = "using PageTypeBuilder.Migrations;"
                + "namespace Namespace1 {"
                + "public class Migration1 : Migration {"
                + "public override void Execute(){}"
                + "}}"
                + "namespace Namespace2 {"
                + "public class Migration2 : Migration {"
                + "public override void Execute(){}"
                + "}}";
            assembly = BuildAssembly(code);

            SyncContext.AssemblyLocator.Add(assembly);

            migrationsHook = new MigrationsHook();
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
            compilerparams.ReferencedAssemblies.Add("PageTypeBuilder.Migrations.dll");
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

        public class AssemblySpecification
        {
            public AssemblySpecification()
            {
                CompilerParameters = new CompilerParameters();
                CompilerParameters.GenerateExecutable = false;
                CompilerParameters.GenerateInMemory = true;

                Namespaces = new List<Namespace>();
            }

            public CompilerParameters CompilerParameters { get; private set; }

            public void AddAssemblyReference(string assemblyName)
            {
                CompilerParameters.ReferencedAssemblies.Add(assemblyName);
            }

            public IList<Namespace> Namespaces { get; private set; }

            public string GetCode()
            {
                return base.ToString();
            }
        }

        public class Namespace
        {
            public Namespace(string name)
            {
                Name = name;
            }

            public string Name { get; private set; }
        }

        private static Assembly BuildAssembly(Action<AssemblySpecification> assemblySpecificationExpression)
        {
            var specification = new AssemblySpecification();
            CSharpCodeProvider provider =
              new CSharpCodeProvider();
            ICodeCompiler compiler = provider.CreateCompiler();
            CompilerResults results =
               compiler.CompileAssemblyFromSource(specification.CompilerParameters, specification.GetCode());
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
            () => thrownException = Catch.Exception(() => migrationsHook.PreSynchronization(new SynchronizationHookContext(SyncContext.AssemblyLocator)));

        It should_throw_an_exception
            = () => thrownException.ShouldNotBeNull();

        It should_throw_a_PageTypeBuilderException
            = () => thrownException.ShouldBeOfType<PageTypeBuilderException>();
    }
}
