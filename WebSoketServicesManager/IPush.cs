using Hammer.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSoketServicesManager
{
  public  interface IPush
    {
      void Send(Iwotp iwotp);
      void Send(string uid, string message);
      void SendAll(string message);
      bool Contains(string uid);
    }
}
