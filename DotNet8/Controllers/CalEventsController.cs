﻿using DotNet8.Data;
using DotNet8.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public async Task<IActionResult> Index()
        {
            var events = await _context.CalEvents.ToListAsync();
            var eventsVm = new List<CalEventVM>();
            foreach (var evt in events)
            {
                eventsVm.Add(new CalEventVM(evt));
            }

            var today = DateTime.Now;
            var monthMaxDay = Utils.Utils.GetMaxDayOfTheMonth(today);

            for (var i = 1; i <= monthMaxDay; i++)
            {
                eventsVm.Add(new CalEventVM(new CalEvent(
                    new DateTime(today.Year, today.Month, i))));
            }

            return View(eventsVm);
        }

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

        public IActionResult Create(int? id)
        {
            var now = DateTime.Now;

            var newEvent = new CalEventVM()
            {
                EveryXDays = 0,
                Started = new DateTime(now.Year, now.Month, id ?? 1),
                Time = new TimeSpan(0, 0, 0),
                Repeat = CalEventRepeat.Once,
                //RepeatList = Enum.GetValues(typeof(CalEventRepeat))
                //    .Cast<CalEventRepeat>()
                //    .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() }).ToList()
            };

            return View(newEvent);
        }

        // POST: CalEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Consumes("application/x-www-form-urlencoded")]
        // public async Task<IActionResult> Create([FromForm] CalEventVM evnt)
        // public async Task<IActionResult> Create([Bind("Description,Started,Time,EveryXDays,Repeat")] CalEventVM evnt)
        // public async Task<IActionResult> Create([Bind("Description,Started,Time,EveryXDays,Repeat")] CalEventVM evnt, string Description)
        public async Task<IActionResult> Create(string Description, CalEventVM evnt)
        {
            if (!ModelState.IsValid)
            {
                // For Debugging.
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }

            if (evnt is null)
            {
                throw new ArgumentNullException(nameof(evnt));
            }
            if (ModelState.IsValid)
            {
                // TODO:
                // Process this !!!
                // if (evnt.Repeat == CalEventRepeat.EveryXdays)

                var calEvent = new CalEvent()
                {
                    // TODO:
                    // Category = CalEventCategory...
                    Day = evnt.Started.Day,
                    Description = Description,
                    EveryXDays = evnt.EveryXDays,
                    Modified = DateTime.Now,
                    Month = (evnt.Repeat == CalEventRepeat.Monthly) ? 0 : evnt.Started.Month,
                    Repeat = evnt.Repeat,
                    Started = evnt.Started,
                    Status = CalEventStatus.Active,
                    Time = evnt.Time,
                    Year = (evnt.Repeat == CalEventRepeat.Yearly) ? 0 : evnt.Started.Year,
                };

                _context.Add(calEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(evnt);
        }

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
