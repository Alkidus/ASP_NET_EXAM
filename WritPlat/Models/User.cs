using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WritPlat.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //public int BookId { get; set; }
        //public int PostId { get; set; }
        //public int LikeId { get; set; }
    }
}
