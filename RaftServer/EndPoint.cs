using System;
using System.Collections.Generic;
using System.Text;

namespace RaftServer
{
    public static class EndPoint
    {
        public static Dictionary<string, Delegate> EndPoints = new Dictionary<string, Delegate>();

        public static void Store(string endPoint, Delegate proccess) {

        }

    }
}
