using Microsoft.AspNetCore.Mvc;
using test.Models;
using test.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

namespace test.Controllers.Page
{   
   
    public class MailsController : Controller
    {
        private readonly ApplicationDbContext context;
        private readonly IWebHostEnvironment environment;
        private readonly IMapper mapper;

        public MailsController(
            ApplicationDbContext context, 
            IWebHostEnvironment environment,
            IMapper mapper)
        {
            this.context = context;
            this.environment = environment;
            this.mapper = mapper;
        }
        
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Edit()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }



        // [HttpGet]
        // public async Task<IActionResult> Details(long id){
        //     var mail = await context.Mail.FindAsync(id);
        //     if (mail == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(mail);
        // }
        // [HttpGet]
        // public async Task<IActionResult> Edit(long id){
        //     var mail = await context.Mail.FindAsync(id);
        //     if (mail == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(mail);
        // }

        // [HttpPost]
        // public async Task<IActionResult> Edit(Mail mail)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return View(mail);
        //     }
            
        //     var existingMail = await context.Mail.FirstOrDefaultAsync(m => m.Id == mail.Id);
        //     if (existingMail == null)
        //     {
        //         return NotFound();
        //     }

        //     mail.EmailCc = mail.EmailCc?.Replace(",", ";");
        //     mail.EmailBcc = mail.EmailBcc?.Replace(",", ";");
        //     mapper.Map(mail, existingMail);
            
        //     await context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        // [HttpPost]
        // public async Task<IActionResult> Create(Mail mail) {
        //     // if (!ModelState.IsValid)
        //     // {
        //     //     return View(mail);
        //     // }

        //     if (!string.IsNullOrEmpty(mail.EmailCc))
        //     {
        //         mail.EmailCc = mail.EmailCc.Replace(",", ";");
        //     }
            
        //     if (!string.IsNullOrEmpty(mail.EmailBcc))
        //     {
        //         mail.EmailBcc = mail.EmailBcc.Replace(",", ";");
        //     }

        //     await context.Mail.AddAsync(mail);
        //     await context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        // [HttpPost]
        // public async Task<IActionResult> Delete (long id )
        // {
        //     var mail = await context.Mail.FindAsync(id);
        //     if (mail != null)
        //     {
        //         context.Mail.Remove(mail);
        //         await context.SaveChangesAsync();
        //     }
        //     return RedirectToAction(nameof(Index));
        // }

        // [HttpPost]
        // public async Task<IActionResult> DeleteMultiple(List<long> ids)
        // {
        //     var mails = await context.Mail.Where(m => ids.Contains(m.Id)).ToListAsync();
        //     context.Mail.RemoveRange(mails);
        //     await context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }   
    }

}
