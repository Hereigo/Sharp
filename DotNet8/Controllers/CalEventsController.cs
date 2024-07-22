using DotNet8.Data;
using DotNet8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNet8.Controllers
{
    public class CalEventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CalEvents
        public async Task<IActionResult> Index()
        {
            var events = await _context.CalEvents.ToListAsync();

            var eventsVm = new List<CalEventVM>();

            foreach (var evt in events)
            {
                eventsVm.Add(new CalEventVM(evt));
            }
            return View(eventsVm);
        }

        // GET: CalEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calEvent = await _context.CalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (calEvent == null)
            {
                return NotFound();
            }

            return View(calEvent);
        }

        // GET: CalEvents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CalEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PeriodSize,Period,Status,Modified,Started,Description")] CalEvent calEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(calEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(calEvent);
        }

        // GET: CalEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calEvent = await _context.CalEvents.FindAsync(id);
            if (calEvent == null)
            {
                return NotFound();
            }
            return View(calEvent);
        }

        // POST: CalEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PeriodSize,Period,Status,Modified,Started,Description")] CalEvent calEvent)
        {
            if (id != calEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(calEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalEventExists(calEvent.Id))
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
            return View(calEvent);
        }

        // GET: CalEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var calEvent = await _context.CalEvents.FirstOrDefaultAsync(m => m.Id == id);
            if (calEvent == null)
            {
                return NotFound();
            }

            return View(calEvent);
        }

        // POST: CalEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calEvent = await _context.CalEvents.FindAsync(id);
            if (calEvent != null)
            {
                _context.CalEvents.Remove(calEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CalEventExists(int id)
        {
            return _context.CalEvents.Any(e => e.Id == id);
        }
    }
}
