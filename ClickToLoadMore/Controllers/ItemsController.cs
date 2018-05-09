using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClickToLoadMore.Models;
using Microsoft.AspNetCore.Http;
using ClickToLoadMore.Extensions;

namespace ClickToLoadMore.Controllers
{
    public class ItemsController : Controller
    {
        private readonly DatabaseContext _context;
        private const int _recordsPerLoad = 6;

        public ItemsController(DatabaseContext context)
        {
            _context = context;
            ViewBag.RecordsPerLoad = _recordsPerLoad;
        }

        // GET: Items
        public async Task<ActionResult> Index(int? pageNumber)
        {
            pageNumber = pageNumber ?? 0;
            ViewBag.IsEndOfRecords = false;
            if (IsAjaxRequest(Request))
            {
                var items = GetRecordsForPage(pageNumber.Value);
                ViewBag.IsEndOfRecords = ((pageNumber.Value * _recordsPerLoad) >= items.Last().Key);
                return PartialView("_ItemLi", items);
            }
            else
            {
                await LoadAllItemsToSession();
                ViewBag.Items = GetRecordsForPage(pageNumber.Value);
                return View("Index");
            }
        }

        public async Task LoadAllItemsToSession()
        {
            int _itemIndex = 1;
            var _items = await _context.Items.ToListAsync();

            HttpContext.Session.SetObjectAsJson("Items", _items.OrderBy(f => f.Price).ToDictionary(x => _itemIndex++, z => z));
            ViewBag.ItemsCount = _items.Count();
        }

        public Dictionary<int, Item> GetRecordsForPage(int pageNumber)
        {
            var _items = HttpContext.Session.GetObjectFromJson<Dictionary<int, Item>>("Items");

            int from = (pageNumber * _recordsPerLoad);
            int to = from + _recordsPerLoad;

            return _items
                .Where(z => z.Key > from && z.Key <= to)
                .OrderBy(y => y.Key)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return (request.Headers["X-Requested-With"] == "XMLHttpRequest");

            else
            return false;
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .SingleOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ItemName,Price")] Item item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items.SingleOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ItemName,Price")] Item item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.Items
                .SingleOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.Items.SingleOrDefaultAsync(m => m.Id == id);
            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}
