using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WritPlat.Models
{
    public class Post
    {
        public int PostId { get; set; }
        public int BookId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
    }
}
