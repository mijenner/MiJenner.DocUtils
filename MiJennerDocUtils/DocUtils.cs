using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.IO;
using System.Linq;

namespace MiJenner.DocUtils
{
    public static class DocUtils
    {
        public static void WriteDoc(string codePath)
        {
            // Writing out full path: 
            Console.WriteLine("Reading source code from:");
            Console.WriteLine(Path.GetFullPath(codePath));

            if (!File.Exists(codePath))
            {
                Console.WriteLine("File doesn't exist, exiting");
                return;
            }

            string code = File.ReadAllText(codePath);

            // Parse the code into a SyntaxTree
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            // Visit all classes in the parsed source
            foreach (ClassDeclarationSyntax classDeclaration in root.DescendantNodes().OfType<ClassDeclarationSyntax>())
            {
                Console.WriteLine($"Class: {classDeclaration.Identifier.ValueText}");

                // List all properties with their access modifiers
                foreach (var property in classDeclaration.Members.OfType<PropertyDeclarationSyntax>())
                {
                    string accessModifier = GetAccessModifier(property.Modifiers);
                    Console.WriteLine($"  {accessModifier} Property: {property.Identifier.ValueText} (Type: {property.Type})");
                }

                // List all fields with their access modifiers
                foreach (var field in classDeclaration.Members.OfType<FieldDeclarationSyntax>())
                {
                    string accessModifier = GetAccessModifier(field.Modifiers);
                    foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
                    {
                        Console.WriteLine($"  {accessModifier} Field: {variable.Identifier.ValueText} (Type: {field.Declaration.Type})");
                    }
                }

                // List all constructors with their access modifiers and parameters
                foreach (var constructor in classDeclaration.Members.OfType<ConstructorDeclarationSyntax>())
                {
                    string accessModifier = GetAccessModifier(constructor.Modifiers);
                    Console.WriteLine($"  {accessModifier} Constructor: {constructor.Identifier.ValueText}");

                    // List all parameters for each constructor
                    foreach (var parameter in constructor.ParameterList.Parameters)
                    {
                        Console.WriteLine($"    Parameter: {parameter.Identifier.ValueText} (Type: {parameter.Type})");
                    }
                }

                // List all methods with their access modifiers and parameters
                foreach (var method in classDeclaration.Members.OfType<MethodDeclarationSyntax>())
                {
                    string accessModifier = GetAccessModifier(method.Modifiers);
                    Console.WriteLine($"  {accessModifier} Method: {method.Identifier.ValueText}");

                    // List all parameters for each method
                    foreach (var parameter in method.ParameterList.Parameters)
                    {
                        Console.WriteLine($"    Parameter: {parameter.Identifier.ValueText} (Type: {parameter.Type})");
                    }
                }
            }

        }

        private static string GetAccessModifier(SyntaxTokenList modifiers)
        {
            if (modifiers.Any(SyntaxKind.PublicKeyword))
                return "public";
            else if (modifiers.Any(SyntaxKind.PrivateKeyword))
                return "private";
            else if (modifiers.Any(SyntaxKind.ProtectedKeyword))
                return "protected";
            else if (modifiers.Any(SyntaxKind.InternalKeyword))
                return "internal";
            else
                return "private"; // Default access for class members 
        }
    }
}
