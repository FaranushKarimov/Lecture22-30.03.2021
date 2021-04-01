using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using iBook.tj.Models;
using Microsoft.AspNetCore.Authorization;
using iBook.tj.Db;
using iBook.tj.Areas.Identity.Data;
using iBook.tj.ViewModels;
using System.IO;
using Newtonsoft.Json;
using System.Text;

namespace iBook.tj.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private DatabaseContext _db;
        private class userData
        {
            public int id { get; set; }
            public string username { get; set; }
            public int bookamount { get; set; }
        }
        private class searchData
        {
            public int category { get; set; }
            public string author { get; set; }
            public string bookname { get; set; }
        }
        private class userOrder
        {
            public string username { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public string phone { get; set; }
            public double cost { get; set; }
            public int amount { get; set; }
            public int cartitem { get; set; }
        }

        public HomeController(ILogger<HomeController> logger, DatabaseContext db)
        {
            _logger = logger;
            _db = db;
        }
       
        public IActionResult Index()
        {
            if (User.IsInRole("admin"))
            {
                return Redirect("~/Admin/Products");
            }
            return View();
        }

        public IActionResult Products()
        {
            if(User.IsInRole("admin"))
            {
                return Redirect("/Admin/Products");
            }
            ViewBag.categoriesList = _db.Categories.ToList();
            return View();
        }

        [Route("~/Home/Product/{id}")]
        public IActionResult Product(int id)
        {
            var product = (from books in _db.Books
                               from categories in _db.Categories
                               where categories.Id == books.CategoryId && books.Id == id
                               select new ProductViewModel
                               {
                                   Id = books.Id,
                                   Author = books.Author,
                                   Title = books.Title,
                                   BookCost = books.BookCost,
                                   CategoryName = categories.CategoryName,
                                   Amount = books.Amount,
                                   Description = books.Description,
                                   ImageLink = books.ImageLink,
                                   YearOfPublishing = books.YearOfPublishing
                               }).FirstOrDefault();
            ViewBag.product = product;
            return View(); 
        }

        [Route("~/Home/SearchByAuthor")]
        public async Task<IActionResult> SearchByAuthor()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var searchInfo = JsonConvert.DeserializeObject<searchData>(body);
                    if (searchInfo.category == 0)
                    {
                        byte[] bytes = Encoding.Default.GetBytes(searchInfo.author);
                        searchInfo.author = Encoding.UTF8.GetString(bytes);

                        bytes = Encoding.Default.GetBytes(searchInfo.bookname);
                        searchInfo.bookname = Encoding.UTF8.GetString(bytes);
                        Console.OutputEncoding = System.Text.Encoding.UTF8;

                        var bookList = _db.Books.ToList();
                        var categoryList = _db.Categories.ToList();
                        List<ProductViewModel> productList = new List<ProductViewModel>(); 

                        foreach( var book in bookList)
                        {
                            foreach( var category in categoryList)
                            {
                                if( category.Id == book.CategoryId)
                                {
                                    var author = book.Author.ToLower();
                                    bytes = Encoding.Default.GetBytes(author);
                                    author = Encoding.UTF8.GetString(bytes);

                                    var title = book.Title.ToLower();
                                    bytes = Encoding.Default.GetBytes(title);
                                    title = Encoding.UTF8.GetString(bytes);
                                    
                                    if (title.Contains(searchInfo.bookname.ToLower()) && author.Contains(searchInfo.author.ToLower()))
                                    {
                                        var product = new ProductViewModel
                                        {
                                            Id = book.Id,
                                            Author = book.Author,
                                            Title = book.Title,
                                            BookCost = book.BookCost,
                                            CategoryName = category.CategoryName,
                                            Amount = book.Amount,
                                            Description = book.Description,
                                            ImageLink = book.ImageLink,
                                            YearOfPublishing = book.YearOfPublishing
                                        };
                                        productList.Add(product);
                                    }

                                }
                            }
                        }
                        return Json(productList);
                    }
                    else
                    {
                        byte[] bytes = Encoding.Default.GetBytes(searchInfo.author);
                        searchInfo.author = Encoding.UTF8.GetString(bytes);

                        bytes = Encoding.Default.GetBytes(searchInfo.bookname);
                        searchInfo.bookname = Encoding.UTF8.GetString(bytes);
                        Console.OutputEncoding = System.Text.Encoding.UTF8;

                        var bookList = _db.Books.ToList();
                        var categoryList = _db.Categories.ToList();
                        List<ProductViewModel> productList = new List<ProductViewModel>();

                        foreach (var book in bookList)
                        {
                            foreach (var category in categoryList)
                            {
                                if (category.Id == searchInfo.category && book.CategoryId == category.Id)
                                {
                                    var author = book.Author.ToLower();
                                    bytes = Encoding.Default.GetBytes(author);
                                    author = Encoding.UTF8.GetString(bytes);

                                    var title = book.Title.ToLower();
                                    bytes = Encoding.Default.GetBytes(title);
                                    title = Encoding.UTF8.GetString(bytes);

                                    if (title.Contains(searchInfo.bookname.ToLower()) 
                                        && author.Contains(searchInfo.author.ToLower())
                                        && category.Id == book.CategoryId)
                                    {
                                        var product = new ProductViewModel
                                        {
                                            Id = book.Id,
                                            Author = book.Author,
                                            Title = book.Title,
                                            BookCost = book.BookCost,
                                            CategoryName = category.CategoryName,
                                            Amount = book.Amount,
                                            Description = book.Description,
                                            ImageLink = book.ImageLink,
                                            YearOfPublishing = book.YearOfPublishing
                                        };
                                        productList.Add(product);
                                    }

                                }
                            }
                        }
                        return Json(productList);
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Route("~/Home/ProductsCategory/{id}")]
        public IActionResult ProductsCategory(int id)
        {
            if (id == -2)
            {
                var productList = (from books in _db.Books
                                   from categories in _db.Categories
                                   where categories.Id == books.CategoryId
                                   select new ProductViewModel
                                   {
                                       Id = books.Id,
                                       Author = books.Author,
                                       Title = books.Title,
                                       BookCost = books.BookCost,
                                       CategoryName = categories.CategoryName,
                                       Amount = books.Amount,
                                       Description = books.Description,
                                       ImageLink = books.ImageLink,
                                       YearOfPublishing = books.YearOfPublishing
                                   }).ToList();
                return Json(productList);
            }
            else
            {
                var productList = (from books in _db.Books
                                   from categories in _db.Categories
                                   where categories.Id == books.CategoryId && categories.Id == id
                                   select new ProductViewModel
                                   {
                                       Id = books.Id,
                                       Author = books.Author,
                                       Title = books.Title,
                                       BookCost = books.BookCost,
                                       CategoryName = categories.CategoryName,
                                       Amount = books.Amount,
                                       Description = books.Description,
                                       ImageLink = books.ImageLink,
                                       YearOfPublishing = books.YearOfPublishing
                                   }).ToList();
                return Json(productList);
            }
        }

        [Authorize]
        public IActionResult Cart()
        {
            return View();
        }

        [HttpGet("{username}")]
        [Route("~/Home/GetCartItems/{username}")]
        public async Task<IActionResult> GetCartItems(string username)
        {
            try
            {
                if (username != "") {
                    var cartItems = (from cartItem in _db.CartItems
                                     from cartItemDetail in _db.CartItemDetails
                                     from book in _db.Books
                                     from category in _db.Categories
                                     where cartItem.Username == username
                                         && cartItemDetail.CartItemId == cartItem.Id
                                         && book.Id == cartItemDetail.BookId
                                         && book.CategoryId == category.Id
                                     select new CartItemViewModel
                                     {
                                         BookId = book.Id,
                                         BookName = book.Title,
                                         BookCost = book.BookCost,
                                         CartItemId = cartItem.Id,
                                         BookAuthor = book.Author,
                                         ImageLink = book.ImageLink,
                                         Amount = cartItemDetail.Amount,
                                         CategoryName = category.CategoryName
                                     }).ToList();

                    return Json(cartItems);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
            return BadRequest();
        }

        [Route("~/Home/UpdateOrderAmount")]
        public async Task<IActionResult> UpdateOrderAmount()
        {
            try
            {
                using ( var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var data = JsonConvert.DeserializeObject<userData>(body);
                    var id = data.id;
                    var bookamount = data.bookamount;
                    var username = data.username;


                    var cartItem = (from cartItemDetail in _db.CartItemDetails
                                    where cartItemDetail.CartItemId == id
                                    select cartItemDetail).FirstOrDefault(); 

                    if( cartItem != null)
                    {
                        var book = _db.Books.Find(cartItem.BookId);
                        if (book.Amount > bookamount)
                        {
                            cartItem.Amount = bookamount;
                            _db.SaveChanges();
                            return Json(new { success = "done" });
                        }
                    }

                }
            }
            catch (Exception)
            {

            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        [Route("~/Home/DeleteCartItem/{id}")]
        public async Task<IActionResult> DeleteCartItem (int id)
        {
            try
            {
                if (id != 0)
                {
                    var tempCartItem = await _db.CartItems.FindAsync(id);
                    _db.CartItems.Remove(tempCartItem);
                    var result = _db.SaveChanges();
                    if( result > 0)
                    {
                        return Json(new { success = "done" });
                    }
                }
                return BadRequest(); 
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("~/Home/AddToCart")]
        public async Task<IActionResult> AddToCart()
        {
            try
            {
                using ( var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var userData = JsonConvert.DeserializeObject<userData>(body);
                   
                    if (userData.username == null || userData.username.Trim() == "")
                    {
                        return Json(new { success = "register" });
                    }
                    if( userData != null)
                    {
                        var cartId = _db.CartItems.Where(c => c.Username == userData.username).Select(t => t.Id).FirstOrDefault(); 
                        if( cartId != 0 && cartId != default )
                        {
                            var cartItemDetail = _db.CartItemDetails.Where(c => c.BookId == userData.id).FirstOrDefault();
                            if(cartItemDetail != default)
                            {
                                cartItemDetail.Amount += userData.bookamount;
                                _db.SaveChanges(); 
                                return Json(new { success = "done"});
                            }
                            else
                            {
                                var cartDetail = new CartItemDetail()
                                {
                                    BookId = userData.id,
                                    CartItemId = cartId,
                                    Amount = userData.bookamount,
                                };
                                _db.CartItemDetails.Add(cartDetail);
                                var resultCartItemDetail = _db.SaveChanges();
                                if (resultCartItemDetail > 0)
                                {
                                    var counter = (_db.CartItems.Where(c => c.Username == userData.username).Select(c => c)).Count();
                                    return Json(new { success = "done", count = counter });
                                }
                            }

                        }
                        else
                        {
                            var cart = new CartItem()
                            {
                                Username = userData.username,
                                TimeAddded = DateTime.Now
                            };
                            await _db.CartItems.AddAsync(cart);
                            var resultCartItem = _db.SaveChanges();
                            if (resultCartItem > 0)
                            {
                                var cartDetail = new CartItemDetail()
                                {
                                    BookId = userData.id,
                                    CartItemId = cart.Id,
                                    Amount = userData.bookamount,
                                };
                                _db.CartItemDetails.Add(cartDetail);
                                var resultCartItemDetail = _db.SaveChanges();
                                if (resultCartItemDetail > 0)
                                {
                                    var counter = (_db.CartItems.Where(c => c.Username == userData.username).Select(c => c)).Count();
                                    return Json(new { success = "done", count = counter });
                                }
                            }
                        }
                    }

                }
                return BadRequest();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        
        [Authorize]
        public IActionResult Orders()
        { 
            return View();
        }
        
        [Route("~/Home/AddOrder")]
        public async Task<IActionResult> AddOrder()
        {
            try
            {
                using ( var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var user = JsonConvert.DeserializeObject<userOrder>(body);
                    var order = (from cartItem in _db.CartItems
                                 where cartItem.Username == user.username
                                 select new Order
                                 {
                                     ReceiverName = user.name,
                                     Username = user.username,
                                     Address = user.address,
                                     PhoneNumber = user.phone,
                                     AmountOfBooks = user.amount,
                                     AmountOfCost = user.cost,
                                     OrderCreatedTime = DateTime.Now
                                 }).FirstOrDefault();
                    _db.Orders.Add(order);
                    var saveResult =_db.SaveChanges(); 
                    var orderId = order.Id;
                    if( saveResult > 0)
                    {
                        var orderDetails = (from cartItem in _db.CartItems
                                            from cartItemDetail in _db.CartItemDetails
                                            where cartItem.Id == cartItemDetail.CartItemId
                                            select new OrderDetail
                                            {
                                                Amount = cartItemDetail.Amount,
                                                BookId = cartItemDetail.BookId,
                                                OrderId = orderId
                                            }).ToList();
                        try
                        {
                            foreach(var orderDetail in orderDetails)
                            {
                                _db.OrderDetails.Add(orderDetail);
                                var book = _db.Books.Find(orderDetail.BookId);
                                book.Amount -= orderDetail.Amount;
                                _db.SaveChanges();
                            }
                            var cartItem = _db.CartItems.Find(user.cartitem);
                            _db.CartItems.Remove(cartItem);
                            var result = _db.SaveChanges();
                            if( result > 0)
                            {
                                return Json(new { success = "done" });
                            }
                        }
                        catch(Exception)
                        {
                            return BadRequest();
                        }
                        return BadRequest();
                    }
                    return BadRequest();
                }
            }
            catch (Exception)
            {

            }
            return BadRequest(); 
        }

        [Route("~/Home/GetOrders/{username}")]
        public async Task<IActionResult> GetOrders(string username)
        {
            var time = DateTime.Now; 
            var orders = _db.Orders.Where(o => o.Username == username
                && o.IsCanceled == false && Math.Abs(time.Day -  o.OrderCreatedTime.Day) <= 2 ).ToList();
            return Json(orders);
        }
        
        [HttpDelete("{id}")]
        [Route("~/Home/DeleteOrder/{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            try
            {
                Console.WriteLine(id);
                var order = await _db.Orders.FindAsync(id);
                if( order != null)
                {
                    order.IsCanceled = true;
                    var result = _db.SaveChanges();
                    if( result > 0)
                    {
                        return Json(new { success = "done" }); 
                    }
                }
            }
            catch (Exception)
            {
                return BadRequest(); 
            }
            return BadRequest(); 
        }

        [HttpGet("{id}")]
        [Route("~/Home/GetOrderInfo/{id}")]
        public async Task<IActionResult> GetOrderInfo(int id)
        {
            try
            {
                var orderInfo = (from order in _db.Orders
                                 from orderDetail in _db.OrderDetails
                                 from book in _db.Books
                                 where order.Id == orderDetail.OrderId 
                                         && order.Id == id
                                         && book.Id == orderDetail.BookId
                                         && order.IsCanceled == false 
                                 select new ProductViewModel
                                 {
                                     Id = book.Id,
                                     BookCost = book.BookCost,
                                     ImageLink = book.ImageLink,
                                     Title = book.Title,
                                     Author = book.Author,
                                     Amount = orderDetail.Amount,
                                 }).ToList();
                return Json(orderInfo);
            }
            catch (Exception)
            {
                return BadRequest(); 
            }
        }

    }
}