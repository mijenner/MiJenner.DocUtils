using MiJenner.DocUtils;

namespace UsageExamples
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string codePath = @"TestSourceFile.cs"; 
            DocUtils.WriteDoc(codePath);
        }
    }
}
