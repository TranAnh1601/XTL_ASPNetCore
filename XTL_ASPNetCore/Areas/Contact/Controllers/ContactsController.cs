using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactModel = XTL_ASPNetCore.Models.Contact;
using XTL_ASPNetCore.Models;
using Microsoft.AspNetCore.Authorization;

namespace XTL_ASPNetCore.Areas.Contact.Controllers
{
    [Area("Contact")]
    public class ContactsController : Controller
    {
        private readonly AppDbContext _context;

        public ContactsController(AppDbContext context)
        {
            _context = context;
        }

        [TempData]
        public string StatusMessage { set; get; }

        // GET: Contact/Contacts
        [HttpGet("admin/contact")]
        public async Task<IActionResult> Index()
        {
              return _context.contacts != null ? 
                          View(await _context.contacts.ToListAsync()) :
                          Problem("Entity set 'AppDbContext.contacts'  is null.");
        }

        // GET: Contact/Contacts/Details/5
        [HttpGet("/admin/contact/detail/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contact/Contacts/Create
        [HttpGet("/contact")]
        [AllowAnonymous] // khi truy cap vao contact user phai co vai tro gi do
        public IActionResult SendContact()
        {
            return View();
        }

        [HttpPost("/contact")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendContact([Bind("FullName,Email,Message,Phone")] ContactModel contact)
        {
            if (ModelState.IsValid)
            {
                contact.DateSent = DateTime.Now ;
                _context.Add(contact);
                await _context.SaveChangesAsync();
                StatusMessage = "Lien he cua ban da duoc gui";
                return RedirectToAction("index", "home");
            }
            return View(contact);
        }

        // GET: Contact/Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,DateSent,Message,Phone")] ContactModel contact)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            return View(contact);
        }

        // GET: Contact/Contacts/Delete/5
        [HttpGet("/admin/contact/delete/{id}")]

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contact/Contacts/Delete/5
        [HttpPost("/admin/contact/delete/{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.contacts == null)
            {
                return Problem("Entity set 'AppDbContext.contacts'  is null.");
            }
            var contact = await _context.contacts.FindAsync(id);
            if (contact != null)
            {
                _context.contacts.Remove(contact);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
          return (_context.contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
