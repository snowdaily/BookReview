using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BookReview.Models.TemplateModels;

namespace BookReview.Models.ViewModels
{
    public class IndexModel
    {
        public List<BookBox> listBookBox { get; set; }
    }
}