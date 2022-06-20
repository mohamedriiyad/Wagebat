using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wagebat.Models
{
    public class CommentAttachment
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public int CommentId { get; set; }
        public Comment Comment { get; set; }
        public bool IsImage { get; set; }
    }
}
