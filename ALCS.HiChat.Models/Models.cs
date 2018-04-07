using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALCS.HiChat.Models
{
    public class User : IEquatable<User>
    {
        public string Name { get; set; }
        public bool Equals(User other) { return Name.Equals(other.Name); }
        public override bool Equals(Object o)
        {
            if (o == null)
            {
                return false;
            }
            if (!(o is User))
            {
                return false;
            }
            return Equals(o as User);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }

    public class Message
    { 
        public User Sender { get; set; }
        public User Destination { get; set; }
        public string Content { get; set; }
    }
}
