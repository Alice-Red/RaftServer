using System;

namespace RaftServer
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