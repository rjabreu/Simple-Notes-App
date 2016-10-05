using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiDemo1.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Password { get; set; }
    }
}