using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using iBook.tj.Db;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iBook.tj.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using iBook.tj.Models;
using System.Net.Http;
using System.Web;
using Microsoft.AspNetCore.Identity;
using iBook.tj.Areas.Identity.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Newtonsoft.Json;
using System.Text;

namespace iBook.tj.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private DatabaseContext _db;
        private IWebHostEnvironment webHostEnvironment;
        private readonly UserManager<iBooktjUser> _userManager;

        public AdminController(DatabaseContext db, IWebHostEnvironment hostEnvironment, UserManager<iBooktjUser> userManager)
        {
            _db = db;
            webHostEnvironment = hostEnvironment;
            _userManager = userManager;
        }

        [Route("~/Admin")]
        [Route("~/Admin/Index")]
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Orders()
        {
            return View(); 
        }

        [Route("~/Admin/GetOrders")]
        public  IActionResult GetOrders()
        {
            try
            {
                var ordersList = (from order in _db.Orders
                                  from orderD in _db.OrderDetails
                                  where order.Id == orderD.OrderId
                                  select order).ToList(); 
                return Json(ordersList);
            }
            catch (Exception)
            {
                return BadRequest(); 
            }
        }

        [Route("~/Admin/UpdateOrder/{id}")]
        public async Task<IActionResult> UpdateOrder(int id)
        {
            try
            {
                var order = await _db.Orders.FindAsync(id);
                order.IsDelivered = true;
                var result = _db.SaveChanges();
                if( result > 0)
                {
                    return Json(new { success = "done" }); 
                }
            }
            catch (Exception)
            {
                return BadRequest(); 
            }
            return BadRequest();
        }

        [HttpGet("{id}")]
        [Route("~/Admin/GetOrderInfo/{id}")]
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

        public async Task<IActionResult> Users()
        {
            var usersAdminList = await _userManager.GetUsersInRoleAsync("admin");
            ViewBag.usersAdminList = usersAdminList; 
            return View();
        }

        [Route("~/Admin/GetUsers")]
        public async Task<IActionResult?> GetUsers()
        {
            var usersAdminList = await _userManager.GetUsersInRoleAsync("admin");
            if( usersAdminList != null)
            {
                return Json(usersAdminList);
            }
            return NotFound();
        }

        public IActionResult UsersAddition()
        {
            return View();
        }
        
        [HttpPost]
        [Route("~/Admin/AddUser")]
        public async Task<JsonResult> AddUser()
        {
            try
            {
                using (var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    var user = JsonConvert.DeserializeObject<user_temp>(body);
                    var tempCheckUserName = await _userManager.FindByEmailAsync(user.user_email);
                    if( tempCheckUserName != null)
                    {
                        return Json($"User exists");
                    }
                    var userToRegister = new iBooktjUser { UserName = user.user_email, FirstName = user.user_firstname, LastName = user.user_lastname, Email = user.user_email };
                    var result = await _userManager.CreateAsync(userToRegister, user.user_password);
                    await _userManager.AddToRoleAsync(userToRegister, "admin");
                    if (result.Succeeded)
                    {
                        return Json("success");
                    }
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine(error.Description);
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return Json("failed");
        }
        class user_temp
        {
            public string user_firstname { get; set; }
            public string user_lastname { get; set; }
            public string user_email { get; set; }
            public string user_password { get; set; }
        }

        [HttpDelete]
        [Route("~/Admin/DeleteUser")]
        public async Task<IActionResult> DeleteUser()
        {
            try
            {
                using ( var reader = new StreamReader(Request.Body))
                {
                    var body = await reader.ReadToEndAsync();
                    body = body.Remove(0,1);
                    body = body.Remove(body.Length - 1, 1);
                    var user = await _userManager.FindByEmailAsync(body);
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        return Json(new { success = "success" });
                    }
                    else
                    {
                        return StatusCode(404, new { success = "failed" });
                    }
                }
            }
            catch (Exception)
            {
                return StatusCode(404, new { success = "failed" });
            }
        }


        [Route("~/Admin/Products")]
        public IActionResult Products()
        {
            var productList = (from books in _db.Books
                               from categories in _db.Categories
                               where categories.Id == books.CategoryId
                               select new ProductViewModel
                               {
                                   Id = books.Id,
                                   Title = books.Title,
                                   Author = books.Author,
                                   Description = books.Description,
                                   ImageLink = books.ImageLink,
                                   CategoryName = categories.CategoryName,
                                   BookCost = books.BookCost,
                                   YearOfPublishing = books.YearOfPublishing,
                                   Amount = books.Amount
                               }).ToList();
            ViewBag.listOfBooks = productList;
            ViewBag.categoryList = _db.Categories.ToList();
            return View();
        }

        public IActionResult CategoryAddition()
        {
            ViewBag.CategoryList = _db.Categories.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            foreach (var c in _db.Categories)
            {
                if (c.CategoryName == category.CategoryName)
                {
                    ViewBag.CategoryErrorMessage = "Такая категория уже существует";
                    return View("CategoryAddition");
                }
            }
            byte[] bytes = Encoding.Default.GetBytes(category.CategoryName);
            category.CategoryName = Encoding.UTF8.GetString(bytes);

            await _db.Categories.AddAsync(category);
            var result = await _db.SaveChangesAsync();
            if (result > 0)
            {
                return RedirectToAction(nameof(ProductAddition));
            }
            ViewBag.CategoryErrorMessage = "Произошла ошибка при сохранении";
            return View("CategoryAddition");
        }

        public IActionResult ProductAddition()
        {
            ViewBag.categoryList = _db.Categories.ToList();
            return View();
        }

        [HttpPut("{id}")]
        [Route("~/Admin/UpdateAmount/{id}")]
        public async Task UpdateAmount(int id)
        {
            using ( var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                body = body.Remove(0, 1);
                body = body.Remove(body.Length - 1, 1);
                var amountValue = int.Parse(body);
                if ( int.TryParse(body, out amountValue))
                {
                    var tempBook = await _db.Books.FindAsync(id);
                    if ( tempBook != null)
                    {
                        tempBook.Amount = amountValue;
                        _db.SaveChanges(); 
                    }
                }
            }
        }

        [HttpGet("{id}")]
        [Route("~/Admin/EditProduct/{id}")]
        public IActionResult EditProduct(int id)
        {
            var product = _db.Books.Find(id);
            ViewBag.editProductItem = product;
            ViewBag.categoryList = (_db.Categories.ToList());
            return View();
        }

        [HttpPost]
        public IActionResult EditBook(BookViewModel book)
        {
            ViewBag.categoryList = (_db.Categories.ToList());
            if ( ModelState.IsValid && book.CategoryId != -1 )
            {
                byte[] bytes = Encoding.Default.GetBytes(book.Author);
                book.Author = Encoding.UTF8.GetString(bytes);
                bytes = Encoding.Default.GetBytes(book.Description);
                book.Description = Encoding.UTF8.GetString(bytes);
                bytes = Encoding.Default.GetBytes(book.Title);
                book.Title = Encoding.UTF8.GetString(bytes);

                var result = UpdateAndSave(book.Id, book);
                if( result)
                {
                    ViewData["Message"] = "Успешно изменено";
                }
                else
                {
                    ViewData["Message"] = "Произошла ошибка";
                }
                return RedirectToAction("Products");
            }
            if ( book.CategoryId == -1)
            {
                ViewData["Message"] = "Выберите категорию";
            }
            ViewBag.editProductItem = (_db.Books.Find(book.Id));
            return View("EditProduct");
        }
       
        [NonAction]
        public bool UpdateAndSave(int id, BookViewModel book)
        {
            int result = 0;
            try
            {
                var tempBook = _db.Books.SingleOrDefault(b => b.Id == id);
                if ( tempBook != null)
                {
                    tempBook.Amount = int.Parse(book.Amount);
                    tempBook.Author = book.Author;
                    tempBook.BookCost = double.Parse(book.BookCost);
                    tempBook.CategoryId = book.CategoryId;
                    tempBook.Description = book.Description;
                    tempBook.ImageLink = UploadImage(book);
                    tempBook.Title = book.Title;
                    tempBook.YearOfPublishing = int.Parse(book.YearOfPublishing);
                    result = _db.SaveChanges();

                }
            }
            catch (Exception)
            {
            }
            if( result > 0)
            {
                return true; 
            }
            return false; 
        }

        [HttpGet("{id}")]
        [Route("~/Admin/GetByCategory/{id}")]
        public ActionResult? GetByCategory(int id)
        {
            if (id == -1)
            {
                return BadRequest();
            }
            if (id == -2)
            {
                var productList = (from books in _db.Books
                                   from categories in _db.Categories
                                   where categories.Id == books.CategoryId
                                   select new ProductViewModel
                                   {
                                       Id = books.Id,
                                       Title = books.Title,
                                       Author = books.Author,
                                       Description = books.Description,
                                       ImageLink = books.ImageLink,
                                       CategoryName = categories.CategoryName,
                                       BookCost = books.BookCost,
                                       YearOfPublishing = books.YearOfPublishing,
                                       Amount = books.Amount
                                   }).ToList();

                return Json(productList);
            }
            else
            {
                var productList = (from books in _db.Books
                                   from categories in _db.Categories
                                   where categories.Id == id && categories.Id == books.CategoryId
                                   select new ProductViewModel
                                   {
                                       Id = books.Id,
                                       Title = books.Title,
                                       Author = books.Author,
                                       Description = books.Description,
                                       ImageLink = books.ImageLink,
                                       CategoryName = categories.CategoryName,
                                       BookCost = books.BookCost,
                                       YearOfPublishing = books.YearOfPublishing,
                                       Amount = books.Amount
                                   }).ToList();
                return Json(productList);
            }
        }

        [HttpDelete]
        [Route("~/Admin/DeleteBook/{id}")]
        public async Task<ActionResult?> DeleteBook(int id)
        {
            var result = 0;
            try
            {
                var book = await _db.Books.FindAsync(id);
                var image = book.ImageLink;

                string fileToDelete = Path.Combine(webHostEnvironment.WebRootPath, "images", image);

                if (System.IO.File.Exists(fileToDelete))
                {
                    System.IO.File.Delete(fileToDelete);
                }
                _db.Books.Remove(book);
                result = await _db.SaveChangesAsync();
            }
            catch (Exception)
            {
                return BadRequest();
            }
            if (result > 0)
            {
                return Json(new { success = "done" });
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(BookViewModel model)
        {
            ViewBag.CategoryList = _db.Categories.ToList();
            if (ModelState.IsValid && model.CategoryId != -1)
            {
                byte[] bytes = Encoding.Default.GetBytes(model.Author);
                model.Author = Encoding.UTF8.GetString(bytes);
                bytes = Encoding.Default.GetBytes(model.Description);
                model.Description = Encoding.UTF8.GetString(bytes);
                bytes = Encoding.Default.GetBytes(model.Title);
                model.Title = Encoding.UTF8.GetString(bytes);

                string fileName = UploadImage(model);
                var book = new Book
                {
                    Title = model.Title,
                    ImageLink = fileName,
                    Author = model.Author,
                    BookCost = double.Parse(model.BookCost),
                    CategoryId = model.CategoryId,
                    Description = model.Description,
                    YearOfPublishing = int.Parse(model.YearOfPublishing),
                    Amount = int.Parse(model.Amount)
                };
                _db.Books.Add(book);
                ViewData["Message"] = "Продукт добавлен успешно";
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(ProductAddition));
            }
            return View("ProductAddition");
        }
      
        [NonAction]
        public string UploadImage(BookViewModel model)
        {
            string fileName = null;
            if (model.ImageLink != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                fileName = Guid.NewGuid().ToString() + "_" + model.ImageLink.FileName;
                string filePath = Path.Combine(uploadsFolder, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageLink.CopyTo(fileStream);
                }
            }
            return fileName; 
        }
    }
}
