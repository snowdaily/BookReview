using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using BookReview.Models;
using BookReview.Models.TemplateModels;
using BookReview.Models.ViewModels;
using Domain;
using System.Text;

namespace BookReview.Controllers
{
    public class BookController : Controller
    {
        private readonly BookEntities _bookEntities = new BookEntities();
        private readonly MainEntities _mainEntities = new MainEntities();

        //
        // GET: /Book/

        public ActionResult Index(Guid? id)
        {
            return View();
        }

        public ActionResult AddComment(BookComment bookComment)
        {
            bookComment.CreateDate = DateTime.UtcNow;
            bookComment.Id = Guid.NewGuid();

            var comments = new Comments() { IsEnable = "1" };

            Mapper.CreateMap<BookComment, Comments>();
            Mapper.Map(bookComment, comments);

            _bookEntities.Comments.Add(comments);
            _bookEntities.SaveChanges();

            return RedirectToAction("BookDetail", new { id = bookComment.BookId });
        }

        public ActionResult BookDetail(Guid id)
        {
            if (id == null) throw new ArgumentNullException("id");

            var book = _bookEntities.Books.Find(id);
            var model = new BookBookDetail();

            Mapper.CreateMap<Books, BookBookDetail>();
            Mapper.Map(book, model);

            var listComments = _bookEntities.Comments.Where(c => c.BookId.Equals(id)).OrderByDescending(c => c.CreateDate).ToList();
            model.ListBookComment = new List<BookComment>();

            Mapper.CreateMap<Comments, BookComment>();
            Mapper.Map(listComments, model.ListBookComment);

            // Init book comment
            model.BookComment = new BookComment() { BookId = id, CreaterId = User.Identity.IsAuthenticated ? ((UserProfile)_mainEntities.UserProfile.FirstOrDefault(u => u.UserName.Equals(User.Identity.Name))).UserId : -1 };

            //BookBookDetail model = new BookBookDetail()
            //    {
            //        Id = book.Id,
            //        Author = book.Author,
            //        Name = book.Name,
            //        PublishDate = (DateTime)book.PublishDate,
            //        Publisher = book.Publisher
            //    };

            return View(model);
        }

        [HttpPost]
        public string SubmitComment(BookComment bookComment)
        {
            bookComment.CreateDate = DateTime.UtcNow;
            bookComment.Id = Guid.NewGuid();

            var comments = new Comments() { IsEnable = "1" };

            Mapper.CreateMap<BookComment, Comments>();
            Mapper.Map(bookComment, comments);

            _bookEntities.Comments.Add(comments);
            _bookEntities.SaveChanges();

            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("<dt>{0}</dt>", bookComment.CreaterId == null ? "None" : bookComment.CreaterId.ToString()));
            sb.Append(string.Format(@"<div class=""rateit"" data-rateit-value=""{0}"" data-rateit-ispreset=""true"" data-rateit-readonly=""true""></div>", bookComment.Rating));
            sb.Append(string.Format("<dd>{0}{1}</dd>", bookComment.CreateDate, bookComment.Content));

            return sb.ToString();
        }
    }
}
