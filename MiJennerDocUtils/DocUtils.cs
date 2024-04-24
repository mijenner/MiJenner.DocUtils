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

            // Visit all top-level types and delegates in the parsed source
            foreach (var syntaxNode in root.DescendantNodes().OfType<MemberDeclarationSyntax>().Where(node => node.Parent is CompilationUnitSyntax || node.Parent is NamespaceDeclarationSyntax))
            {
                switch (syntaxNode)
                {
                    case ClassDeclarationSyntax classDeclaration:
                        string classAccess = GetAccessModifier(classDeclaration.Modifiers, classDeclaration);
                        Console.WriteLine($"{classAccess} Class: {classDeclaration.Identifier.ValueText}");
                        ListMembers(classDeclaration);
                        break;
                    case EnumDeclarationSyntax enumDeclaration:
                        string enumAccess = GetAccessModifier(enumDeclaration.Modifiers, enumDeclaration);
                        Console.WriteLine($"{enumAccess} Enum: {enumDeclaration.Identifier.ValueText}");
                        foreach (var enumMember in enumDeclaration.Members)
                        {
                            Console.WriteLine($"  Enum Member: {enumMember.Identifier.ValueText}");
                        }
                        break;
                    case InterfaceDeclarationSyntax interfaceDeclaration:
                        string interfaceAccess = GetAccessModifier(interfaceDeclaration.Modifiers, interfaceDeclaration);
                        Console.WriteLine($"{interfaceAccess} Interface: {interfaceDeclaration.Identifier.ValueText}");
                        ListMembers(interfaceDeclaration);
                        break;
                    case DelegateDeclarationSyntax delegateDeclaration:
                        string delegateAccess = GetAccessModifier(delegateDeclaration.Modifiers, delegateDeclaration);
                        Console.WriteLine($"{delegateAccess} Delegate: {delegateDeclaration.Identifier.ValueText} (Return Type: {delegateDeclaration.ReturnType})");
                        ListParameters(delegateDeclaration.ParameterList);
                        break;
                }
            }
        }

        private static void ListMembers(TypeDeclarationSyntax typeDeclaration)
        {
            foreach (var member in typeDeclaration.Members)
            {
                switch (member)
                {
                    case PropertyDeclarationSyntax property:
                        string propAccess = GetAccessModifier(property.Modifiers, property);
                        Console.WriteLine($"  {propAccess} Property: {property.Identifier.ValueText} (Type: {property.Type})");
                        break;
                    case FieldDeclarationSyntax field:
                        string fieldAccess = GetAccessModifier(field.Modifiers, field);
                        foreach (var variable in field.Declaration.Variables)
                        {
                            Console.WriteLine($"  {fieldAccess} Field: {variable.Identifier.ValueText} (Type: {field.Declaration.Type})");
                        }
                        break;
                    case ConstructorDeclarationSyntax constructor:
                        string ctorAccess = GetAccessModifier(constructor.Modifiers, constructor);
                        Console.WriteLine($"  {ctorAccess} Constructor: {constructor.Identifier.ValueText}");
                        ListParameters(constructor.ParameterList);
                        break;
                    case MethodDeclarationSyntax method:
                        string methodAccess = GetAccessModifier(method.Modifiers, method);
                        Console.WriteLine($"  {methodAccess} Method: {method.Identifier.ValueText}");
                        ListParameters(method.ParameterList);
                        break;
                    case EventFieldDeclarationSyntax eventField:
                        string eventAccess = GetAccessModifier(eventField.Modifiers, eventField);
                        foreach (var variable in eventField.Declaration.Variables)
                        {
                            Console.WriteLine($"  {eventAccess} Event: {variable.Identifier.ValueText} (Type: {eventField.Declaration.Type})");
                        }
                        break;
                }
            }
        }

        private static void ListParameters(BaseParameterListSyntax parameterList)
        {
            if (parameterList != null)
            {
                foreach (var parameter in parameterList.Parameters)
                {
                    Console.WriteLine($"    Parameter: {parameter.Identifier.ValueText} (Type: {parameter.Type})");
                }
            }
        }

        private static string GetAccessModifier(SyntaxTokenList modifiers, SyntaxNode syntaxNode)
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
            {
                // Determine default access based on the type of the node
                if (syntaxNode is ClassDeclarationSyntax ||
                    syntaxNode is StructDeclarationSyntax ||
                    syntaxNode is InterfaceDeclarationSyntax ||
                    syntaxNode is EnumDeclarationSyntax ||
                    syntaxNode is DelegateDeclarationSyntax)
                    return "internal";  // Default for top-level types
                else
                    return "private";  // Default for members within types
            }
        }
    }
}
