using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WritPlat.Models
{
    public class Book
    {
        public int BookId { get; set; }
        public string BookName { get; set; }
        public DateTime PublishDate { get; set; }
        public int Rating { get; set; }
        public int Genre { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public User user {get;set;}
        //public Post post { get; set; }
        //public List<Post> posts { get; set; }
        //public Book()
        //{
        //    posts = new List<Post>();
        //}
    }
}
