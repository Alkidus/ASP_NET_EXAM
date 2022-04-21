using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WritPlat.Models;

namespace WritPlat.Controllers
{
    public class BookController : Controller
    {
        UserContext db;
        public BookController(UserContext context)
        {
            this.db = context;
            if (db.User.Count() == 0)
            {
                User admin = new User { Email = "admin@gmail.com", UserName = "admin", Password = "admin" };
                db.User.Add(admin);
                db.SaveChanges();
            }
        }
        private ReadBook GetBook(int id)
        {
            //using (ReadBookDB db = new ReadBookDB())
            //{
            var readBook = new ReadBook();
            //Book book = new Book();
            var book = db.Book.FirstOrDefault(x => x.BookId == id);
            if (book != null)
            {
                readBook.BookID = book.BookId;
                readBook.BookName = book.BookName;
                readBook.BookContent = book.Content;
                readBook.AuthorName = book.user.UserName;
                readBook.BookPublishDate = book.PublishDate;
                //readBook.Rating = Book.GetAverageLikes(book.Like.ToArray());
                //readBook.Genres = Book.GetGenresByInt((int)book.Genre);

                //var posts = book.Post.ToArray();
                //readBook.PostsUserName = new string[posts.Length];
                //readBook.PostsContent = new string[posts.Length];
                //for (int i = 0; i < posts.Length; i++)
                //{
                //    readBook.PostsContent[i] = posts[i].Content;
                //    readBook.PostsUserName[i] = posts[i].User.UserName;
                //}
                return readBook;
            }
            //}
            return null;
        }

        private Models.User GetCurrentUser()
        {
            var user = new Models.User();
            //using (ReadBookDB db = new ReadBookDB())
            //{
                GetCurrentUser(out user);
            //}
            return user;
        }

        private Models.User GetCurrentUser(out Models.User user)
        {
            if (User.Identity.IsAuthenticated)
            {
                return user = db.User.FirstOrDefault(x => x.UserName == User.Identity.Name);
            }
            return user = new Models.User();
        }



        public ActionResult Ratings(string id, int? genre)
        {
            var books = new SearchBook();
            //using (ReadBookDB db = new ReadBookDB())
            //{
                switch (id)
                {
                    case "ByGenges":
                        if (genre != null)
                        {
                            List<Book> listBooks = new List<Book>();
                            foreach (var item in db.Book)
                            {
                                if (((GenreType)item.Genre).HasFlag((GenreType)genre))
                                {
                                    listBooks.Add(item);
                                }
                            }
                            foreach (var item in listBooks)
                            {
                                //item.Rating = (int)Book.GetAverageLikes(item.Like.ToArray());
                            }
                            books.Result = listBooks.OrderByDescending(book => book.Rating).ToList();
                        }
                        else
                        {
                            books.IsTopByGenre = true;
                        }
                        //books.Genres = Book.FillGenresEnumModel(0);
                        break;
                    case "ByRating":
                        var allBooks = db.Book.ToArray();
                        foreach (var item in allBooks)
                        {
                            //item.Rating = (int)Book.GetAverageLikes(item.Like.ToArray());
                        }
                        books.Result = allBooks.OrderByDescending(book => book.Rating).Take(50).ToList();
                        break;
                    case "ByComments":
                        //books.Result = db.Book.OrderByDescending(book => book.Post.Count).Take(50).ToList();
                        break;
                }
            //}
            if (books.Result == null)
            {
                books.Result = new List<Book>();
            }

            return View(books);
        }

        public ActionResult Search()
        {
            return View(new SearchBook()
            {
                //Genres = Book.FillGenresEnumModel(0)
            });
        }

