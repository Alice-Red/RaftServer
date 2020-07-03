using System;

namespace RDSandboxCUI
{
    public class Program
    {
        public static void Main(params string[] args) {
            RaftServer raft = new RaftServer();
            
            
            Console.WriteLine($"-------------------------");
            Console.ReadKey(true);
        }
    }
}