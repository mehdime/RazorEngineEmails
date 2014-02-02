using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication.Models
{
    public class WelcomeModel
    {
        public string UserName { get; set; }
        public IEnumerable<Gift> Gifts { get; set; } 
    }

    public class Gift
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
}
