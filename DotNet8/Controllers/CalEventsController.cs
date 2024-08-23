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

        public async Task<IActionResult> Index()
        {
            var events = await _context.CalEvents.ToListAsync();
            var eventsVm = new List<CalEvent>();
            foreach (var evt in events)
            {
                eventsVm.Add(evt);
                if (evt.Repeat == CalEventRepeat.EveryXdays)
                {
                    var nextDate = evt.Started.AddDays(evt.EveryXDays.Value);
                    var isCurrentMonth = true;

                    while (nextDate.Month == evt.Month)
                    {
                        eventsVm.Add(new CalEvent()
                        {
                            Id = evt.Id,
                            Day = evt.Started.Day,
                            Description = evt.Description,
                            EveryXDays = evt.EveryXDays,
                            Modified = evt.Modified,
                            Month = (evt.Repeat == CalEventRepeat.Monthly) ? 0 : evt.Started.Month,
                            Repeat = evt.Repeat,
                            Started = nextDate,
                            Status = evt.Status,
                            Time = evt.Time,
                            Year = (evt.Repeat == CalEventRepeat.Yearly) ? 0 : evt.Started.Year
                        });
                        nextDate = nextDate.AddDays(evt.EveryXDays.Value);
                    }
                }
            }
            var today = DateTime.Now;
            var monthMaxDay = Utils.Utils.GetMaxDayOfTheMonth(today);
            for (var i = 1; i <= monthMaxDay; i++)
            {
                eventsVm.Add(new CalEvent(new DateTime(today.Year, today.Month, i)));
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
            var newEvent = new CalEvent()
            {
                EveryXDays = 0,
                Modified = DateTime.Now,
                Started = new DateTime(now.Year, now.Month, id ?? 1),
                Time = new TimeSpan(0, 0, 0),
                Repeat = CalEventRepeat.Once,
            };
            return View(newEvent);
        }

        // TODO:
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CalEvent evnt)
        {
            if (evnt is null)
            {
                throw new ArgumentNullException(nameof(evnt));
            }
            if (!ModelState.IsValid) // For Debugging.
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
            }
            if (ModelState.IsValid)
            {
                // TODO:
                // Process this !!!
                // if (evnt.Repeat == CalEventRepeat.EveryXdays)

                var calEvent = new CalEvent()
                {
                    Day = evnt.Started.Day,
                    Description = evnt.Description,
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
        public async Task<IActionResult> Edit(int id, CalEvent evnt)
        {
            if (id != evnt.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var calEvent = await _context.CalEvents.FindAsync(id);
                    if (calEvent == null)
                    {
                        return NotFound();
                    }
                    calEvent.Day = evnt.Started.Day;
                    calEvent.Description = evnt.Description;
                    calEvent.EveryXDays = evnt.EveryXDays;
                    calEvent.Modified = DateTime.Now;
                    calEvent.Month = (evnt.Repeat == CalEventRepeat.Monthly) ? 0 : evnt.Started.Month;
                    calEvent.Repeat = evnt.Repeat;
                    calEvent.Started = evnt.Started;
                    calEvent.Status = evnt.Status;
                    calEvent.Time = evnt.Time;
                    calEvent.Year = (evnt.Repeat == CalEventRepeat.Yearly) ? 0 : evnt.Started.Year;

                    _context.Update(calEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CalEventExists(evnt.Id))
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
            return View(evnt);
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