        [HttpPost]
        public ActionResult Search(SearchBook searchBook)
        {
            List<Book> resultBooks = null;
            //using (ReadBookDB db = new ReadBookDB())
            //{
                //if (searchBook.AuthorName != null)
                //{
                //    resultBooks = db.Book.Where(book => book.User.UserName == searchBook.AuthorName).ToList();
                //}
                if (searchBook.BookName != null)
                {
                    if (resultBooks != null)
                    {
                        var result1 = resultBooks.Where(book => book.BookName.Contains(searchBook.BookName)).ToList();
                        resultBooks = result1;
                    }
                    else
                    {
                        resultBooks = db.Book.Where(book => book.BookName.Contains(searchBook.BookName)).ToList();
                    }
                }
                if (resultBooks == null)
                {
                    resultBooks = db.Book.ToList();
                }
                foreach (var genre in searchBook.Genres)
                {
                    for (int i = 0; i < resultBooks.Count; i++)
                    {
                        if (genre.IsSelected)// && resultBooks[i].Genre == null)
                        {
                            resultBooks.Remove(resultBooks[i]);
                            i--;
                        }
                        else if (genre.IsSelected && !((GenreType)resultBooks[i].Genre).HasFlag(genre.GenreType))
                        {
                            resultBooks.Remove(resultBooks[i]);
                            i--;
                        }
                    }
                }
            //}
            searchBook.Result = resultBooks;
            return View(searchBook);
        }

        public ActionResult Read(string id, int? page = 1)
        {
            if (Int32.TryParse(id, out int ID))
            {
                var readBook = GetBook(ID);
                if (readBook != null)
                {
                    const int symbolsSize = 3024;
                    var content = readBook.BookContent;
                    var pagesCount = content.Length / symbolsSize;
                    ViewBag.PagesCount = pagesCount;

                    readBook.BookContent = string.Concat(content
                        .Skip(((int)page - 1) * symbolsSize)
                        .Take(symbolsSize));

                    return View(readBook);
                }
            }
            return Redirect("/home");
        }

        [HttpPost, ActionName("AddPost")]
        //[Authorize]
        public ActionResult AddPost(ReadBook model)
        {
            //using (ReadBookDB db = new ReadBookDB())
            //{
                GetCurrentUser(out Models.User currUser);
                db.Post.Add(new Post()
                {
                    Content = model.NewPostContent,
                    UserId = currUser.UserId,
                    BookId = model.BookID
                });
                db.SaveChanges();
                ModelState.Clear();
            //}

            var readBook = GetBook(model.BookID);
            if (readBook != null)
            {
                return Redirect("/book/read/" + model.BookID);
            }
            return Redirect("/home");
        }

        [HttpPost, ActionName("Rate")]
        //[Authorize]
        public ActionResult Rate(ReadBook model)
        {
            //using (ReadBookDB db = new ReadBookDB())
            //{
                GetCurrentUser(out Models.User currUser);
                var like = db.Like.FirstOrDefault(x => x.BookId == model.BookID && x.UserId == currUser.UserId);
                if (like == null)
                {
                    db.Like.Add(new Like()
                    {
                        UserId = currUser.UserId,
                        BookId = model.BookID,
                        Value = (int)model.NewRating
                    });
                }
                else
                {
                    like.Value = (int)model.NewRating;
                    //db.Entry(like).State = EntityState.Modified;
                }

                db.SaveChanges();
                ModelState.Clear();
            //}

            var readBook = GetBook(model.BookID);
            if (readBook != null)
            {
                return Redirect("/book/read/" + model.BookID);
            }
            return Redirect("/home");
        }

        //[Authorize]
        public ActionResult Publish()
        {
            var book = new Book();
            //book.Genre = Book.FillGenresEnumModel(book.Genre == null ? 0 : (int)book.Genre);
            return View(book);
        }

        [HttpPost]
        //[Authorize]
        public ActionResult Publish(Book book)
        {
            //using (ReadBookDB db = new ReadBookDB())
            //{
                if (db.Book.Any(x => x.BookName == book.BookName))
                {
                    ViewBag.ErrorMessage = "Book Name Already Exist";
                    return View("Publish", book);
                }

                book.PublishDate = DateTime.UtcNow;
                //book.Genre = Book.GenresToInt(book.Genre);
                book.UserId = GetCurrentUser(out Models.User currUser).UserId;
                //book.BookId = db.Book.Add(book).BookID;
                db.SaveChanges();
            //}
            return Redirect("/user/profile");
        }
    }
}
